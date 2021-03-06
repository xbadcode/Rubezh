﻿using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI;

namespace GKProcessor
{
	public abstract class BaseDescriptor
	{
		public DatabaseType DatabaseType { get; set; }
		public int No { get; set; }
		public DescriptorType DescriptorType { get; protected set; }
		public GKBase GKBase { get; private set; }

		public ushort ControllerAdress { get; protected set; }
		public ushort AdressOnController { get; protected set; }
		public ushort PhysicalAdress { get; protected set; }

		public List<byte> DeviceType { get; protected set; }
		public List<byte> Address { get; protected set; }
		public List<byte> Description { get; protected set; }
		public List<byte> Offset { get; protected set; }
		public List<byte> InputDependensesCount { get; private set; }
		public List<byte> InputDependenses { get; protected set; }
		public List<byte> OutputDependensesCount { get; private set; }
		public List<byte> OutputDependenses { get; protected set; }
		public List<byte> FormulaBytes { get; set; }
		public List<byte> ParametersCount { get; private set; }
		public List<byte> Parameters { get; protected set; }
		public List<byte> AllBytes { get; private set; }
		public FormulaBuilder Formula { get; set; }
		public bool IsFormulaGeneratedOutside { get; set; }

		public BaseDescriptor(GKBase gkBase)
		{
			GKBase = gkBase;
			Formula = new FormulaBuilder();
			Parameters = new List<byte>();
		}

		protected void SetAddress(ushort address)
		{
			PhysicalAdress = address;
			Address = new List<byte>();

			switch (DatabaseType)
			{
				case DatabaseType.Gk:
					if (GKBase.KauDatabaseParent != null)
					{
						ushort lineNo = GKManager.GetKauLine(GKBase.KauDatabaseParent);
						ControllerAdress = (ushort)(lineNo * 256 + GKBase.KauDatabaseParent.IntAddress);
						AdressOnController = GKBase.KAUDescriptorNo;
					}
					else
					{
						ControllerAdress = 0x200;
						AdressOnController = GKBase.GKDescriptorNo;
					}
					Address.AddRange(BytesHelper.ShortToBytes(ControllerAdress));
					Address.AddRange(BytesHelper.ShortToBytes(AdressOnController));
					Address.AddRange(BytesHelper.ShortToBytes(PhysicalAdress));
					break;

				case DatabaseType.Mirror:
					ControllerAdress = 0x200;
					AdressOnController = GKBase.KAUDescriptorNo;
					Address.AddRange(BytesHelper.ShortToBytes(ControllerAdress));
					Address.AddRange(BytesHelper.ShortToBytes(AdressOnController));
					Address.AddRange(BytesHelper.ShortToBytes(PhysicalAdress));
					break;

				case DatabaseType.Kau:
					Address.AddRange(BytesHelper.ShortToBytes(PhysicalAdress));
					break;
			}
		}

		void InitializeInputOutputDependences()
		{
			InputDependenses = new List<byte>();
			OutputDependenses = new List<byte>();

			if (DatabaseType == DatabaseType.Gk)
			{
				foreach (var inputGKBase in GKBase.InputDescriptors)
				{
					InputDependenses.AddRange(BitConverter.GetBytes(inputGKBase.GKDescriptorNo));
				}
				foreach (var outputGKBase in GKBase.OutputDescriptors)
				{
					OutputDependenses.AddRange(BitConverter.GetBytes(outputGKBase.GKDescriptorNo));
				}
			}

			if (DatabaseType == DatabaseType.Kau || DatabaseType == DatabaseType.Mirror)
			{
				foreach (var outputGKBase in GKBase.OutputDescriptors.Where(x => x.KauDatabaseParent == GKBase.KauDatabaseParent))
				{
					OutputDependenses.AddRange(BitConverter.GetBytes(outputGKBase.KAUDescriptorNo));
				}
			}
		}

		public void InitializeDescription()
		{
			switch (DatabaseType)
			{
				case DatabaseType.Gk:
				case DatabaseType.Mirror:
					Description = BytesHelper.StringDescriptionToBytes(GKBase.GetGKDescriptorName(GKManager.DeviceConfiguration.GKNameGenerationType));
					break;

				case DatabaseType.Kau:
					Description = new List<byte>();
					break;
			}
		}

		public void InitializeOffset()
		{
			int offsetToParameters = 0;
			switch (DatabaseType)
			{
				case DatabaseType.Gk:
				case DatabaseType.Mirror:
					offsetToParameters = 2 + 6 + 32 + 2 + 2 + InputDependenses.Count + 2 + OutputDependenses.Count + FormulaBytes.Count;
					break;

				case DatabaseType.Kau:
					offsetToParameters = 2 + 2 + 2 + 2 + OutputDependenses.Count + FormulaBytes.Count;
					break;
			}
			Offset = BytesHelper.ShortToBytes((ushort)offsetToParameters);
		}

		public void InitializeAllBytes()
		{
			InitializeDescription();
			InitializeInputOutputDependences();

			InputDependensesCount = BytesHelper.ShortToBytes((ushort)(InputDependenses.Count / 2));
			OutputDependensesCount = BytesHelper.ShortToBytes((ushort)(OutputDependenses.Count / 2));
			ParametersCount = BytesHelper.ShortToBytes((ushort)(Parameters.Count / 4));

			InitializeOffset();

			AllBytes = new List<byte>();
			AllBytes.AddRange(DeviceType);
			AllBytes.AddRange(Address);
			AllBytes.AddRange(Description);
			AllBytes.AddRange(Offset);
			if (DatabaseType == DatabaseType.Gk || DatabaseType == DatabaseType.Mirror)
			{
				AllBytes.AddRange(InputDependensesCount);
				AllBytes.AddRange(InputDependenses);
			}
			AllBytes.AddRange(OutputDependensesCount);
			AllBytes.AddRange(OutputDependenses);
			AllBytes.AddRange(FormulaBytes);
			AllBytes.AddRange(ParametersCount);
			AllBytes.AddRange(Parameters);
		}

		public ushort GetDescriptorNo()
		{
			switch (DatabaseType)
			{
				case DatabaseType.Gk:
					return GKBase.GKDescriptorNo;

				case DatabaseType.Kau:
				case DatabaseType.Mirror:
					return GKBase.KAUDescriptorNo;
			}
			return 0;
		}

		public abstract void Build();

		public abstract void BuildFormula();
	}
}