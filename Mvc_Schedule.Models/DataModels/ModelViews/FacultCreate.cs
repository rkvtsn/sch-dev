using System.ComponentModel.DataAnnotations;
using System.Web;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.DataModels.ModelViews
{
    public class FacultCreate
    {
        public Facult FacultInstance { get; set; }

        private StudGroup[] _studGroupsNames;
        public StudGroup[] StudGroupsNames
        {
            get { return _studGroupsNames; }
            set
            {
                _studGroupsNames = value;
                if (_studGroupsNames != null)
                    foreach (var group in _studGroupsNames)
                        group.Facult = this.FacultInstance;
            }
        }

        [Required(ErrorMessage = "Поле необходимо заполнить")]
        [FileSize(10240)]
        [FileTypes("xlsx")]
        public HttpPostedFileBase Template { get; set; }
    }
}
