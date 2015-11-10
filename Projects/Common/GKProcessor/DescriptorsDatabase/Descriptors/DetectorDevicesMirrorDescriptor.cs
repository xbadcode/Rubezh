﻿using RubezhAPI.GK;

namespace GKProcessor
{
	public class DetectorDevicesMirrorDescriptor : BaseDescriptor
	{
		GKDevice Device { get; set; }

		public DetectorDevicesMirrorDescriptor(GKDevice device)
			: base(device)
		{
			DescriptorType = DescriptorType.Device;
			Device = device;
			foreach (var dev in Device.GKReflectionItem.Devices)
			{
				Device.LinkToDescriptor(dev);
			}
			foreach (var dir in Device.GKReflectionItem.Diretions)
			{
				Device.LinkToDescriptor(dir);
			}
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(Device.Driver.DriverTypeNo);

			var address = 0;
			if (Device.Driver.IsDeviceOnShleif)
				address = (Device.ShleifNo - 1) * 256 + Device.IntAddress;
			SetAddress((ushort)address);
		}

		public override void BuildFormula()
		{
			Formula = new FormulaBuilder();
			int count = 0;
			foreach (var device in Device.GKReflectionItem.Devices)
			{
				Formula.AddGetWord(false, device);
				count++;
				if (count > 1)
				{
					Formula.Add(FormulaOperationType.OR);
				}
			}
			foreach (var direction in Device.GKReflectionItem.Diretions)
			{
				Formula.AddGetWord(false, direction);
				count++;
				if (count > 1)
				{
					Formula.Add(FormulaOperationType.OR);
				}
			}
			count = 0;
			foreach (var device in Device.GKReflectionItem.Devices)
			{
				Formula.AddGetWord(true, device);
				count++;
				if (count > 1)
				{
					Formula.Add(FormulaOperationType.OR);
				}
			}
			foreach (var direction in Device.GKReflectionItem.Diretions)
			{
				Formula.AddGetWord(true, direction);
				count++;
				if (count > 1)
				{
					Formula.Add(FormulaOperationType.OR);
				}
			}

			Formula.Add(FormulaOperationType.CONST, 0, 0x400);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutWord(true, Device);
			Formula.AddPutWord(false, Device);
		}
	}
}
