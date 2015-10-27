﻿using ResursAPI.License;
using RubezhLicense;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Resurs.LicenseEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length >= 3)
            {
                var key = InitialKey.FromHexString(args[1]);
                if (key.BinaryValue == null)
                    return;

				int devicesCount;
				if (!int.TryParse(args[2], out devicesCount))
                    return;

			    var licenseInfo = new ResursLicenseInfo { DevicesCount = devicesCount };
				LicenseManager.TrySave(args[0], licenseInfo, key);
                return;
            }

            if (args.Length == 1 && args[0].ToLower().Replace("/", "-").Replace(" ", "") == "-gui")
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }
    }
}
