﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;


namespace FiresecService.Presenters
{
	public abstract class ApplicationPresenter
	{
		public ApplicationPresenter()
		{
			FormDispatcher = Dispatcher.CurrentDispatcher;
		}

		public static MainPresenter Current { get; set; }

		public Dispatcher FormDispatcher { get; private set; }
	}
}