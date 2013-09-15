using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

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

        public string FullName { get { return GetFullName(); } }

        public string GetFullName()
        {
            var sb = new StringBuilder(SecondName).Append(" ").Append(Name[0]).Append(".");
            if (ThirdName != null && ThirdName.Trim() != String.Empty)
                sb.Append(ThirdName[0]).Append(".");
            return sb.ToString(); // _fullName = sb.ToString(); // return _fullName;
        }
    }
}