﻿using NUnit.Framework;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GKIntegratedTest
{
	public partial class ItegratedTest
	{
		[Test]
		public void TestDoor()
		{
			var cardReaderDevice = AddDevice(kauDevice11, GKDriverType.RSR2_CardReader);
			var amDevice = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var am2Device = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var rmDevice = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
			var door = new GKDoor { Name = "Тестовая ТД", No = 1, DoorType = GKDoorType.OneWay};
			door.EnterDeviceUID = cardReaderDevice.UID;
			door.ExitDeviceUID = amDevice.UID;
			door.LockControlDeviceUID = am2Device.UID;
			door.LockDeviceUID = rmDevice.UID;
			GKManager.AddDoor(door);
			SetConfigAndRestartImitator();
			WaitWhileState(door, XStateClass.Off, 10000, "Ждем выключено в ТД");
			Assert.IsTrue(door.State.StateClass == XStateClass.Off, "Проверка того, что ТД выключено");
			var card = AddNewUser(10, door);
			var traceMessage = "Прикладывание карты с номером " + card.Number + " к считывателю";
			EnterCard(cardReaderDevice, card, GKCodeReaderEnterType.CodeOnly, traceMessage);
			WaitWhileState(cardReaderDevice, XStateClass.Attention, 3000, "Ждем внимания в считывателе");
			WaitWhileState(door, XStateClass.On, 5000, "Ждем включено в ТД");
			Assert.IsTrue(door.State.StateClass == XStateClass.On, "Проверка того, что ТД перешла в сотояние включено");
			CheckJournal(JournalItem(cardReaderDevice, JournalEventNameType.Внимание),
				JournalItem(door, JournalEventNameType.Проход_пользователя_разрешен), JournalItem(door, JournalEventNameType.Включено));
		}
	}
}
