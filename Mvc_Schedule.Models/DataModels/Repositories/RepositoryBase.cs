using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
	public abstract class RepositoryBase<TD> where TD : DbContext
	{
		protected TD _ctx;

		public RepositoryBase(TD ctx)
		{
			_ctx = ctx;
		}

		public void SaveChanges()
		{
			_ctx.SaveChanges();
		}
	}
}
