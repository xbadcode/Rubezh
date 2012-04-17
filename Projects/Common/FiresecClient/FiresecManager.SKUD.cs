﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models.Skud;

namespace FiresecClient
{
    public partial class FiresecManager
    {
		public static IEnumerable<EmployeeCardIndex> GetEmployees()
        {
            return FiresecService.GetEmployees();
        }
		public static ActionResult EmployeeCardDelete(EmployeeCardIndex card)
		{
			return FiresecService.DeleteEmployee(card.Id);
		}

    }
}