using System.Data.Entity;
using System.Linq;
using Pager;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
    public abstract class RepositoryBase<TD> where TD : DbContext
    {
        protected TD _ctx;

        protected RepositoryBase(TD ctx)
        {
            _ctx = ctx;
        }

        public void SaveChanges()
        {
            _ctx.SaveChanges();
        }
    }

    public abstract class RepositoryBase<TD, T> : RepositoryBase<TD> where TD : DbContext
    {
        public IQueryable<T> QueryPager;

        protected RepositoryBase(TD ctx) : base(ctx) { }

        protected readonly PagerViewBuilder<T> PagerViewBuilder = new PagerViewBuilder<T>();

        public PagerView<T> ListWithPager(int page = 1, int count = 10)
        {
            PagerViewBuilder.Query = QueryPager;
            PagerViewBuilder.CurrentPage = page;
            PagerViewBuilder.ItemsOnPage = count;
            
            return PagerViewBuilder.Build();
        }
    }
}
