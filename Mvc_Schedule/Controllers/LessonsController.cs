using System.Web.Mvc;
using Mvc_Schedule.Models;
using Mvc_Schedule.Models.DataModels;
using Mvc_Schedule.Models.DataModels.ModelViews;

namespace Mvc_Schedule.Controllers
{
	[Authorize(Roles = StaticData.AdminRoleName)]
    public class LessonsController : Controller
    {
        private readonly DomainContext _db = new DomainContext();

    	public LessonsController() { ViewBag.Title = "Редактор звонков"; }
		
		public ViewResult Index() { return View(_db.Lessons.List()); }

		public ActionResult Create() { return View(); } 

        [HttpPost]
		[ValidateAntiForgeryToken]
        public ActionResult Create(LessonsCreate lesson)
        {
            if (ModelState.IsValid)
            {
                _db.Lessons.Add(lesson);
                _db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(lesson);
        }

		public ActionResult Edit(int id)
		{
			var lesson = _db.Lessons.GetForEdit(id);
			return View(lesson);
		}

        [HttpPost]
		[ValidateAntiForgeryToken]
        public ActionResult Edit(LessonsTime lesson)
        {
            if (ModelState.IsValid)
            {
                _db.Lessons.Edit(lesson);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lesson);
        }

        public ActionResult Delete(int id) { return View(_db.Lessons.Get(id)); }

        [HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _db.Lessons.Delete(id);
			_db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}