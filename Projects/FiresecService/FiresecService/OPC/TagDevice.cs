﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecService.OPC
{
	public class TagDevice
	{
		public int TagId { get; set; }
		public DeviceState DeviceState { get; set; }
	}
}