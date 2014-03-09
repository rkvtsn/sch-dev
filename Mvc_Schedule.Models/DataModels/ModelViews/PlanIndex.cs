using System.Collections.Generic;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.DataModels.ModelViews
{
    public class PlanIndex
    {
        public List<Plan> Plans { get; set; }
        public StudGroup StudGroup { get; set; }
    }
}
