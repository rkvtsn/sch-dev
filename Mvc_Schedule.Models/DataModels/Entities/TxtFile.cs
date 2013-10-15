using System.ComponentModel.DataAnnotations;
using System.Web;
using Mvc_Schedule.Models.DataModels.ModelViews;

namespace Mvc_Schedule.Models.DataModels.Entities
{
    public class TxtFile
    {
        [FileSize(1024)]
        [Required(ErrorMessage = "Необходимо выбрать файл")]
        [FileTypes("txt")]
        public HttpPostedFileBase Txt { get; set; }
    }
}
