using System.Web.Mvc;
using Mvc_Schedule.Models;

namespace Mvc_Schedule.Controllers
{
    public class DefaultController : Controller
    {
        public DefaultController()
        {
            ViewBag.Title = "Главная";
        }

        public ActionResult Index()
        {
            using (var db = new DomainContext())
            {
                var model = db.Facults.ListNames(true);
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult GetGroups(string id)
        {
            int facultId;
            if (int.TryParse(id, out facultId))
                using (var db = new DomainContext())
                {
                    var data = db.Ajax.GroupsByFacult(facultId);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ViewResult Error(int id = 101)
        {
            switch (id)
            {
                case 404:
                    ViewBag.Message = "Ошибка 404. Запрашиваемой страницы не существует.";
                    break;
                case 400:
                    ViewBag.Message = "Ошибка 400. Ой, что-то не то...";
                    break;
                case 500:
                    ViewBag.Message = "Ошибка 500. Сервер отдыхает.";
                    break;
                default:
                    id = 101;
                    ViewBag.Message = "Ошибка 101. Непонятно... Попробуйте ещё.";
                    break;
            }
            Response.StatusCode = id;
            return View();
        }
    }
}
