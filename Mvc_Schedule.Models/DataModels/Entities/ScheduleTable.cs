using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mvc_Schedule.Models.DataModels.Entities
{
	public class ScheduleTable
	{
		[Key]
		public int ScheduleTableId { get; set; }

		[Display(Name = "Аудитория")]
		[MaxLength(32)]
		[Required(ErrorMessage = "Поле необходимо заполнить")]
		public string Auditory { get; set; }

		[Display(Name = "Преподаватель")]
		[MaxLength(127)]
		public string LectorName { get; set; }

		//Нечетная неделя
		[Display(Name = "Нечётная неделя")]
		public bool IsWeekOdd { get; set; }

		[Display(Name = "Дисциплина")]
		[Required(ErrorMessage = "Поле необходимо заполнить")]
		[MaxLength(127, ErrorMessage = "Допустимо {0} символов в названии")]
		public string SubjectName { get; set; }

		[Display(Name = "Звонок")]
		public int LessonId { get; set; }

		[Display(Name = "День недели")]
		public int WeekdayId { get; set; }

		[Display(Name = "Группа")]
		public int GroupId { get; set; }

		[ForeignKey("WeekdayId")]
		public virtual Weekday Weekday { get; set; }
		[ForeignKey("GroupId")]
		public virtual StudGroup StudGroup { get; set; }
		[ForeignKey("LessonId")]
		public virtual Lesson Lesson { get; set; }
	}
}