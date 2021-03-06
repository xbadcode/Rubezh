﻿using System;
using System.IO;
using System.Windows;
using Common;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Ionic.Zip;
using Microsoft.Win32;
using System.Xml.Linq;

namespace FireAdministrator
{
	public static class FileConfigurationHelper
	{
		public static string SaveToFile()
		{
			try
			{
				var saveDialog = new SaveFileDialog()
				{
					Filter = "rubezh2 files|*.fscp",
					DefaultExt = "rubezh2 files|*.fscp"
				};
				if (saveDialog.InitialDirectory != null)
				{
					var fileName = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
					fileName = fileName.Replace(":", " ");
					saveDialog.FileName = Path.Combine(saveDialog.InitialDirectory, fileName + ".fscp");
				}

				if (saveDialog.ShowDialog().Value)
				{
					WaitHelper.Execute(() =>
					{
						SaveToZipFile(saveDialog.FileName);
					});
					return saveDialog.FileName;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.SaveToFile");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
			return null;
		}

		public static void SaveToZipFile(string fileName)
		{
			ClientManager.InvalidateContent();

			var folderName = AppDataFolderHelper.GetTempFolder();
			if (!Directory.Exists(folderName))
				Directory.CreateDirectory(folderName);

			if (File.Exists(fileName))
				File.Delete(fileName);

			TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

			AddConfiguration(folderName, "PlansConfiguration.xml", ClientManager.PlansConfiguration);
			AddConfiguration(folderName, "SystemConfiguration.xml", ClientManager.SystemConfiguration);
			AddConfiguration(folderName, "GKDeviceConfiguration.xml", GKManager.DeviceConfiguration);
			AddConfiguration(folderName, "LayoutsConfiguration.xml", ClientManager.LayoutsConfiguration);
			AddConfiguration(folderName, "ZipConfigurationItemsCollection.xml", TempZipConfigurationItemsCollection);

			var destinationImagesDirectory = AppDataFolderHelper.GetFolder(Path.Combine(folderName, "Content"));
			if (Directory.Exists(ServiceFactory.ContentService.ContentFolder))
			{
				if (Directory.Exists(destinationImagesDirectory))
					Directory.Delete(destinationImagesDirectory);
				if (!Directory.Exists(destinationImagesDirectory))
					Directory.CreateDirectory(destinationImagesDirectory);
				var sourceImagesDirectoryInfo = new DirectoryInfo(ServiceFactory.ContentService.ContentFolder);
				foreach (var fileInfo in sourceImagesDirectoryInfo.GetFiles())
				{
					fileInfo.CopyTo(Path.Combine(destinationImagesDirectory, fileInfo.Name));
				}
			}

			var zipFile = new ZipFile(fileName);
			zipFile.AddDirectory(folderName);
			zipFile.Save(fileName);
			zipFile.Dispose();
			if (Directory.Exists(folderName))
				Directory.Delete(folderName, true);
		}

		static ZipConfigurationItemsCollection TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

		static void AddConfiguration(string folderName, string name, VersionedConfiguration configuration, int minorVersion = 1, int majorVersion = 1)
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MinorVersion = minorVersion, MajorVersion = majorVersion };
			ZipSerializeHelper.Serialize(configuration, Path.Combine(folderName, name));

			TempZipConfigurationItemsCollection.ZipConfigurationItems.Add(new ZipConfigurationItem(name, minorVersion, majorVersion));
		}

		public static string LoadFromFile()
		{
			try
			{
				var openDialog = new OpenFileDialog
					{
						Filter = "rubezh2 files|*.fscp",
						DefaultExt = "rubezh2 files|*.fscp"
					};
				if (openDialog.ShowDialog().Value)
				{
					LoadFromFile(openDialog.FileName);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.LoadFromFile");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
			return null;
		}

		public static string LoadFromFile(string fileName)
		{
			try
			{
				WaitHelper.Execute(() =>
				{
					ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Publish(null);
					var folderName = AppDataFolderHelper.GetLocalFolder("Administrator/Configuration");
					var configFileName = Path.Combine(folderName, "Config.fscp");
					if (Directory.Exists(folderName))
						Directory.Delete(folderName, true);
					Directory.CreateDirectory(folderName);
					File.Copy(fileName, configFileName);
					ClientManager.LoadFromZipFile(configFileName);
					ServiceFactory.ContentService.Invalidate();

					ClientManager.UpdateConfiguration();

					if (LoadingErrorManager.HasError)
						MessageBoxService.ShowWarning(LoadingErrorManager.ToString(), "Ошибки при загрузке конфигурации");

					ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
					ConfigManager.ShowFirstDevice();

					ServiceFactory.SaveService.Set();
				});
				return fileName;
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.LoadFromFile");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
			return null;
		}
	}
}