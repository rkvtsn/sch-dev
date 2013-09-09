using System.Collections.Generic;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.DataModels.ModelViews
{
    public class ScheduleTableSearch
    {
        public string Keyword { get; set; }

        public int SearchType { get; set; }

        public IEnumerable<Weekday> Weekdays { get; set; }

        public bool IsWeekOdd { get; set; }

        public IEnumerable<ScheduleTable> Schedule { get; set; }

        public IEnumerable<Lesson> Lessons { get; set; }
    }
}
