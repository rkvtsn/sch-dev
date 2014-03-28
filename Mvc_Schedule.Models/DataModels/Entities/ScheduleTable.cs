using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mvc_Schedule.Models.DataModels.Entities
{
    public class ScheduleTable
	{
		[Key]
		public int ScheduleTableId { get; set; }

		[Display(Name = "���������")]
		[MaxLength(32)]
		[Required(ErrorMessage = "���� ���������� ���������")]
		public string Auditory { get; set; }

		[Display(Name = "�������������")]
		[MaxLength(127)]
		public string LectorName { get; set; }

		[Display(Name = "�������� ������")]
		public bool IsWeekOdd { get; set; }

		[Display(Name = "����������")]
		[Required(ErrorMessage = "���� ���������� ���������")]
		[MaxLength(127, ErrorMessage = "��������� {0} �������� � ��������")]
		public string SubjectName { get; set; }

		[Display(Name = "������")]
		public int LessonId { get; set; }

		[Display(Name = "���� ������")]
		public int WeekdayId { get; set; }

		
        [Required]
        public int LessonType { get; set; }


        [Display(Name = "������")]
        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual StudGroup StudGroup { get; set; }
        
        [ForeignKey("WeekdayId")]
		public virtual Weekday Weekday { get; set; }
		
        [ForeignKey("LessonId")]
		public virtual Lesson Lesson { get; set; }

        // Plans Update
        public DateTime Date { get; set; }

	    public ScheduleTable()
	    {
	        Date = DateTime.Now;
	    }
	}

}