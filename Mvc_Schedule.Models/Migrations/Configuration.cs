using System.Collections.Generic;
using System.Linq;
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

        protected override void Seed(DataModels.ConnectionContext context)
        {
            if (!context.Weekdays.Any())
                new List<string> { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" }
                    .ForEach(x => context.Weekdays.AddOrUpdate(new Weekday { Name = x }));

            if (Membership.GetUser(StaticData.AdminDefaultName) != null) return;

            var admin = Membership.CreateUser(StaticData.AdminDefaultName, StaticData.AdminDefaultPassword, email: "email@email.ru");

            if (!Roles.RoleExists(StaticData.AdminRole))
                Roles.CreateRole(StaticData.AdminRole);
            
            if (!Roles.IsUserInRole(admin.UserName, StaticData.AdminRole))
                Roles.AddUserToRole(admin.UserName, StaticData.AdminRole);
        }
    }
}
