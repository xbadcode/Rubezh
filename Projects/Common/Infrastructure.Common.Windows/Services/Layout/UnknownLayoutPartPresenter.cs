﻿using System;

namespace Infrastructure.Common.Services.Layout
{
	public class UnknownLayoutPartPresenter : LayoutPartPresenter
	{
		public UnknownLayoutPartPresenter(Guid uid)
			: base(uid, "Неизвестный элемент", "Close.png", (p) => null)
		{
		}
	}
}