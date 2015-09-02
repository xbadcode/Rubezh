﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FiresecAPI
{
	public enum ServerState
	{
		[Description("Запуск")]
		Starting,

		[Description("Перезапуск")]
		Restarting,

		[Description("Готово")]
		Ready
	}
}