using GKModule.Events;
using GKModule.Plans;
using GKModule.Validation;
using GKModule.ViewModels;
using GKProcessor;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using Ionic.Zip;
using LayoutModule.LayoutParts.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace GKModule
{
	public class GroupControllerModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
	{
		public DevicesViewModel DevicesViewModel { get; private set; }
		public ParameterTemplatesViewModel ParameterTemplatesViewModel { get; private set; }
		public ZonesViewModel ZonesViewModel { get; private set; }
		public DirectionsViewModel DirectionsViewModel { get; private set; }
		public PumpStationsViewModel PumpStationsViewModel { get; private set; }
		public MPTsViewModel MPTsViewModel { get; private set; }
		public DelaysViewModel DelaysViewModel { get; private set; }
		public CodesViewModel CodesViewModel { get; private set; }
		public GuardZonesViewModel GuardZonesViewModel { get; private set; }
		public DoorsViewModel DoorsViewModel { get; private set; }
		public SKDZonesViewModel SKDZonesViewModel { get; private set; }
		public LibraryViewModel DeviceLidraryViewModel { get; private set; }
		public OPCsViewModel OPCViewModel { get; private set; }
		public DescriptorsViewModel DescriptorsViewModel { get; private set; }
		public DiagnosticsViewModel DiagnosticsViewModel { get; private set; }
		GKPlanExtension _planExtension;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<CreateGKZoneEvent>().Subscribe(OnCreateGKZone);
			ServiceFactory.Events.GetEvent<EditGKZoneEvent>().Subscribe(OnEditGKZone);
			ServiceFactory.Events.GetEvent<SelectGKZoneEvent>().Subscribe(OnSelectGKZone);
			ServiceFactory.Events.GetEvent<SelectGKZonesEvent>().Subscribe(OnSelectGKZones);
			ServiceFactory.Events.GetEvent<CreateGKGuardZoneEvent>().Subscribe(OnCreateGKGuardZone);
			ServiceFactory.Events.GetEvent<EditGKGuardZoneEvent>().Subscribe(OnEditGKGuardZone);
			ServiceFactory.Events.GetEvent<SelectGKGuardZoneEvent>().Subscribe(OnSelectGKGuardZone);
			ServiceFactory.Events.GetEvent<SelectGKGuardZonesEvent>().Subscribe(OnSelectGKGuardZones);
			ServiceFactory.Events.GetEvent<CreateGKSKDZoneEvent>().Subscribe(OnCreateSKDZone);
			ServiceFactory.Events.GetEvent<EditGKSKDZoneEvent>().Subscribe(OnEditSKDZone);
			ServiceFactory.Events.GetEvent<CreateGKDelayEvent>().Subscribe(OnCreateGKDelay);
			ServiceFactory.Events.GetEvent<EditGKDelayEvent>().Subscribe(OnEditGKDelay);
			ServiceFactory.Events.GetEvent<SelectGKDelayEvent>().Subscribe(OnSelectGKDelay);
			ServiceFactory.Events.GetEvent<SelectGKDelaysEvent>().Subscribe(OnSelectGKDelays);
			ServiceFactory.Events.GetEvent<CreateGKDirectionEvent>().Subscribe(OnCreateGKDirection);
			ServiceFactory.Events.GetEvent<EditGKDirectionEvent>().Subscribe(OnEditGKDirection);
			ServiceFactory.Events.GetEvent<SelectGKDirectionEvent>().Subscribe(OnSelectGKDirection);
			ServiceFactory.Events.GetEvent<SelectGKDirectionsEvent>().Subscribe(OnSelectGKDirections);
			ServiceFactory.Events.GetEvent<CreateGKMPTEvent>().Subscribe(OnCreateGKMPT);
			ServiceFactory.Events.GetEvent<EditGKMPTEvent>().Subscribe(OnEditGKMPT);
			ServiceFactory.Events.GetEvent<SelectGKMPTEvent>().Subscribe(OnSelectGKMPT);
			ServiceFactory.Events.GetEvent<SelectGKMPTsEvent>().Subscribe(OnSelectGKMPTs);
			ServiceFactory.Events.GetEvent<CreateGKDoorEvent>().Subscribe(OnCreateGKDoor);
			ServiceFactory.Events.GetEvent<EditGKDoorEvent>().Subscribe(OnEditGKDoor);
			ServiceFactory.Events.GetEvent<SelectGKDoorEvent>().Subscribe(OnSelectGKDoor);
			ServiceFactory.Events.GetEvent<SelectGKDoorsEvent>().Subscribe(OnSelectGKDoors);
			ServiceFactory.Events.GetEvent<CreateGKCodeEvent>().Subscribe(OnCreateGKCode);
			ServiceFactory.Events.GetEvent<SelectGKDeviceEvent>().Subscribe(OnSelectGKDevice);
			ServiceFactory.Events.GetEvent<SelectGKDevicesEvent>().Subscribe(OnSelectGKDevices);
			ServiceFactory.Events.GetEvent<SelectGKPumpStationEvent>().Subscribe(OnSelectGKPumpStation);
			ServiceFactory.Events.GetEvent<SelectGKPumpStationsEvent>().Subscribe(OnSelectGKPumpStations);

			DevicesViewModel = new DevicesViewModel();
			ParameterTemplatesViewModel = new ParameterTemplatesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DirectionsViewModel = new DirectionsViewModel();
			DelaysViewModel = new DelaysViewModel();
			PumpStationsViewModel = new PumpStationsViewModel();
			MPTsViewModel = new MPTsViewModel();
			DelaysViewModel = new DelaysViewModel();
			CodesViewModel = new CodesViewModel();
			GuardZonesViewModel = new GuardZonesViewModel();
			DoorsViewModel = new DoorsViewModel();
			SKDZonesViewModel = new SKDZonesViewModel();
			DeviceLidraryViewModel = new LibraryViewModel();
			OPCViewModel = new OPCsViewModel();
			DescriptorsViewModel = new DescriptorsViewModel();
			DiagnosticsViewModel = new DiagnosticsViewModel();
			_planExtension = new GKPlanExtension(DevicesViewModel, ZonesViewModel, GuardZonesViewModel, SKDZonesViewModel, DelaysViewModel, DirectionsViewModel, MPTsViewModel, DoorsViewModel);
		}

		public override void Initialize()
		{
			DevicesViewModel.Initialize();
			ParameterTemplatesViewModel.Initialize();
			ZonesViewModel.Initialize();
			DirectionsViewModel.Initialize();
			PumpStationsViewModel.Initialize();
			MPTsViewModel.Initialize();
			DelaysViewModel.Initialize();
			CodesViewModel.Initialize();
			GuardZonesViewModel.Initialize();
			DoorsViewModel.Initialize();
			SKDZonesViewModel.Initialize();
			OPCViewModel.Initialize();
		}

		public override void RegisterPlanExtension()
		{
			_planExtension.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
			_planExtension.Cache.BuildAllSafe();
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem(ModuleType.ToDescription(), "Tree", new List<NavigationItem>()
				{
					new NavigationItem<ShowGKDeviceEvent, Guid>(DevicesViewModel, "Устройства", "Tree", null, null, Guid.Empty),
					new NavigationItem<ShowGKParameterTemplatesEvent, Guid>(ParameterTemplatesViewModel, "Шаблоны","Briefcase", null, null, Guid.Empty),
					new NavigationItemEx<ShowGKZoneEvent, Guid>(ZonesViewModel, "Пожарные зоны", "Zones", null, null, Guid.Empty),
                    new NavigationItem<ShowGKDirectionEvent, Guid>(DirectionsViewModel, "Направления", "Direction", null, null, Guid.Empty),
					new NavigationItem<ShowGKPumpStationEvent, Guid>(PumpStationsViewModel, "НС", "PumpStation", null, null, Guid.Empty),
					new NavigationItem<ShowGKMPTEvent, Guid>(MPTsViewModel, "МПТ", "MPT", null, null, Guid.Empty),
					new NavigationItem<ShowXDelayEvent, Guid>(DelaysViewModel, "Задержки", "Watch", null, null, Guid.Empty),

					new NavigationItem("Охрана", "tree",
						new List<NavigationItem>()
						{
							new NavigationItem<ShowGKCodeEvent, Guid>(CodesViewModel, "Коды", "User", null, null, Guid.Empty),
							new NavigationItemEx<ShowGKGuardZoneEvent, Guid>(GuardZonesViewModel, "Зоны", "Zones", null, null, Guid.Empty),
						}),
					new NavigationItem("СКД", "tree",
						new List<NavigationItem>()
						{
							new NavigationItemEx<ShowGKDoorEvent, Guid>(DoorsViewModel, "Точки доступа", "DoorW", null, null, Guid.Empty),
							new NavigationItemEx<ShowGKSKDZoneEvent, Guid>(SKDZonesViewModel, "Зоны", "Zones", null, null, Guid.Empty),
						}),

					new NavigationItem<ShowGKDescriptorsEvent, object>(DescriptorsViewModel, "Дескрипторы", "Descriptors"),
					new NavigationItem<ShowGKOPCDevicesEvent, Guid>(OPCViewModel,"OPC Сервер", "tree", null, null, Guid.Empty),
#if DEBUG
					new NavigationItem<ShowGKDeviceLidraryEvent, object>(DeviceLidraryViewModel, "Библиотека", "Book"),
#endif
					new NavigationItem<ShowGKDiagnosticsEvent, object>(DiagnosticsViewModel, "Диагностика", "Bug"),
				}) {IsExpanded = true},
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.GK; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Delays/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Descriptors/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "DeviceLibrary/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Devices/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Diagnostics/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Directions/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Guard/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Journal/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "MPTs/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "OPC/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Parameters/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "PumpStation/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Selectation/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "SKD/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Devices/PmfUsers/DataTemplates/Dictionary.xaml");
		}

		#region IValidationModule Members
		public IEnumerable<IValidationError> Validate()
		{
			var validator = new Validator();
			return validator.Validate();
		}
		#endregion

		private void OnCreateGKZone(CreateGKZoneEventArg createZoneEventArg)
		{
			ZonesViewModel.CreateZone(createZoneEventArg);
		}
		private void OnEditGKZone(Guid zoneUID)
		{
			ZonesViewModel.EditZone(zoneUID);
		}
		void OnSelectGKZone(SelectGKZoneEventArg selectZoneEventArg)
		{
			var zoneSelectionViewModel = new ZoneSelectionViewModel(selectZoneEventArg.Zone);
			selectZoneEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(zoneSelectionViewModel);
			selectZoneEventArg.Zone = selectZoneEventArg.Cancel || zoneSelectionViewModel.SelectedZone == null ?
				null :
				zoneSelectionViewModel.SelectedZone.Zone;
		}
		void OnSelectGKZones(SelectGKZonesEventArg selectZonesEventArg)
		{
			var zonesSelectionViewModel = new ZonesSelectationViewModel(selectZonesEventArg.Zones);
			selectZonesEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(zonesSelectionViewModel);
			selectZonesEventArg.Zones = zonesSelectionViewModel.TargetZones.ToList();
		}

		private void OnCreateGKGuardZone(CreateGKGuardZoneEventArg createZoneEventArg)
		{
			GuardZonesViewModel.CreateZone(createZoneEventArg);
		}
		private void OnEditGKGuardZone(Guid zoneUID)
		{
			GuardZonesViewModel.EditZone(zoneUID);
		}
		void OnSelectGKGuardZone(SelectGKGuardZoneEventArg selectGuardZoneEventArg)
		{
			var zoneSelectionViewModel = new GuardZoneSelectionViewModel(selectGuardZoneEventArg.GuardZone);
			selectGuardZoneEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(zoneSelectionViewModel);
			selectGuardZoneEventArg.GuardZone = selectGuardZoneEventArg.Cancel || zoneSelectionViewModel.SelectedZone == null ?
				null :
				zoneSelectionViewModel.SelectedZone.Zone;
		}
		void OnSelectGKGuardZones(SelectGKGuardZonesEventArg selectGuardZonesEventArg)
		{
			var zonesSelectionViewModel = new GuardZonesSelectationViewModel(selectGuardZonesEventArg.GuardZones);
			selectGuardZonesEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(zonesSelectionViewModel);
			selectGuardZonesEventArg.GuardZones = zonesSelectionViewModel.TargetZones.ToList();
		}

		public void OnCreateGKDelay(CreateGKDelayEventArgs createDelayEventArg)
		{
			DelaysViewModel.CreateDelay(createDelayEventArg);
		}

		public void OnEditGKDelay(Guid delayUID)
		{
			DelaysViewModel.EditDelay(delayUID);
		}
		void OnSelectGKDelay(SelectGKDelayEventArg selectDelayEventArg)
		{
			var delaySelectionViewModel = new DelaySelectionViewModel(selectDelayEventArg.Delay);
			selectDelayEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(delaySelectionViewModel);
			selectDelayEventArg.Delay = selectDelayEventArg.Cancel || delaySelectionViewModel.SelectedDelay == null ?
				null :
				delaySelectionViewModel.SelectedDelay.Delay;
		}
		void OnSelectGKDelays(SelectGKDelaysEventArg selectDelaysEventArg)
		{
			var delaysSelectionViewModel = new DelaysSelectationViewModel(selectDelaysEventArg.Delays);
			selectDelaysEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(delaysSelectionViewModel);
			selectDelaysEventArg.Delays = delaysSelectionViewModel.TargetDelays.ToList();
		}

		private void OnCreateGKDirection(CreateGKDirectionEventArg createDirectionEventArg)
		{
			DirectionsViewModel.CreateDirection(createDirectionEventArg);
		}
		private void OnEditGKDirection(Guid directionUID)
		{
			DirectionsViewModel.EditDirection(directionUID);
		}
		void OnSelectGKDirection(SelectGKDirectionEventArg selectDirectionEventArg)
		{
			var directionSelectionViewModel = new DirectionSelectionViewModel(selectDirectionEventArg.Direction);
			selectDirectionEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(directionSelectionViewModel);
			selectDirectionEventArg.Direction = selectDirectionEventArg.Cancel || directionSelectionViewModel.SelectedDirection == null ?
				null :
				directionSelectionViewModel.SelectedDirection.Direction;
		}
		void OnSelectGKDirections(SelectGKDirectionsEventArg selectDirectionsEventArg)
		{
			var directionsSelectionViewModel = new DirectionsSelectationViewModel(selectDirectionsEventArg.Directions);
			selectDirectionsEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(directionsSelectionViewModel);
			selectDirectionsEventArg.Directions = directionsSelectionViewModel.TargetDirections.ToList();
		}

		private void OnCreateGKMPT(CreateGKMPTEventArg createMPTEventArg)
		{
			MPTsViewModel.CreateMPT(createMPTEventArg);
		}
		private void OnEditGKMPT(Guid mptUID)
		{
			MPTsViewModel.EditMPT(mptUID);
		}
		void OnSelectGKMPT(SelectGKMPTEventArg selectMPTEventArg)
		{
			var mptSelectionViewModel = new MPTSelectionViewModel(selectMPTEventArg.MPT);
			selectMPTEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(mptSelectionViewModel);
			selectMPTEventArg.MPT = selectMPTEventArg.Cancel || mptSelectionViewModel.SelectedMPT == null ?
				null :
				mptSelectionViewModel.SelectedMPT.MPT;
		}
		void OnSelectGKMPTs(SelectGKMPTsEventArg selectMPTsEventArg)
		{
			var mptsSelectionViewModel = new MPTsSelectationViewModel(selectMPTsEventArg.MPTs);
			selectMPTsEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(mptsSelectionViewModel);
			selectMPTsEventArg.MPTs = mptsSelectionViewModel.TargetMPTs.ToList();
		}

		private void OnCreateGKDoor(CreateGKDoorEventArg createGKDoorEventArg)
		{
			DoorsViewModel.CreateDoor(createGKDoorEventArg);
		}
		private void OnEditGKDoor(Guid doorUID)
		{
			DoorsViewModel.EditDoor(doorUID);
		}
		void OnSelectGKDoor(SelectGKDoorEventArg selectDoorEventArg)
		{
			var doorSelectionViewModel = new GKDoorSelectionViewModel(selectDoorEventArg.Door);
			selectDoorEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(doorSelectionViewModel);
			selectDoorEventArg.Door = selectDoorEventArg.Cancel || doorSelectionViewModel.SelectedDoor == null ?
				null :
				doorSelectionViewModel.SelectedDoor.Door;
		}
		void OnSelectGKDoors(SelectGKDoorsEventArg selectDoorsEventArg)
		{
			var doorsSelectionViewModel = new DoorsSelectationViewModel(selectDoorsEventArg.Doors);
			selectDoorsEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(doorsSelectionViewModel);
			selectDoorsEventArg.Doors = doorsSelectionViewModel.TargetDoors.ToList();
		}

		private void OnCreateSKDZone(CreateGKSKDZoneEventArg createZoneEventArg)
		{
			SKDZonesViewModel.CreateZone(createZoneEventArg);
		}
		private void OnEditSKDZone(Guid zoneUID)
		{
			SKDZonesViewModel.EditZone(zoneUID);
		}
		private void OnCreateGKCode(CreateGKCodeEventArg createGKCodeEventArg)
		{
			CodesViewModel.CreateCode(createGKCodeEventArg);
		}

		void OnSelectGKDevice(SelectGKDeviceEventArg selectDeviceEventArg)
		{
			var deviceSelectionViewModel = new DeviceSelectionViewModel(selectDeviceEventArg.Device);
			selectDeviceEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(deviceSelectionViewModel);
			selectDeviceEventArg.Device = selectDeviceEventArg.Cancel || deviceSelectionViewModel.SelectedDevice == null ?
				null :
				deviceSelectionViewModel.SelectedDevice.Device;
		}
		void OnSelectGKDevices(SelectGKDevicesEventArg selectDevicesEventArg)
		{
			var devicesSelectionViewModel = new DevicesSelectationViewModel(selectDevicesEventArg.Devices);
			selectDevicesEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(devicesSelectionViewModel);
			selectDevicesEventArg.Devices = devicesSelectionViewModel.DevicesList;
		}

		void OnSelectGKPumpStation(SelectGKPumpStationEventArg selectPumpStationEventArg)
		{
			var pumpStationSelectionViewModel = new PumpStationSelectionViewModel(selectPumpStationEventArg.PumpStation);
			selectPumpStationEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(pumpStationSelectionViewModel);
			selectPumpStationEventArg.PumpStation = selectPumpStationEventArg.Cancel || pumpStationSelectionViewModel.SelectedPumpStation == null ?
				null :
				pumpStationSelectionViewModel.SelectedPumpStation.PumpStation;
		}
		void OnSelectGKPumpStations(SelectGKPumpStationsEventArg selectPumpStationsEventArg)
		{
			var pumpStationsSelectionViewModel = new PumpStationsSelectationViewModel(selectPumpStationsEventArg.PumpStations);
			selectPumpStationsEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(pumpStationsSelectionViewModel);
			selectPumpStationsEventArg.PumpStations = pumpStationsSelectionViewModel.TargetPumpStations.ToList();
		}

		public override void AfterInitialize()
		{
			SafeFiresecService.CallbackOperationResultEvent -= new Action<CallbackOperationResult>(OnCallbackOperationResult);
			SafeFiresecService.CallbackOperationResultEvent += new Action<CallbackOperationResult>(OnCallbackOperationResult);
		}

		void OnCallbackOperationResult(CallbackOperationResult callbackOperationResult)
		{
			if (callbackOperationResult != null)
			{
				ApplicationService.Invoke(() =>
				{
					LoadingService.Close();

					if (callbackOperationResult.CallbackOperationResultType == CallbackOperationResultType.GetAllUsers)
					{
						if (!callbackOperationResult.HasError)
						{
							GKUsersViewModel gkUsersViewModel = null;
							WaitHelper.Execute(() =>
							{
								gkUsersViewModel = new GKUsersViewModel(callbackOperationResult.Users);
							});
							//LoadingService.Close();
							DialogService.ShowModalWindow(gkUsersViewModel);
						}
						else
						{
							//LoadingService.Close();
							MessageBoxService.ShowWarning(callbackOperationResult.Error, "Ошибка при перезаписи пользователей");
						}
					}
					if (callbackOperationResult.CallbackOperationResultType == CallbackOperationResultType.RewriteUsers)
					{
						if (!callbackOperationResult.HasError)
						{
							MessageBoxService.Show("Операция перезаписи пользователей завершилась успешно");
						}
						else
						{
							MessageBoxService.ShowWarning(callbackOperationResult.Error, "Ошибка при получении пользователей");
						}
					}
					if (callbackOperationResult.CallbackOperationResultType == CallbackOperationResultType.WriteConfiguration)
					{
						if (!callbackOperationResult.HasError)
						{
							MessageBoxService.Show("Операция записи конфигурации в прибор завершилась успешно");
						}
						else
						{
							MessageBoxService.ShowWarning(callbackOperationResult.Error, "Ошибка при записи конфигурации в прибор");
						}
					}
					if (callbackOperationResult.CallbackOperationResultType == CallbackOperationResultType.ReadConfigurationFromGKFile)
					{
						if (!callbackOperationResult.HasError)
						{
							var stream = ClientManager.FiresecService.GetServerFile(callbackOperationResult.FileName);

							if (stream != Stream.Null)
							{
								var folderName = AppDataFolderHelper.GetLocalFolder("GKFile");
								var configFileName = Path.Combine(folderName, "Config.fscp");
								if (Directory.Exists(folderName))
									Directory.Delete(folderName, true);
								Directory.CreateDirectory(folderName);

								var configFileStream = File.Create(configFileName);
								ClientManager.CopyStream(stream, configFileStream);
								configFileStream.Close();

								if (new FileInfo(configFileName).Length == 0)
								{
									MessageBoxService.ShowError("Ошибка при чтении конфигурации");
									return;
								}

								var zipFile = ZipFile.Read(configFileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
								var fileInfo = new FileInfo(configFileName);
								var unzipFolderPath = fileInfo.Directory.FullName;
								zipFile.ExtractAll(unzipFolderPath);
								zipFile.Dispose();
								var configurationFileName = Path.Combine(unzipFolderPath, "GKDeviceConfiguration.xml");
								if (!File.Exists(configurationFileName))
								{
									MessageBoxService.ShowError("Ошибка при распаковке файла");
									return;
								}
								var deviceConfiguration = ZipSerializeHelper.DeSerialize<GKDeviceConfiguration>(configurationFileName, true);

								ConfigurationCompareViewModel configurationCompareViewModel = null;
								WaitHelper.Execute(() =>
								{
									DescriptorsManager.Create();
									deviceConfiguration.UpdateConfiguration();
									deviceConfiguration.PrepareDescriptors();
									configurationCompareViewModel = new ConfigurationCompareViewModel(GKManager.DeviceConfiguration, deviceConfiguration, DevicesViewModel.SelectedDevice.Device, configFileName);
								});
								//LoadingService.Close();
								if (configurationCompareViewModel.Error != null)
								{
									MessageBoxService.ShowError(configurationCompareViewModel.Error, "Ошибка при чтении конфигурации");
									return;
								}
								if (DialogService.ShowModalWindow(configurationCompareViewModel))
									ServiceFactoryBase.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
							}
							else
							{
								//LoadingService.Close();
								MessageBoxService.ShowWarning("Ошибка при чтении конфигурационного файла");
							}
						}
						else
						{
							MessageBoxService.ShowWarning(callbackOperationResult.Error, "Ошибка при чтении конфигурации из прибора");
						}
					}
				});
			}
		}

		#region ILayoutDeclarationModule Members

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			Converter<ILayoutProperties, BaseLayoutPartViewModel> factory = (p) => new LayoutPartWithAdditioanlPropertiesViewModel(p as LayoutPartAdditionalProperties);
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Indicator, 110, "Индикаторы", "Панель индикаторов состояния", "BAlarm.png", false, new LayoutPartSize() { PreferedSize = new Size(1000, 100) });
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.ConnectionIndicator, 111, "Индикатор связи", "Панель индикаторов связи", "BConnectionIndicator.png", false, new LayoutPartSize() { PreferedSize = new Size(50, 30) });
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Alarms, 112, "Состояния", "Панель состояний", "BAlarm.png", false);
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.GDevices, 113, "Устройства", "Панель с устройствами", "BTree.png", false) { Factory = factory };
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Zones, 114, "Зоны", "Панель зон", "BZones.png", false) { Factory = factory };
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.GuardZones, 115, "Охранные зоны", "Панель охранных зон", "BZones.png", false) { Factory = factory };
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Directions, 116, "Направления", "Панель направления", "BDirection.png", false);
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.PumpStations, 117, "НС", "Панель НС", "BPumpStation.png", false);
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.MPTs, 118, "МПТ", "Панель МПТ", "BMPT.png", false) { Factory = factory };
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Delays, 119, "Задержки", "Панель задержек", "Delay.png", false);
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Doors, 120, "Точки доступа", "Панель точек досткпа", "Door.png", false) { Factory = factory };
		}
		#endregion
	}
}