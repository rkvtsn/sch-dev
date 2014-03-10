using System.Globalization;
using System.Web.Security;
using Mvc_Schedule.Models;
using System.Web.Mvc;
using Mvc_Schedule.Models.DataModels;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly DomainContext _db = new DomainContext();
        
        public ScheduleController() { ViewBag.Title = "Редактор расписания"; }



        [HttpGet]
        public ActionResult IndexDev(int id = -1)
        {
            var group = _db.Groups.Get(id);
            if (group == null)
                return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });

            ViewBag.Title = group.Name;
            ViewBag.GroupId = group.GroupId;
            ViewBag.IsAvailable = (Roles.IsUserInRole(group.FacultId.ToString(CultureInfo.InvariantCulture)) ||
                                   Roles.IsUserInRole(StaticData.AdminRole));

            return View();
        }



        #region @main

        [HttpGet]
        public JsonResult List(int id)
        {
            return Json(_db.Ajax.SchList(id), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult Get(int id)
        {
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public JsonResult Add(FormCollection form)
        {
            return Json(null);
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public JsonResult Edit(FormCollection form)
        {
            return Json(null);
        }
        
        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public JsonResult Drop(int id)
        {
            return Json(null);
        }

        #endregion









        #region @feat.

        [HttpPost]
        public JsonResult GetList(string letter, string method)
        {
            if (method == "Lectors") { return Json(_db.Ajax.ListLectors(letter)); }
            else if (method == "Auditory") { return Json(_db.Ajax.ListAuditory(letter)); }
            else return Json(_db.Ajax.ListSubjects(letter));
        }

        [HttpPost]
        public JsonResult GetAvailableLectors(int timeId, string value, bool week)
        {
            return Json(_db.Ajax.IsAvailableLector(timeId, value, week));
        }

        [HttpPost]
        public JsonResult GetAvailableAuditory(int timeId, string value, bool week)
        {
            return Json(_db.Ajax.IsAvailableAuditory(timeId, value, week));
        }

        #endregion


        
        [HttpGet]
        //[OutputCache(Duration = 3600, VaryByParam = "id")]
        public ActionResult Index(int id = -1)
        {
            var group = _db.Groups.Get(id);
            if (group == null)
                return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });
            ViewBag.Title = group.Name;
            var model = _db.Schedule.GetWeekdaysWithSchedule(id);
            return View(model);
        }




        [Authorize, HttpGet]
        public ActionResult Create(int id = -1, int week = 1)
        {
            var group = _db.IsAccessableFor(id);

            var model = _db.Schedule.ListForCreate(group, 1 == week);
            if (model == null)
                return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });

            return View(model);
        }


        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection scheduleRows)
        {
            bool isValid;
            var scheduletable = _db.Schedule.FormToTable(scheduleRows, out isValid);

            if (isValid)
            {
                var group = _db.IsAccessableFor(scheduletable.GroupId);

                if (_db.Schedule.ListAdd(scheduletable, group))
                {
                    _db.SaveChanges();
                    return RedirectToAction("Create", new { id = group.GroupId, week = scheduletable.IsWeekOdd ? 1 : 0 });
                }
            }

            ViewBag.Error = "Ошибка ввода (все поля должны быть заполнены)";
            scheduletable.Lessons = _db.Lessons.List();
            scheduletable.Weekdays = _db.Weekdays.List();

            return View(scheduletable);
        }




        [HttpGet]
        public ActionResult Excel(int id = -1, int week = 1)
        {
            var facult = _db.Facults.Get(id);
            if (facult == null) return RedirectToAction("Error", "Default", new { id = 404 });
            var result = facult.IsReady ? ExcelTemplate.Path(id, week) : _db.Schedule.CheckExcel(id, week);
            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Search(string keyword)
        {
            if (keyword == null || keyword.Trim() == string.Empty)
                return View();
            return View("Index", model: _db.Schedule.Search(keyword));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}