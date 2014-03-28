using System;
using System.Globalization;
using System.Web.Security;
using Mvc_Schedule.Models.DataModels;
using Mvc_Schedule.Models.DataModels.Entities;
using Mvc_Schedule.Models.DataModels.Repositories;

namespace Mvc_Schedule.Models
{
    public class DomainContext : IDisposable
    {
        private readonly ConnectionContext _ctx;

        public int SaveChanges() { return _ctx.SaveChanges(); }
        public DomainContext()
        {
            _ctx = new ConnectionContext();
        }

        public StudGroup IsAccessableFor(int groupId)
        {
            var group = this.Groups.Get(groupId);
            if (group == null ||
                (!Roles.IsUserInRole(group.FacultId.ToString(CultureInfo.InvariantCulture)) &&
                 !Roles.IsUserInRole("Admin")))
                return null;
            else
                return group;
        }


        #region @Repositories

        private RepositoryLessons _lessons;
        public RepositoryLessons Lessons
        {
            get { return _lessons ?? (_lessons = new RepositoryLessons(_ctx)); }
            set { _lessons = value; }
        }

        private RepositoryAjax _ajax;
        public RepositoryAjax Ajax
        {
            get { return _ajax ?? (_ajax = new RepositoryAjax(_ctx)); }
            set { _ajax = value; }
        }


        private RepositoryScheduleTable _schedule;
        public RepositoryScheduleTable Schedule
        {
            get { return _schedule ?? (_schedule = new RepositoryScheduleTable(_ctx)); }
            set { _schedule = value; }
        }

        private RepositoryFacults _facults;
        public RepositoryFacults Facults
        {
            get { return _facults ?? (_facults = new RepositoryFacults(_ctx)); }
            set { _facults = value; }
        }

        private RepositoryGroups _groups;
        public RepositoryGroups Groups
        {
            get { return _groups ?? (_groups = new RepositoryGroups(_ctx)); }
            set { _groups = value; }
        }

        private RepositoryWeekdays _weekdays;
        public RepositoryWeekdays Weekdays
        {
            get { return _weekdays ?? (_weekdays = new RepositoryWeekdays(_ctx)); }
            set { _weekdays = value; }
        }

        private RepositoryLectors _lectors;
        public RepositoryLectors Lectors
        {
            get { return _lectors ?? (_lectors = new RepositoryLectors(_ctx)); }
            set { _lectors = value; }
        }

        private RepositorySubjects _subjects;
        public RepositorySubjects Subjects
        {
            get { return _subjects ?? (_subjects = new RepositorySubjects(_ctx)); }
            set { _subjects = value; }
        }

        private RepositoryAuditory _auditories;
        public RepositoryAuditory Auditories
        {
            get { return _auditories ?? (_auditories = new RepositoryAuditory(_ctx)); }
            set { _auditories = value; }
        }

        private RepositoryPlans _plans;
        public RepositoryPlans Plans
        {
            get { return _plans ?? (_plans = new RepositoryPlans(_ctx)); }
            set { _plans = value; }
        }

        #endregion
        #region @Implementation of IDisposable

        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed) return;
            _ctx.Dispose();
            _disposed = true;
        }

        #endregion
    }
}
