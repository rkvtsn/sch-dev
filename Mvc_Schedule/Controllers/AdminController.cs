using System;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Mvc_Schedule.Models.DataModels;
using Mvc_Schedule.Models.DataModels.ModelViews;


namespace Mvc_Schedule.Controllers
{
	[Authorize(Roles = StaticData.AdminRoleName)]
	public class AdminController : Controller
	{
		public ActionResult Index() { return View(); }

		[HttpPost]
		public JsonResult RefreshUsrs()
		{
			var usrs = Membership.GetAllUsers();
			var model = (from MembershipUser usr in usrs select usr.UserName).ToArray();
			return Json(model);
		}

		[HttpPost]
		public JsonResult GetRoles(string id)
		{
			var usr = Membership.GetUser(id);
			if (usr == null)
				return Json(null);

			using (var db = new ConnectionContext())
			{
				var roles = Roles.GetRolesForUser(usr.UserName);

				var roleChecks = from x in db.Facults
								 orderby x.FacultId
								 select new RoleCheck
								 {
									 Name = x.Name,
									 RoleCheckId = SqlFunctions.StringConvert((double)x.FacultId).Trim(),
									 IsChecked = roles.Contains(SqlFunctions.StringConvert((double)x.FacultId).Trim())
								 };

				var model = new Member
				{
					UserName = usr.UserName,
					RoleChecks = (roleChecks).ToList()
				};
				model.RoleChecks.Add(new RoleCheck
				{
					Name = "Администратор",
					RoleCheckId = StaticData.AdminRoleName,
					IsChecked = Roles.IsUserInRole(usr.UserName, StaticData.AdminRoleName),
				});

				return Json(model);
			}
		}

		[ValidateAntiForgeryToken]
		[HttpPost]
		public JsonResult SaveRoles(FormCollection form)
		{
			var usr = Membership.GetUser(form[1]);
			int count;
			if (usr != null && Int32.TryParse(form[0], out count))
			{
				count += 2;
				for (int i = 2; i < count; i++)
				{
					if (form[i].Equals("true", StringComparison.InvariantCultureIgnoreCase))
					{
						if (!Roles.IsUserInRole(usr.UserName, form.GetKey(i))) { Roles.AddUserToRole(usr.UserName, form.GetKey(i)); }
					}
					else
					{
						if (Roles.IsUserInRole(usr.UserName, form.GetKey(i))) { Roles.RemoveUserFromRole(usr.UserName, form.GetKey(i)); }
					}
				}
				return Json(true);
			}
			return Json(false);
		}

		[ValidateAntiForgeryToken]
		[HttpPost]
		public bool Remove(FormCollection form)
		{
			var usr = Membership.GetUser(form[1]);
			return usr != null && Membership.DeleteUser(usr.UserName, true);
		}

		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Register(RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				MembershipCreateStatus createStatus;
				Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

				if (createStatus == MembershipCreateStatus.Success)
				{
					return RedirectToAction("Index", "Admin");
				}
				ModelState.AddModelError("", ErrorCodeToString(createStatus));
			}
			return View(model);
		}

		public ActionResult ChangePassword(string id = null)
		{
			if (id != null && id.Trim() != string.Empty)
			{
				var usr = Membership.GetUser(id);
				if (usr != null)
					return View(new ChangePasswordModel { UserName = usr.UserName });
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ChangePassword(ChangePasswordModel model)
		{
			if (ModelState.IsValid)
			{
				bool isSuccess;
				try
				{
					var currentUser = Membership.GetUser(model.UserName, true);
					isSuccess = (currentUser != null) && currentUser.ChangePassword(currentUser.ResetPassword(), model.NewPassword);
				}
				catch (Exception)
				{
					isSuccess = false;
				}
				if (isSuccess)
				{
					return RedirectToAction("Index", "Admin");
				}
				ModelState.AddModelError("", "Увы, ошибочка.");
			}

			return View(model);
		}

		#region Status Codes

		private static string ErrorCodeToString(MembershipCreateStatus createStatus)
		{
			switch (createStatus)
			{
				case MembershipCreateStatus.DuplicateUserName:
					return "Имя пользователя уже существует. Введите другое имя пользователя.";

				case MembershipCreateStatus.DuplicateEmail:
					return "Имя пользователя для данного адреса электронной почты уже существует. Введите другой адрес электронной почты.";

				case MembershipCreateStatus.InvalidPassword:
					return "Указан недопустимый пароль. Введите допустимое значение пароля.";

				case MembershipCreateStatus.InvalidEmail:
					return "Указан недопустимый адрес электронной почты. Проверьте значение и повторите попытку.";

				case MembershipCreateStatus.InvalidAnswer:
					return "Указан недопустимый ответ на вопрос для восстановления пароля. Проверьте значение и повторите попытку.";

				case MembershipCreateStatus.InvalidQuestion:
					return "Указан недопустимый вопрос для восстановления пароля. Проверьте значение и повторите попытку.";

				case MembershipCreateStatus.InvalidUserName:
					return "Указано недопустимое имя пользователя. Проверьте значение и повторите попытку.";

				case MembershipCreateStatus.ProviderError:
					return "Поставщик проверки подлинности вернул ошибку. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";

				case MembershipCreateStatus.UserRejected:
					return "Запрос создания пользователя был отменен. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";

				default:
					return "Произошла неизвестная ошибка. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";
			}
		}

		#endregion
	}

}
