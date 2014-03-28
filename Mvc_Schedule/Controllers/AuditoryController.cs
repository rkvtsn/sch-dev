using System.Web.Mvc;
using Mvc_Schedule.Models;
using Mvc_Schedule.Models.DataModels;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Controllers
{
    [Authorize(Roles = StaticData.AdminRole)]
    public class AuditoryController : Controller
    {
        private readonly DomainContext _db = new DomainContext();

        public AuditoryController()
        {
            ViewBag.Title = "Справочник: Аудитории";
        }

        [HttpGet, Authorize]
        public ViewResult AddFromTxt()
        {
            return View();
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public ActionResult AddFromTxt(TxtFile model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new DomainContext())
                {
                    var result = db.Auditories.AddListFromTxt(model.Txt);
                    db.SaveChanges();
                    ViewBag.Result = result;
                    return View();
                }
            }
            return View(model);
        }


        public ViewResult Index(int page = 1)
        {
            return View(_db.Auditories.ListWithPager(page));
        }

        public ActionResult Create()
        {
            ViewBag.IsDublicate = false;
            return View();
        } 

        [HttpPost]
        public ActionResult Create(Auditory auditory)
        {
            var dublicate = _db.Auditories.IsDublicate(auditory);
            if (ModelState.IsValid && !dublicate)
            {
                _db.Auditories.Add(auditory);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IsDublicate = dublicate;
            return View(auditory);
        }
        

        public ActionResult Edit(int id)
        {
            var auditory = _db.Auditories.Get(id);
            return View(auditory);
        }

        [HttpPost]
        public ActionResult Edit(Auditory auditory)
        {
            var dublicate = _db.Auditories.IsDublicate(auditory);
            if (ModelState.IsValid && !dublicate)
            {
                _db.Auditories.Edit(auditory);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IsDublicate = dublicate;
            return View(auditory);
        }

        public ActionResult Delete(int id)
        {
            var auditory = _db.Auditories.Get(id);
            return View(auditory);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            _db.Auditories.Remove(id);
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
