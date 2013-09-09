using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mvc_Schedule.Models.DataModels.Entities
{
	public class Weekday
	{
		[Key]
		public int WeekdayId { get; set; }

		[Display(Name = "День недели")]
		public string Name { get; set; }
		
		public virtual ICollection<ScheduleTable> ScheduleTables { get; set; }
	}
}