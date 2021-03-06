﻿using System;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Plans.Events
{
	public class NavigateToPlanElementEventArgs
	{
		public Guid PlanUID { get; private set; }
		public Guid ElementUID { get; private set; }

		public NavigateToPlanElementEventArgs(Guid planUID, Guid elementUID)
		{
			PlanUID = planUID;
			ElementUID = elementUID;
		}
	}
	public class NavigateToPlanElementEvent : CompositePresentationEvent<NavigateToPlanElementEventArgs>
	{
	}
}