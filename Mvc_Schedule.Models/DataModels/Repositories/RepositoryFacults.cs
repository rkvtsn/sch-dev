using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Security;
using Mvc_Schedule.Models.DataModels.Entities;
using Mvc_Schedule.Models.DataModels.ModelViews;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
	public class RepositoryFacults : RepositoryBase<ConnectionContext>
	{
		public RepositoryFacults(ConnectionContext ctx) : base(ctx) { }

		public Facult Get(int id)
		{
			return _ctx.Facults.Find(id);
		}

		public IList<Facult> List()
		{
			return _ctx.Facults
					.Include(x => x.StudGroups)
					.OrderBy(x => x.Name)
					.ToList();
		}

		public IList<FacultName> ListNames(bool isGuest = false)
		{
			var roles = Roles.GetRolesForUser();

			if (roles.Contains("Admin") || isGuest)
				return (from x in _ctx.Facults orderby x.Name select new FacultName { FacultId = x.FacultId, Name = x.Name }).ToList();

			var facIds = new List<int>();
			foreach (string role in roles)
			{
				int facId;
				if (int.TryParse(role, out facId))
					facIds.Add(facId);
			}

			return (from x in _ctx.Facults
					where facIds.Contains(x.FacultId)
					orderby x.Name
					select new FacultName { FacultId = x.FacultId, Name = x.Name }).ToList();
		}

		public void Add(FacultCreate facult)
		{
			_ctx.Facults.Add(facult.FacultInstance);
			if (facult.StudGroupsNames != null)
				foreach (var group in facult.StudGroupsNames)
					_ctx.StudGroups.Add(group);
		}

		public void Edit(Facult facult)
		{
			var old = Get(facult.FacultId);
			old.Name = facult.Name;
		}

		public bool Delete(int id)
		{
			var fac = Get(id);
			if (fac == null)
				return false;
			_ctx.Facults.Remove(fac);
			return true;
		}
	}
}