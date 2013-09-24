using Mvc_Schedule.Models;
using System.Web.Mvc;

namespace Mvc_Schedule.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly DomainContext _db = new DomainContext();

        public ScheduleController() { ViewBag.Title = "Редактор расписания"; }

        [HttpGet]
        public ActionResult Excel(int id = -1, int week = 1)
        {
            var group = _db.Groups.Get(id);
            
            if (group == null)
                return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });
            
            var model = _db.Schedule.ToExcel(id, week);

            return View();


            /*var document = ...
    var cd = new System.Net.Mime.ContentDisposition
    {
        // for example foo.bak
        FileName = document.FileName, 

        // always prompt the user for downloading, set to true if you want 
        // the browser to try to show the file inline
        Inline = false, 
    };
    Response.AppendHeader("Content-Disposition", cd.ToString());
    return File(document.Data, document.ContentType);
             * 
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=Sample3.xlsx");
*/
        }


        [HttpPost]
        public JsonResult GetList(string letter, string method)
        {
            if (method == "Lectors") { return Json(_db.Schedule.ListLectors(letter)); }
            else if (method == "Auditory") { return Json(_db.Schedule.ListAuditory(letter)); }
            else return Json(_db.Schedule.ListSubjects(letter));
        }

        [HttpPost]
        public JsonResult GetAvailableLectors(int timeId, string value, bool week)
        {
            return Json(_db.Schedule.IsAvailableLector(timeId, value, week));
        }

        [HttpPost]
        public JsonResult GetAvailableAuditory(int timeId, string value, bool week)
        {
            return Json(_db.Schedule.IsAvailableAuditory(timeId, value, week));
        }

        [HttpGet]
        public ActionResult Index(int id = -1)
        {
            var group = _db.Groups.Get(id);
            if (group == null)
                return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });
            ViewBag.Title = group.Name;
            var model = _db.Schedule.GetWeekdaysWithSchedule(id);
            return View(model);
        }

        // Немного поиска
        [HttpGet]
        public ActionResult Search(string keyword, int searchType, int week = 1)
        {
            if (keyword == null || keyword.Trim() == string.Empty)
                return View();
            else
                return View(_db.Schedule.Search(keyword, searchType, 1 == week));
        }


        [Authorize]
        [HttpGet]
        public ActionResult Create(int id = -1, int week = 1)
        {
            var model = _db.Schedule.ListForCreate(id, 1 == week);
            if (model == null)
                return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection scheduleRows)
        {
            bool isValid;
            var scheduletable = _db.Schedule.FormToTable(scheduleRows, out isValid);

            if (isValid)
            {
                _db.Schedule.ListAdd(scheduletable);
                _db.SaveChanges();
                return RedirectToAction("Index", "Facult");
            }

            ViewBag.Error = "Ошибка ввода, заполните все поля";
            scheduletable.Lessons = _db.Lessons.List();
            scheduletable.Weekdays = _db.Weekdays.List();

            return View(scheduletable);
            //return View(new ScheduleTableCreate());
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}