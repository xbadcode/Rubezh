﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhService.Monitor.ViewModels
{
	/// <summary>
	/// Описывает настройки параметров СУБД
	/// </summary>
	public class DatabaseSettingsViewModel : BaseViewModel
	{
		private string _dbServerName;
		private string _dbServerAddress;
		private int _dbServerPort;
		private SqlServerAuthenticationMode _sqlServerAuthenticationMode;
		private string _dbUserID;
		private string _dbUserPwd;

		public string DBServerName
		{
			get { return _dbServerName; }
			set
			{
				if (_dbServerName == value)
					return;
				_dbServerName = value;
				OnPropertyChanged(() => DBServerName);
			}
		}

		/// <summary>
		/// IP-адрес сервера
		/// </summary>
		public string DBServerAddress
		{
			get { return _dbServerAddress; }
			set
			{
				if (_dbServerAddress == value)
					return;
				_dbServerAddress = value;
				OnPropertyChanged(() => DBServerAddress);
			}
		}

		/// <summary>
		/// Порт сервера
		/// </summary>
		public int DBServerPort
		{
			get { return _dbServerPort; }
			set
			{
				if (_dbServerPort == value)
					return;
				_dbServerPort = value;
				OnPropertyChanged(() => DBServerPort);
			}
		}

		/// <summary>
		/// Метод аутентификации
		/// </summary>
		public SqlServerAuthenticationMode SqlServerAuthenticationMode
		{
			get { return _sqlServerAuthenticationMode; }
			set
			{
				if (_sqlServerAuthenticationMode == value)
					return;
				_sqlServerAuthenticationMode = value;
				OnPropertyChanged(() => SqlServerAuthenticationMode);
				OnPropertyChanged(() => UseSqlServerAuthentication);
			}
		}

		public ObservableCollection<SqlServerAuthenticationMode> AvailableSqlServerAuthenticationModes { get; private set; }

		public bool UseSqlServerAuthentication
		{
			get { return SqlServerAuthenticationMode == SqlServerAuthenticationMode.SqlServer; }
		}

		/// <summary>
		/// Логин
		/// </summary>
		public string DBUserID
		{
			get { return _dbUserID; }
			set
			{
				if (_dbUserID == value)
					return;
				_dbUserID = value;
				OnPropertyChanged(() => DBUserID);
			}
		}

		/// <summary>
		/// Пароль
		/// </summary>
		public string DBUserPwd
		{
			get { return _dbUserPwd; }
			set
			{
				if (_dbUserPwd == value)
					return;
				_dbUserPwd = value;
				OnPropertyChanged(() => DBUserPwd);
			}
		}

		public RelayCommand CheckSqlServerConnectionCommand { get; private set; }

		public RelayCommand ApplyCommand { get; private set; }

		public DatabaseSettingsViewModel()
		{
			CheckSqlServerConnectionCommand = new RelayCommand(OnCheckSqlServerConnection);
			ApplyCommand = new RelayCommand(OnApply);
			InitializeAvailableSqlServerAuthenticationModes();
			ReadFromModel();
		}

		private void InitializeAvailableSqlServerAuthenticationModes()
		{
			AvailableSqlServerAuthenticationModes = new ObservableCollection<SqlServerAuthenticationMode>
			{
				SqlServerAuthenticationMode.Windows,
				SqlServerAuthenticationMode.SqlServer
			};
		}
		
		private void OnCheckSqlServerConnection()
		{
			string errors;
			var checkResult = CheckSqlServerConnection(DBServerAddress, DBServerPort,
				DBServerName, SqlServerAuthenticationMode == SqlServerAuthenticationMode.Windows, DBUserID, DBUserPwd, out errors);

			var msg = string.Format("Соединение с сервером {0} {1}", DBServerName, checkResult ? "успешно установлено" : string.Format("установить не удалось по причине ошибки: \n\n{0}", errors));

			if (!checkResult)
				MessageBoxService.ShowWarning(msg);
			else
				MessageBoxService.Show(msg);
		}

		/// <summary>
		/// Проверяет доступность СУБД MS SQL Server
		/// </summary>
		/// <param name="ipAddress">IP-адрес сервера СУБД</param>
		/// <param name="ipPort">IP-порт сервера СУБД</param>
		/// <param name="instanceName">Название именованной установки сервера СУБД</param>
		/// <param name="useIntegratedSecurity">Метод аутентификации</param>
		/// <param name="userID">Логин (только для SQL Server аутентификации)</param>
		/// <param name="userPwd">Пароль (только для SQL Server аутентификации)</param>
		/// <param name="errors">Ошибки, возникшие в процессе проверки соединения</param>
		/// <returns>true - в случае успеха, false - в противном случае</returns>
		private static bool CheckSqlServerConnection(string ipAddress, int ipPort, string instanceName, bool useIntegratedSecurity, string userID, string userPwd, out string errors)
		{
			errors = null;
			var connectionString = BuildConnectionString(ipAddress, ipPort, instanceName, "master", useIntegratedSecurity, userID, userPwd);
			using (var connection = new SqlConnection(connectionString))
			{
				try
				{
					connection.Open();
				}
				catch (Exception e)
				{
					errors = e.Message;
					return false;
				}
			}
			return true;
		}

		private static string BuildConnectionString(string ipAddress, int ipPort, string instanceName, string db, bool useIntegratedSecurity, string userID, string userPwd)
		{
			var csb = new SqlConnectionStringBuilder();
			csb.DataSource = String.Format(@"{0}{1},{2}", ipAddress, String.IsNullOrEmpty(instanceName) ? String.Empty : String.Format(@"\{0}", instanceName), ipPort);
			csb.InitialCatalog = db;
			csb.IntegratedSecurity = useIntegratedSecurity;
			if (!csb.IntegratedSecurity)
			{
				csb.UserID = userID;
				csb.Password = userPwd;
			}
			return csb.ConnectionString;
		}

		private void OnApply()
		{
			WriteToModel();
			MessageBoxService.ShowWarning("Параметры вступят в силу после перезапуска сервера приложений");
		}

		private void ReadFromModel()
		{
			var settings = AppServerSettingsHelper.AppServerSettings;
			DBServerAddress = settings.DBServerAddress;
			DBServerPort = settings.DBServerPort;
			DBServerName = settings.DBServerName;
			SqlServerAuthenticationMode = settings.DBUseIntegratedSecurity
				? SqlServerAuthenticationMode.Windows
				: SqlServerAuthenticationMode.SqlServer;
			DBUserID = settings.DBUserID;
			DBUserPwd = settings.DBUserPwd;
		}

		private void WriteToModel()
		{
			var settings = AppServerSettingsHelper.AppServerSettings;
			settings.DBServerAddress = DBServerAddress;
			settings.DBServerPort = DBServerPort;
			settings.DBServerName = DBServerName;
			settings.DBUseIntegratedSecurity = SqlServerAuthenticationMode == SqlServerAuthenticationMode.Windows;
			settings.DBUserID = DBUserID;
			settings.DBUserPwd = DBUserPwd;
			AppServerSettingsHelper.Save();
		}
	}

	public enum SqlServerAuthenticationMode
	{
		[Description("Windows")]
		Windows,
		[Description("SQL Sever")]
		SqlServer
	}
}