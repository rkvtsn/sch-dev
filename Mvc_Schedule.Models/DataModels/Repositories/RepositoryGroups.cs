using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Mvc_Schedule.Models.DataModels.Entities;
using Mvc_Schedule.Models.DataModels.ModelViews;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
	public class RepositoryGroups : RepositoryBase<ConnectionContext>
	{
		public RepositoryGroups(ConnectionContext ctx) : base(ctx) { }

		public StudGroup Get(int id)
		{
			return _ctx.StudGroups.SingleOrDefault(x => x.GroupId == id);
		}

		public GroupsJson[] ByFacult(int id)
		{
			return (from x in _ctx.StudGroups
					where x.FacultId == id
					orderby x.Name
					select new GroupsJson
							{
								Id = x.GroupId,
								Name = x.Name
							}).ToArray();
		}

		public void Edit(StudGroup group)
		{
			var old = Get(group.GroupId);
			old.Name = group.Name;
			old.FacultId = group.FacultId;
		}

		public void Add(StudGroup group)
		{
			_ctx.StudGroups.Add(group);
		}

		public void AddList(StudGroup[] groups)
		{
			foreach (var studGroup in groups)
				Add(studGroup);
		}

		public void Delete(int id)
		{
			var old = Get(id);
			_ctx.StudGroups.Remove(old);
		}

		public IList<StudGroup> List()
		{
			return (from x in _ctx.StudGroups orderby x.Name select x).ToList();
		}

		public StudGroup GetFirst()
		{
			return _ctx.StudGroups.FirstOrDefault();
		}
	}
}
