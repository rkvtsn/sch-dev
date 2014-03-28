using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mvc_Schedule.Models.DataModels.Entities
{
    public class Lector
    {
        [Key]
        public int LectorId { get; set; }

        [Required, StringLength(20)]
        public string Name { get; set; }

        [Required, StringLength(20)]
        public string SecondName { get; set; }

        [StringLength(20)]
        public string ThirdName { get; set; }

        [Required, StringLength(20)]
        public string PreName { get; set; }


        [NotMapped]
        public string FullName
        {
            get
            {
                return PreName + " " + SecondName + " " + Name[0] + "."
                    + ((ThirdName != null && ThirdName.Trim() != String.Empty) ?
                    ThirdName[0] + "." : "");
            }
        }
    }
}