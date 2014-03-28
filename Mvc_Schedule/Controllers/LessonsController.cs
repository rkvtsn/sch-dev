using System;
using System.Linq;
using System.Web.Mvc;
using Mvc_Schedule.Models;
using Mvc_Schedule.Models.DataModels;
using Mvc_Schedule.Models.DataModels.Entities;
using Mvc_Schedule.Models.DataModels.ModelViews;

namespace Mvc_Schedule.Controllers
{
    public class LessonsController : Controller
    {
        private readonly DomainContext _db = new DomainContext();

        public LessonsController() { ViewBag.Title = "Редактор звонков"; }
        
        [Authorize(Roles = StaticData.AdminRole)]
        public ViewResult Index()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = StaticData.AdminRole)]
        public JsonResult Add(FormCollection form)
        {
            var lesson = new Lesson { Time = DateTime.Parse(form[0] + ":" + form[1]) };
            var success = _db.Lessons.Add(lesson);
            _db.SaveChanges();
            return success ? Json(lesson.TimeString) : Json(false);
        }

        [HttpGet]
        public JsonResult List()
        {
            var model = _db.Lessons.Array();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = StaticData.AdminRole)]
        public JsonResult Edit(FormCollection form)
        {
            int h = 0;
            int m = 0;
            int id = 0;
            if (int.TryParse(form[0], out h) && int.TryParse(form[1], out m) && int.TryParse(form[2], out id))
            {
                var lesson = new LessonsTime() { Hours = h, Minutes = m, LessonId = id };
                _db.Lessons.Edit(lesson);
                _db.SaveChanges();
                return Json(lesson.Time.ToShortTimeString());
            }
            return Json(false);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = StaticData.AdminRole)]
        public JsonResult Drop(FormCollection form)
        {
            int id = 0;
            if (int.TryParse(form.GetValue("lesson-id").AttemptedValue, out id))
            {
                var result = _db.Lessons.Delete(id);
                _db.SaveChanges();
                return Json(result.TimeString);
            }
            return Json(false);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }



        //public ActionResult Create() { return View(); }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(LessonsCreate lesson)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.Lessons.Add(lesson);
        //        _db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(lesson);
        //}
        //public ActionResult Delete(int id) { return View(_db.Lessons.Get(id)); }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    _db.Lessons.Delete(id);
        //    _db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        //public ActionResult Edit(int id)
        //{
        //    var lesson = _db.Lessons.GetForEdit(id);
        //    return View(lesson);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(LessonsTime lesson)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.Lessons.Edit(lesson);
        //        _db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(lesson);
        //}
        
    }
}