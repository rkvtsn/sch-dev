using System.Collections.Generic;
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
            new List<string> { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" }
                .ForEach(x => context.Weekdays.AddOrUpdate(new Weekday { Name = x }));
        }
    }
}
