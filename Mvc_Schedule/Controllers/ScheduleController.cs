using System;
using System.Globalization;
using System.Web.Security;
using Mvc_Schedule.Models;
using System.Web.Mvc;
using Mvc_Schedule.Models.DataModels;
using Mvc_Schedule.Models.DataModels.Entities;
using Mvc_Schedule.Models.DataModels.ModelViews;

namespace Mvc_Schedule.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly DomainContext _db = new DomainContext();

        public ScheduleController() { ViewBag.Title = "Редактор расписания"; }

        [HttpGet]
        public ActionResult Index(int id = -1, string search = "")
        {
            var model = new SchIndex(search);
            if (model.IsValid) return View("Index", model);

            var group = _db.Groups.Get(id);
            if (group == null) return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });

            model = new SchIndex
            {
                GroupId = id,
                IsAvailable = (Roles.IsUserInRole(group.FacultId.ToString(CultureInfo.InvariantCulture)) || Roles.IsUserInRole(StaticData.AdminRole)),
                Keyword = null,
                Title = group.Name
            };

            if (model.IsAvailable && (group.LastCheck - DateTime.Now).Days >= 7)
            {
                _db.Schedule.UpdateAllPlans(id);
                group.LastCheck = DateTime.Now;
                _db.SaveChanges();
            }
            return View(model);
        }

        #region @main

        [HttpGet]
        public JsonResult List(int id) { return Json(_db.Ajax.SchList(id), JsonRequestBehavior.AllowGet); }

        [HttpGet]
        public JsonResult Search(string keyword)
        {
            return Json(_db.Ajax.SchSearch(keyword), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get(int id) { return Json(_db.Ajax.SchGet(id), JsonRequestBehavior.AllowGet); }

        // {MVC3 не поддерживает сериализацию токена}
        private ScheduleTable CheckGroup(FormCollection form)
        {
            int groupId;
            short groupSub;
            if (int.TryParse(form.GetValue("group-id").AttemptedValue, out groupId)
                && short.TryParse(form.GetValue("group-sub").AttemptedValue, out groupSub))
            {
                var auditory = form.Get("auditory").Trim();
                var lector = form.Get("lector").Trim();
                var subjectTitle = form.Get("subject-title").Trim();

                if (auditory == "" || lector == "" || subjectTitle == "") return null;

                var group = _db.IsAccessableFor(groupId);
                if (group != null)
                    return new ScheduleTable
                    {
                        StudGroup = group,
                        Auditory = auditory,
                        LectorName = lector,
                        SubjectName = subjectTitle,
                        GroupSub = groupSub
                    };
            }
            return null;
        }


        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public JsonResult Add(FormCollection form)
        {

            var sch = CheckGroup(form);
            bool week;
            int lessonId, lessonType, weekdayId;
            if (sch != null &&
                bool.TryParse(form.Get("week"), out week) &&
                int.TryParse(form.Get("lesson-id"), out lessonId) &&
                int.TryParse(form.Get("lesson-type"), out lessonType) &&
                int.TryParse(form.Get("weekday-id"), out weekdayId))
            {

                sch.IsWeekOdd = week;
                sch.LessonId = lessonId;
                sch.LessonType = lessonType;
                sch.WeekdayId = weekdayId;

                sch.LectorName = _db.Lectors.Add(sch.LectorName);
                _db.Schedule.Add(sch);
                _db.Subjects.Add(sch.SubjectName);
                _db.Auditories.Add(sch.Auditory);
                _db.SaveChanges();
                return Json(sch.SubjectName + " " + sch.Auditory);
            }

            return Json(null);
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public JsonResult Edit(FormCollection form)
        {
            var sch = CheckGroup(form);
            int lessonType;
            int schId;
            if (sch != null
                && int.TryParse(form.Get("lesson-type"), out lessonType)
                && int.TryParse(form.Get("sch-id"), out  schId))
            {
                sch.ScheduleTableId = schId;
                sch.LessonType = lessonType;


                sch.LectorName = _db.Lectors.Add(sch.LectorName);
                _db.Schedule.Edit(sch);
                _db.Subjects.Add(sch.SubjectName);
                _db.Auditories.Add(sch.Auditory);
                _db.SaveChanges();
                return Json(sch.SubjectName + " " + sch.Auditory);
            }

            return Json(null);
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public JsonResult Drop(FormCollection form)
        {
            int id;
            if (!int.TryParse(form.Get("sch-id"), out id)) return Json(null);
            var result = _db.Schedule.Delete(id);
            if (result != null) _db.SaveChanges();
            return Json(result);
        }

        #endregion


        #region @feat.

        [HttpGet]
        public JsonResult GetList(string letter, string method)
        {
            if (method == "Lectors") { return Json(_db.Ajax.ListLectors(letter), JsonRequestBehavior.AllowGet); }
            else if (method == "Auditory") { return Json(_db.Ajax.ListAuditory(letter), JsonRequestBehavior.AllowGet); }
            else return Json(_db.Ajax.ListSubjects(letter), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAvailableLectors(int groupId, int timeId, string value, bool week, int weekdayId)
        {
            return Json(_db.Ajax.IsAvailableLector(groupId, timeId, value, week, weekdayId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSubjectPlan(int groupid, string value, int lessontype)
        {
            return Json(_db.Ajax.IsAvailablePlan(groupid, value, lessontype), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAvailableAuditory(int groupId, int timeId, string value, bool week, int weekdayId)
        {
            return Json(_db.Ajax.IsAvailableAuditory(groupId, timeId, value, week, weekdayId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckLessons(int groupId)
        {
            return Json(_db.Ajax.CheckListOnAvailability(groupId), JsonRequestBehavior.AllowGet);
        }

        #endregion



        [HttpGet]
        public ActionResult Excel(int id = -1, int week = 1)
        {
            var facult = _db.Facults.Get(id);
            if (facult == null) return RedirectToAction("Error", "Default", new { id = 404 });
            var result = facult.IsReady ? ExcelTemplate.Path(id, week) : _db.Schedule.CheckExcel(id, week);
            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }




        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }



        #region @static
        [HttpGet]
        //[OutputCache(Duration = 3600, VaryByParam = "id")]
        public ActionResult Static(int id = -1)
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
                    return RedirectToAction("Index", new { controller = "Facult" });
                }
            }

            ViewBag.Error = "Ошибка ввода (все поля должны быть заполнены)";
            scheduletable.Lessons = _db.Lessons.List();
            scheduletable.Weekdays = _db.Weekdays.List();

            return View(scheduletable);
        }

        #endregion


    }
}