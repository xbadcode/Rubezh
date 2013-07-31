﻿using System;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using System.Windows;

namespace ServerFS2.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		public static MainViewModel Current { get; private set; }

		public MainViewModel()
		{
			Current = this;
			Title = "Сервер ОПС FS2";
			ExitCommand = new RelayCommand(OnExit);
		}

		private string _status;
		public string Satus
		{
			get { return _status; }
			set
			{
				_status = value;
				OnPropertyChanged("Status");
			}
		}

		public void AddLog(string message)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				LastLog = message;
				InfoLog += message + "\n";
			}
			));
		}

		string _lastLog = "";
		public string LastLog
		{
			get { return _lastLog; }
			set
			{
				_lastLog = value;
				OnPropertyChanged("LastLog");
			}
		}

		string _infoLog = "";
		public string InfoLog
		{
			get { return _infoLog; }
			set
			{
				_infoLog = value;
				OnPropertyChanged("InfoLog");
			}
		}

		public RelayCommand ExitCommand { get; private set; }
		void OnExit()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите остановить драйвер ОПС Firesec-2?") == MessageBoxResult.Yes)
			{
				this.Close();
				NotifyIconService.Stop();
				Bootstrapper.Close();
			}	
		}

		public override bool OnClosing(bool isCanceled)
		{
			ApplicationMinimizeCommand.ForceExecute();
			return true;
		}
	}
}