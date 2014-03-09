using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Mvc_Schedule.Models.DataModels.Entities
{
    
    [Serializable]
    public class Plan
    {
        [Key]
        public int PlanId { get; set; }

        [Required]
        virtual public int LectionH { get; set; }
        [Required]
        virtual public int PracticeH { get; set; }
        [Required]
        virtual public int LabH { get; set; }

        virtual public int GroupId { get; set; }
        virtual public int SubjectId { get; set; }

        [ForeignKey("GroupId")]
        public virtual StudGroup StudGroup { get; set; }
        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }

    }
    
    
}
