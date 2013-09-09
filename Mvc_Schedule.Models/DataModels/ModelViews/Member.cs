using System;
using System.Collections.Generic;

namespace Mvc_Schedule.Models.DataModels.ModelViews
{
	[Serializable]
	public class RoleCheck
	{
		public string RoleCheckId { get; set; }

		public string Name { get; set; }

		public bool IsChecked { get; set; }
	}

	[Serializable]
	public class Member
	{
		public string UserName { get; set; }

		public List<RoleCheck> RoleChecks { get; set; }
	}
}
