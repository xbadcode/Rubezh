﻿using System;
using Infrastructure.Common.Windows.Navigation;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class ShowGKSKDZoneEvent : CompositePresentationEvent<ShowOnPlanArgs<Guid>>
	{
	}
}