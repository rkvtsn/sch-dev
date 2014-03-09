using System;
using System.ComponentModel.DataAnnotations;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.DataModels.ModelViews
{
    [Serializable]
    public class PlanCreate
    {
        [Required]
        public string SubjectName { get; set; }

        public Plan Plan { get; set; }
    }
}
