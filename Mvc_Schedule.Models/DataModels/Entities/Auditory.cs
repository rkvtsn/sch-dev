using System.ComponentModel.DataAnnotations;

namespace Mvc_Schedule.Models.DataModels.Entities
{
    public class Auditory
    {
        
        [Key]
        public int AuditoryId { get; set; }

        [Required, StringLength(255, MinimumLength = 1)]
        public string Number { get; set; }

    }
}