﻿using System;
using Microsoft.Practices.Prism.Events;

namespace PlansModule.Events
{
    public class PlanStateChangedEvent : CompositePresentationEvent<Guid>
    {
    }
}