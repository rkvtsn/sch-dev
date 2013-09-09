using System.Collections.Generic;
using System.Linq;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
	public class RepositoryWeekdays : RepositoryBase<ConnectionContext>
	{
		public RepositoryWeekdays(ConnectionContext ctx) : base(ctx) { }

		public IList<Weekday> List()
		{
			return _ctx.Weekdays
				.OrderBy(x => x.WeekdayId)
				.ToList();
		}

		public Weekday[] Array()
		{
			return _ctx.Weekdays
				.OrderBy(x => x.WeekdayId)
				.ToArray();
		}
	}
}