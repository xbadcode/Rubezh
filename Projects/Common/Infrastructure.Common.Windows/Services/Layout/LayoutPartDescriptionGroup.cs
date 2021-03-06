﻿using System.ComponentModel;

namespace Infrastructure.Common.Services.Layout
{
	public enum LayoutPartDescriptionGroup
	{
		Root,

		[Description("Общие")]
		Common,

		[Description("ГК")]
		GK,

		[Description("СКД")]
		SKD,

		[Description("Видео")]
		Video,

		[Description("Элементы управления")]
		Control,
	}
}
