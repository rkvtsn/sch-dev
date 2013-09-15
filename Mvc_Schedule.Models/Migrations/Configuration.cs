using System.Collections.Generic;
using System.Web.Security;
using Mvc_Schedule.Models.DataModels;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.Migrations
{
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<DataModels.ConnectionContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Mvc_Schedule.Models.DataModels.ConnectionContext context)
        {
            //new List<string> { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" }
            //    .ForEach(x => context.Weekdays.AddOrUpdate(new Weekday { Name = x }));

            if (Membership.GetUser("admin") != null) return;

            var admin = Membership.CreateUser("admin", "password", email: "email@email.ru");
            if (!Roles.RoleExists(StaticData.AdminRoleName))
                Roles.CreateRole(StaticData.AdminRoleName);
            if (!Roles.IsUserInRole(admin.UserName, StaticData.AdminRoleName))
                Roles.AddUserToRole(admin.UserName, StaticData.AdminRoleName);
        }
    }
}
