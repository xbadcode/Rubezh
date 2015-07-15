﻿using FiresecAPI.GK;
using GKProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKImitator.Processor
{
	public static class UsersProcessor
	{
		public static void AddUser(List<byte> bytes)
		{
			var imitatorUser = ImitatorUserFromBytes(bytes);
			var packNo = BytesHelper.SubstructShort(bytes, 255);
			imitatorUser.GKNo = DBHelper.ImitatorSerializedCollection.ImitatorUsers.Count;
			DBHelper.ImitatorSerializedCollection.ImitatorUsers.Add(imitatorUser);
		}

		public static void EditUser(List<byte> bytes)
		{
			var imitatorUser = ImitatorUserFromBytes(bytes);
			var packNo = BytesHelper.SubstructShort(bytes, 255);
			var existingImitatorUser = DBHelper.ImitatorSerializedCollection.ImitatorUsers.FirstOrDefault(x => x.GKNo == imitatorUser.GKNo);
			existingImitatorUser = imitatorUser;
		}

		public static List<byte> ReadUser(int gkUserNo)
		{
			var imitatorUser = DBHelper.ImitatorSerializedCollection.ImitatorUsers.FirstOrDefault(x => x.GKNo == gkUserNo);
			if (imitatorUser != null)
			{
				var bytes = new List<byte>();
				bytes.Add(0);
				bytes.AddRange(BytesHelper.ShortToBytes((ushort)(imitatorUser.GKNo)));
				bytes.Add((byte)imitatorUser.CardType);
				bytes.Add(0);
				var nameBytes = BytesHelper.StringDescriptionToBytes(imitatorUser.Name);
				bytes.AddRange(nameBytes);
				bytes.AddRange(BytesHelper.IntToBytes((int)imitatorUser.Number));
				bytes.Add((byte)imitatorUser.Level);
				bytes.Add((byte)imitatorUser.ScheduleNo);

				bytes.Add(0);
				bytes.Add(0);

				bytes.AddRange(BytesHelper.IntToBytes((int)imitatorUser.TotalSeconds));

				for (int packNo = 0; packNo < 65535; packNo++)
				{
					var startCardScheduleNo = 0;
					var cardScheduleCount = 68;
					if (packNo > 0)
					{
						startCardScheduleNo = 68 + (packNo - 1) * 84;
						cardScheduleCount = 84;
					}
					var cardSchedules = imitatorUser.ImitatorUserDevices.Skip(startCardScheduleNo).Take(cardScheduleCount).ToList();
					if (cardSchedules.Count == 0)
						break;

					foreach (var cardSchedule in cardSchedules)
					{
						bytes.AddRange(BytesHelper.ShortToBytes((ushort)cardSchedule.DescriptorNo));
					}
					for (int i = 0; i < cardScheduleCount - cardSchedules.Count; i++)
					{
						bytes.Add(0);
						bytes.Add(0);
					}
					foreach (var cardSchedule in cardSchedules)
					{
						bytes.Add((byte)cardSchedule.ScheduleNo);
					}
					for (int i = 0; i < cardScheduleCount - cardSchedules.Count; i++)
					{
						bytes.Add(0);
					}

					bytes.Add(0);
					bytes.Add(0);

					if (startCardScheduleNo + cardScheduleCount < imitatorUser.ImitatorUserDevices.Count)
					{
						bytes.AddRange(BytesHelper.ShortToBytes((ushort)(packNo + 1)));
					}
					else
					{
						bytes.Add(0);
						bytes.Add(0);
					}
				}
				return bytes;
			}
			return new List<byte>();
		}

		static ImitatorUser ImitatorUserFromBytes(List<byte> bytes)
		{
			var imitatorUser = new ImitatorUser();
			imitatorUser.GKNo = BytesHelper.SubstructShort(bytes, 0);
			imitatorUser.CardType = (GKCardType)bytes[2];
			imitatorUser.IsBlocked = bytes[3] == 1;
			imitatorUser.Name = BytesHelper.BytesToStringDescription(bytes, 4);
			imitatorUser.Number = BytesHelper.SubstructInt(bytes, 36);
			imitatorUser.Level = bytes[40];
			imitatorUser.ScheduleNo = bytes[41];
			imitatorUser.TotalSeconds = BytesHelper.SubstructInt(bytes, 44);

			var descriptorNos = new List<int>();
			for (int i = 0; i <= 68; i++)
			{
				var descriptorNo = BytesHelper.SubstructInt(bytes, 48 + i * 2);
				descriptorNos.Add(descriptorNo);
			}

			var sheduleNos = new List<int>();
			for (int i = 0; i <= 68; i++)
			{
				var sheduleNo = BytesHelper.SubstructInt(bytes, 184 + i * 2);
				sheduleNos.Add(sheduleNo);
			}

			for (int i = 0; i <= 68; i++)
			{
				var descriptorNo = descriptorNos[i];
				var sheduleNo = sheduleNos[i];
				if (descriptorNo == 0)
					break;
				var imitatorUserDevice = new ImitatorUserDevice()
				{
					DescriptorNo = descriptorNo,
					ScheduleNo = sheduleNo
				};
				imitatorUser.ImitatorUserDevices.Add(imitatorUserDevice);
			}
			return imitatorUser;
		}
	}
}