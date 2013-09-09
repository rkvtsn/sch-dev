using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.DataModels.ModelViews
{
	public class ScheduleTableCreate
	{
		public string GroupName { get; set; }

		public int GroupId { get; set; }

		[Display(Name = "Нечётная неделя")]
		public bool IsWeekOdd { get; set; }

		private IList<ScheduleTable> _scheduleTableRows;
		public IList<ScheduleTable> ScheduleTableRows
		{
			get { return _scheduleTableRows; }
			set 
			{
				_scheduleTableRows = value;
				if (_scheduleTableRows != null)
					foreach (var scheduleTable in value)
						scheduleTable.IsWeekOdd = IsWeekOdd;
			}
		}

		public IList<Lesson> Lessons { get; set; }

		public IList<Weekday> Weekdays { get; set; }
	}
}
