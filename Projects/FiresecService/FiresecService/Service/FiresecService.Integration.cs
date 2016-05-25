﻿using System.Collections.Generic;
using StrazhAPI;
using StrazhAPI.Integration.OPC;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public OperationResult<bool> PingOPCServer()
		{
			var result = _integrationService.PingOPCServer();

			return new OperationResult<bool>(result);
		}

		public OperationResult<List<OPCZone>> GetOPCZones()
		{
			var result = _integrationService.GetOPCZones();

			return new OperationResult<List<OPCZone>>(result);
		}
	}
}
