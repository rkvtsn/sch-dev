using System.ComponentModel.DataAnnotations;
using System.Web;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.DataModels.ModelViews
{
    public class FacultEdit
    {
        public Facult FacultInstance { get; set; }

        [FileSize(10240)]
        [FileTypes("xlsx")]
        public HttpPostedFileBase Template { get; set; }
    }
}