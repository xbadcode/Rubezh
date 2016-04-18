﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Extensions
{
	public static class GuidExtensions
	{
		public static bool IsNullOrEmpty(this Guid? id)
		{
			return id == null || id == Guid.Empty;
		}
	}
}
