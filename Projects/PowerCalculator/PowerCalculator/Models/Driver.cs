﻿namespace PowerCalculator.Models
{
	public class Driver
	{
		public Driver()
		{
			CanAdd = true;
		}

		public DriverType DriverType { get; set; }
		public bool CanAdd { get; set; }
		public uint Mult { get; set; }
		public double R { get; set; }
        public double I { get; set; }
        public double U { get; set; }
		public DeviceType DeviceType { get; set; }
        public double Umin { get; set; }
        public double Imax { get; set; }

		public string ImageSource
		{
			get { return "/Controls;component/GKIcons/" + DriverType + ".png"; }
		}
	}
}