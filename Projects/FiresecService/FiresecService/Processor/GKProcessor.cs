﻿using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecClient;
using GKProcessor;
using SKDDriver;
using ChinaSKDDriver;
using FiresecAPI.SKD;
using SKDDriver.Translators;
using System.Collections.Generic;

namespace FiresecService
{
	public static class GKProcessor
	{
		public static void Create()
		{
			GKProcessorManager.GKProgressCallbackEvent -= new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
			GKProcessorManager.GKProgressCallbackEvent += new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
			GKProcessorManager.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResultEvent);
			GKProcessorManager.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResultEvent);
		}

		public static void Start()
		{
			// DeviceLibraryConfigurationPatchHelper.Patch();
			GKProcessorManager.MustMonitor = true;
			GKProcessorManager.Start();
			GKLicenseProcessor.Start();
		}

		public static void Stop()
		{
			GKProcessorManager.Stop();
		}

		public static void SetNewConfig()
		{
			Stop();
			Create();
			Start();
		}

		static void OnGKProgressCallbackEvent(GKProgressCallback gkProgressCallback)
		{
			FiresecService.Service.FiresecService.NotifyGKProgress(gkProgressCallback);
		}

		static void OnGKCallbackResultEvent(GKCallbackResult gkCallbackResult)
		{
			ChackPendingCards(gkCallbackResult);

			if (gkCallbackResult.JournalItems.Count > 0)
			{
				foreach (var journalItem in gkCallbackResult.JournalItems)
				{
					ProcessPassJournal(journalItem);
					FiresecService.Service.FiresecService.AddCommonJournalItem(journalItem);
				}
			}

			foreach (var doorState in gkCallbackResult.GKStates.DoorStates)
			{
				if (doorState.Door != null)
				{
					var enterZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == doorState.Door.EnterZoneUID);
					if (enterZone != null)
					{
						GKProcessorManager.CalculateSKDZone(enterZone);
						if (!gkCallbackResult.GKStates.SKDZoneStates.Any(x => x.UID == enterZone.UID))
						{
							gkCallbackResult.GKStates.SKDZoneStates.Add(enterZone.State);
						}
					}
					var exitZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == doorState.Door.ExitZoneUID);
					if (exitZone != null)
					{
						GKProcessorManager.CalculateSKDZone(exitZone);
						if (!gkCallbackResult.GKStates.SKDZoneStates.Any(x => x.UID == exitZone.UID))
						{
							gkCallbackResult.GKStates.SKDZoneStates.Add(exitZone.State);
						}
					}
				}
			}
			FiresecService.Service.FiresecService.NotifyGKObjectStateChanged(gkCallbackResult);

			ProcedureRunner.RunOnStateChanged();
		}

		static void ProcessPassJournal(JournalItem journalItem)
		{
			if (journalItem.JournalEventNameType == JournalEventNameType.Проход_пользователя_разрешен)
			{
				Guid? zoneUID = null;
				var door = GKManager.Doors.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
				if (door != null)
				{
					if (journalItem.JournalEventDescriptionType == JournalEventDescriptionType.Вход_Глобал)
					{
						zoneUID = door.EnterZoneUID;
					}
					else if (journalItem.JournalEventDescriptionType == JournalEventDescriptionType.Выход_Глобал)
					{
						zoneUID = door.ExitZoneUID;
					}
				}

				if (zoneUID.HasValue)
				{
					using (var passJournalTranslator = new PassJournalTranslator())
					{
						passJournalTranslator.AddPassJournal(journalItem.EmployeeUID, zoneUID.Value);
					}
				}
			}
		}

		static void ChackPendingCards(GKCallbackResult gkCallbackResult)
		{
			foreach (var deviceState in gkCallbackResult.GKStates.DeviceStates)
			{
				if (deviceState.Device.DriverType == GKDriverType.GK && !deviceState.StateClasses.Contains(XStateClass.Unknown) && !deviceState.StateClasses.Contains(XStateClass.ConnectionLost))
				{
					var gkSKDHelper = new GKSKDHelper();

					using (var databaseService = new SKDDatabaseService())
					{
						var pendingCards = databaseService.CardTranslator.GetAllPendingCards(deviceState.Device.UID);
						foreach (var pendingCard in pendingCards)
						{
							var operationResult = databaseService.CardTranslator.GetSingle(pendingCard.CardUID);
							if (!operationResult.HasError && operationResult.Result != null)
							{
								var card = operationResult.Result;
								var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
								var employeeOperationResult = databaseService.EmployeeTranslator.GetSingle(card.HolderUID);
								var employeeName = employeeOperationResult.Result != null ? employeeOperationResult.Result.FIO : "";
								if ((PendingCardAction)pendingCard.Action == PendingCardAction.Add)
								{
									var addGKResult = gkSKDHelper.AddOneCard(deviceState.Device, card, getAccessTemplateOperationResult.Result, employeeName);
									if (!addGKResult.HasError)
									{
										databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceState.Device.UID);
									}
								}
								if ((PendingCardAction)pendingCard.Action == PendingCardAction.Edit)
								{
									var editGKResult = gkSKDHelper.AddOneCard(deviceState.Device, card, getAccessTemplateOperationResult.Result, employeeName);
									if (!editGKResult.HasError)
									{
										var removeGKCardResult = gkSKDHelper.RemoveOneCard(deviceState.Device, card);
										if (!removeGKCardResult.HasError)
										{
											databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceState.Device.UID);
										}
									}
								}
								if ((PendingCardAction)pendingCard.Action == PendingCardAction.Delete)
								{
									var removeGKCardResult = gkSKDHelper.RemoveOneCard(deviceState.Device, card);
									if (!removeGKCardResult.HasError)
									{
										databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceState.Device.UID);
									}
								}
							}
							else
							{
								databaseService.CardTranslator.DeleteAllPendingCards(pendingCard.CardUID, deviceState.Device.UID);
							}
						}
					}
				}
			}
		}
	}
}