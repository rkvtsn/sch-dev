using System.ComponentModel.DataAnnotations;

namespace Mvc_Schedule.Models.DataModels.ModelViews
{
	public class ChangePasswordModel
	{
		public string UserName { get; set; }
		
		[Required(ErrorMessage = "Новый пароль необходимо заполнить.")]
		[StringLength(100, ErrorMessage = "Значение {0} должно содержать не менее {2} символов.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Новый пароль")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Подтверждение пароля")]
        [System.Web.Mvc.CompareAttribute("NewPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
		public string ConfirmPassword { get; set; }
	}

	public class LogOnModel
	{
		[Required(ErrorMessage = "Имя пользователя необходимо заполнить.")]
		[Display(Name = "Имя пользователя")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Пароль необходимо заполнить.")]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		[Display(Name = "Запомнить меня")]
		public bool RememberMe { get; set; }
	}

	public class RegisterModel
	{
		[Required(ErrorMessage = "Необходимо заполнить имя пользователя")]
		[Display(Name = "Имя пользователя")]
		[RegularExpression(@"[A-Za-z0-9._]{5,100}$", ErrorMessage = "Имя пользователя должно состоять из латинских символов длиной от 5 до 100")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Необходимо заполнить адрес электронной почты")]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "Адрес электронной почты")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Необходимо заполнить пароль")]
		[StringLength(100, ErrorMessage = "Значение {0} должно содержать не менее {2} символов.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Подтверждение пароля")]
        [System.Web.Mvc.CompareAttribute("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
		public string ConfirmPassword { get; set; }
	}
}
