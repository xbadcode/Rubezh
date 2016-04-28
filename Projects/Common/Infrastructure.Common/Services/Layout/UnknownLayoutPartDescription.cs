﻿using System;

namespace Infrastructure.Common.Services.Layout
{
	public class UnknownLayoutPartDescription : LayoutPartDescription
	{
		public UnknownLayoutPartDescription(Guid uid)
			: base(LayoutPartDescriptionGroup.Root, uid, 1, Resources.Language.Services.Layout.UnknownLayoutPartDescription.Name, null, Resources.Language.Services.Layout.UnknownLayoutPartDescription.IconName)
		{
		}
	}
}