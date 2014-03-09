using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mvc_Schedule.Models.DataModels.Entities
{
    [Serializable]
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required, StringLength(255, MinimumLength = 2)]
        public string Title { get; set; }

        // Plans Update
        public virtual ICollection<Plan> Plans { get; set; }
    }
}