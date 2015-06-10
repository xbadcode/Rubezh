﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Journal;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel : BaseViewModel
	{
		ushort OnDelay { get; set; }
		ushort HoldDelay { get; set; }
		ushort OffDelay { get; set; }
		DelayRegime? DelayRegime { get; set; }

		void InitializeDelays()
		{
			var device = GKBase as GKDevice;
			if (device != null)
			{
				var property = device.Properties.FirstOrDefault(x => x.Name == "Задержка на включение, с");
				if (property != null)
				{
					OnDelay = property.Value;
				}
				property = device.Properties.FirstOrDefault(x => x.Name == "Время удержания, с");
				if (property != null)
				{
					HoldDelay = property.Value;
				}
				property = device.Properties.FirstOrDefault(x => x.Name == "Задержка на выключение, с");
				if (property != null)
				{
					OffDelay = property.Value;
				}
			}
			var direction = GKBase as GKDirection;
			if (direction != null)
			{
				OnDelay = direction.Delay;
				HoldDelay = direction.Hold;
				DelayRegime = direction.DelayRegime;
			}
			var delay = GKBase as GKDelay;
			if (delay != null)
			{
				OnDelay = delay.DelayTime;
				HoldDelay = delay.Hold;
				DelayRegime = delay.DelayRegime;
			}
		}

		TurningState TurningState = TurningState.None;

		ushort _currentOnDelay;
		public ushort CurrentOnDelay
		{
			get { return _currentOnDelay; }
			set
			{
				_currentOnDelay = value;
				OnPropertyChanged(() => CurrentOnDelay);
			}
		}

		ushort _currentHoldDelay;
		public ushort CurrentHoldDelay
		{
			get { return _currentHoldDelay; }
			set
			{
				_currentHoldDelay = value;
				OnPropertyChanged(() => CurrentHoldDelay);
			}
		}

		ushort _currentOffDelay;
		public ushort CurrentOffDelay
		{
			get { return _currentOffDelay; }
			set
			{
				_currentOffDelay = value;
				OnPropertyChanged(() => CurrentOffDelay);
			}
		}

		public void InitializeTurning()
		{
			TurnOnCommand = new RelayCommand(OnTurnOn);
			TurnOnNowCommand = new RelayCommand(OnTurnOnNow);
			TurnOffCommand = new RelayCommand(OnTurnOff);
			TurnOffNowCommand = new RelayCommand(OnTurnOffNow);

			var device = GKBase as GKDevice;
			if (device != null)
			{
				switch (device.DriverType)
				{
					case GKDriverType.RSR2_RM_1:
					case GKDriverType.RSR2_MDU:
					case GKDriverType.RSR2_MDU24:
					case GKDriverType.RSR2_MVK8:
					case GKDriverType.RSR2_Bush_Drenazh:
					case GKDriverType.RSR2_Bush_Jokey:
					case GKDriverType.RSR2_Bush_Fire:
					case GKDriverType.RSR2_Bush_Shuv:
					case GKDriverType.RSR2_Valve_KV:
					case GKDriverType.RSR2_Valve_KVMV:
					case GKDriverType.RSR2_Valve_DU:
					case GKDriverType.RSR2_OPK:
					case GKDriverType.RSR2_OPS:
					case GKDriverType.RSR2_OPZ:
					case GKDriverType.RSR2_Buz_KV:
					case GKDriverType.RSR2_Buz_KVMV:
					case GKDriverType.RSR2_Buz_KVDU:
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						break;
				}
			}

			var direction = GKBase as GKDirection;
			if (direction != null)
			{
				HasTurnOn = true;
				HasTurnOnNow = true;
				HasTurnOff = true;
				StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
			}
			var delay = GKBase as GKDelay;
			if (delay != null)
			{
				HasTurnOn = true;
				HasTurnOnNow = true;
				HasTurnOff = true;
				StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
			}
		}

		public bool HasTurnOn { get; private set; }
		public bool HasTurnOnNow { get; private set; }
		public bool HasTurnOff { get; private set; }
		public bool HasTurnOffNow { get; private set; }

		public void CheckDelays()
		{
			if (TurningState == TurningState.TurningOn)
			{
				CurrentOnDelay--;
				AdditionalShortParameters[0] = CurrentOnDelay;
				if (CurrentOnDelay == 0)
				{
					TurningState = TurningState.None;
					StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = true;
					StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = false;
					StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = false;
					StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = false;
					var journalItem = new ImitatorJournalItem(2, 9, 2, 0);
					AddJournalItem(journalItem);

					if (HoldDelay > 0)
					{
						CurrentHoldDelay = HoldDelay;
						TurningState = TurningState.Holding;
					}
				}
			}
			if (TurningState == TurningState.Holding)
			{
				CurrentHoldDelay--;
				AdditionalShortParameters[1] = CurrentHoldDelay;
				if (CurrentHoldDelay == 0)
				{
					TurningState = TurningState.None;
					if (DelayRegime != null)
					{
						if (DelayRegime.Value == FiresecAPI.GK.DelayRegime.Off)
						{
							TurnOffNow();
						}
					}
					else
					{
						if (OffDelay > 0)
						{
							CurrentOffDelay = OffDelay;
							TurningState = TurningState.TurningOff;
							TurnOff();
						}
					}
				}
			}
			if (TurningState == TurningState.TurningOff)
			{
				CurrentOffDelay--;
				AdditionalShortParameters[2] = CurrentOffDelay;
				if (CurrentOffDelay == 0)
				{
					TurningState = TurningState.None;
					TurnOffNow();
				}
			}
		}

		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			if (OnDelay > 0)
			{
				CurrentOnDelay = OnDelay;
				TurningState = TurningState.TurningOn;
			}
			TurnOn();
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			TurnOnNow();
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			if (OffDelay > 0)
			{
				CurrentOffDelay = OffDelay;
				TurningState = TurningState.None;
			}
			TurnOff();
		}

		public RelayCommand TurnOffNowCommand { get; private set; }
		void OnTurnOffNow()
		{
			TurnOffNow();
		}

		void TurnOn()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = true;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = false;
			var journalItem = new ImitatorJournalItem(2, 9, 4, 0);
			AddJournalItem(journalItem);
		}

		void TurnOnNow()
		{
			CurrentOnDelay = 0;
			CurrentHoldDelay = 0;
			CurrentOffDelay = 0;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = true;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = false;
			var journalItem = new ImitatorJournalItem(2, 9, 3, 3);
			AddJournalItem(journalItem);
		}

		void TurnOff()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = true;
			var journalItem = new ImitatorJournalItem(2, 9, 5, 3);
			AddJournalItem(journalItem);
		}

		void TurnOffNow()
		{
			CurrentOnDelay = 0;
			CurrentHoldDelay = 0;
			CurrentOffDelay = 0;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = true;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = false;
			var journalItem = new ImitatorJournalItem(2, 9, 3, 3);
			AddJournalItem(journalItem);
		}
	}
}