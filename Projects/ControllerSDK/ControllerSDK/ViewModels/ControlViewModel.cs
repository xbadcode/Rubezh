﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using ControllerSDK.SDK;
using ControllerSDK.Views;
using System.Windows;

namespace ControllerSDK.ViewModels
{
	public class ControlViewModel : BaseViewModel
	{
		public ControlViewModel()
		{
			OpenDoorCommand = new RelayCommand(OnOpenDoor);
			CloseDoorCommand = new RelayCommand(OnCloseDoor);
			GetDoorStatusCommand = new RelayCommand(OnGetDoorStatus);
		}

		public RelayCommand OpenDoorCommand { get; private set; }
		void OnOpenDoor()
		{
			var result = SDKImport.WRAP_DevCtrl_OpenDoor(MainWindow.LoginID);
			MessageBox.Show(result.ToString());
		}

		public RelayCommand CloseDoorCommand { get; private set; }
		void OnCloseDoor()
		{
			var result = SDKImport.WRAP_DevCtrl_CloseDoor(MainWindow.LoginID);
			MessageBox.Show(result.ToString());
		}

		public RelayCommand GetDoorStatusCommand { get; private set; }
		void OnGetDoorStatus()
		{
			var result = SDKImport.WRAP_DevState_DoorStatus(MainWindow.LoginID);
			switch(result)
			{
				case -1:
					MessageBox.Show("Error");
					break;

				case 0:
					MessageBox.Show("Unknown");
					break;

				case 1:
					MessageBox.Show("Opened");
					break;

				case 2:
					MessageBox.Show("Closed");
					break;
			}
		}
	}
}