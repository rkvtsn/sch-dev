using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mvc_Schedule.Models.DataModels.Entities
{
	public class Facult
	{
		[Key]
		public int FacultId { get; set; }

		[Required(ErrorMessage = "Поле необходимо заполнить")]
		[MaxLength(128)]
		[Display(Name = "Название факультета")]
		public string Name { get; set; }
		
		public virtual ICollection<StudGroup> StudGroups { get; set; }
	}
}