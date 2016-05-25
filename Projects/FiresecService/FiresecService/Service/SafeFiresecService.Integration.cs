﻿using System.Collections.Generic;
using Common;
using StrazhAPI;
using StrazhAPI.Integration.OPC;

namespace FiresecService.Service
{
	public partial class SafeFiresecService
	{
		public OperationResult<bool> PingOPCServer()
		{
			return SafeContext.Execute(() => FiresecService.PingOPCServer());
		}

		public OperationResult<List<OPCZone>> GetOPCZones()
		{
			return SafeContext.Execute(() => FiresecService.GetOPCZones());
		}
	}
}
