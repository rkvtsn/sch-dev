using System;
using System.ComponentModel.DataAnnotations;

namespace Mvc_Schedule.Models.DataModels.ModelViews
{
	
	public class LessonsCreate
	{
		[Display(Name = "Время урока")]
		public LessonsTime[] Lessons { get; set; }
	}

	public class LessonsTime
	{
        [Required(ErrorMessage = "Поле необходимо заполнить.")]
		[Range(0, 59, ErrorMessage = "От {0} до {1}")]
		[Display(Name = "Минуты")]
		public int Minutes { get; set; }

		[Required(ErrorMessage = "Поле необходимо заполнить.")]
		[Range(0, 23, ErrorMessage = "От {0} до {1}")]
		[Display(Name = "Часы")]
		public int Hours { get; set; }

		public DateTime Time { get { return new DateTime(2012, 01, 01, Hours, Minutes, 0); } }

	    public int LessonId { get; set; }
	}

}
