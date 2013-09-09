using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mvc_Schedule.Models.DataModels.Entities
{
	public class StudGroup
	{
		[Key]
		public int GroupId { get; set; }

		[Required(ErrorMessage = "Поле необходимо заполнить")]
		[MaxLength(256)]
		[Display(Name = "Название группы")]
		public string Name { get; set; }
		
		public int FacultId { get; set; }

		[ForeignKey("FacultId")]
		public virtual Facult Facult { get; set; }

		public virtual ICollection<ScheduleTable> ScheduleTable { get; set; }
	}
}