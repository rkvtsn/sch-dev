using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mvc_Schedule.Models.DataModels.Entities
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }

        [Required(ErrorMessage = "���� ���������� ���������")]
        [Display(Name = "����� ������")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public DateTime Time { get; set; }

        public String TimeString
        {
            get { return String.Format("{0:HH:mm}", Time); }
        }

        public virtual ICollection<ScheduleTable> ScheduleTable { get; set; }
    }
}