﻿using System.Configuration;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using FiresecClient;
using ItvIntergation.Ngi;
using System;
using FiresecAPI.Models;

namespace RepFileManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var FS_Address = ConfigurationManager.AppSettings["FS_Address"] as string;
            var FS_Port = Convert.ToInt32(ConfigurationManager.AppSettings["FS_Port"] as string);
            var FS_Login = ConfigurationManager.AppSettings["FS_Login"] as string;
            var FS_Password = ConfigurationManager.AppSettings["FS_Password"] as string;
            var serverAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            var Login = ConfigurationManager.AppSettings["Login"] as string;
            var Password = ConfigurationManager.AppSettings["Password"] as string;

            var message = ItvManager.Connect(serverAddress, Login, Password, FS_Address, FS_Port, FS_Login, FS_Password);
            if (message != null)
            {
                MessageBox.Show(message);
                return;
            }

            Directory.CreateDirectory("BMP");

            var repositoryModule = new repositoryModule();
            repositoryModule.name = "Rubezh devices";
            repositoryModule.version = "1.0.0";
            repositoryModule.port = "1234";
            var repository = new repository();
            repository.module = repositoryModule;

            repositoryModule.device = DevicesGenerator.Generate().ToArray();

            var serializer = new XmlSerializer(typeof(repositoryModule));
            using (var fileStream = File.CreateText("Rubezh.rep"))
            {
                serializer.Serialize(fileStream, repositoryModule);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        }
    }
}