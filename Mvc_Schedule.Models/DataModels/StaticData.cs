using System;
using System.Globalization;
using System.Web.Security;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.DataModels
{
    // Статичные Данные приложения
	public static class StaticData
	{
	    public const string AdminDefaultName = "admin";

	    public const string AdminDefaultPassword = "password";

		public const string AdminRole = "Admin";

	    public static readonly string SaltPswd = "just_salt4PasswordSOplzUseMe!" + new Random().Next();
        
	    //public static readonly IDictionary<int, string> WeekdaysConst = new Dictionary<int, string>
	    //                                                                    {
	    //                                                                        { 1, "Понедельник" }, 
	    //                                                                        { 2, "Вторник" },
	    //                                                                        { 3, "Среда" },
	    //                                                                        { 4, "Четверг" }, 
	    //                                                                        { 5, "Пятница" }, 
	    //                                                                        { 6, "Суббота" }, 
	    //                                                                        { 7, "Восскресение" }
	    //                                                                    };
        
	}
}
