using System;
using System.Web.Mvc;
using System.Web.Security;
using Mvc_Schedule.Models.DataModels.ModelViews;

namespace Mvc_Schedule.Controllers
{
    public class AuthController : Controller
    {
		public ActionResult LogOn()
		{
			if (Request.IsAuthenticated)
				return RedirectToAction("Index", "Default");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOn(LogOnModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				if (Membership.ValidateUser(model.UserName, model.Password))
				{
					FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe); 
					if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")) return Redirect(returnUrl);
					return RedirectToAction("Index", "Default");
				}
				ModelState.AddModelError("", "Имя пользователя или пароль указаны неверно.");
			}
			return View(model);
		}

		public ActionResult LogOff()
		{
			FormsAuthentication.SignOut();
			return RedirectToAction("Index", "Default");
		}
    }
}
