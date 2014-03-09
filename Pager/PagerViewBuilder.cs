using System;
using System.Linq;

namespace Pager
{
    // ver 1.0      Работоспособный постраничный просмотр
    // ver 1.0.1    Проверка на выход из допустимого диапозона страниц
	//public static class Demo
	//{
	//    internal class MyClass { }
	//    static void Main()
	//    {
	//        var pager = new PagerViewBuilder<MyClass> { /* ... */ }.Build();
	//    }
	//}
	// Собственность Рукавицына Андрея =)
	
	/// <summary>
    /// Постраничный просмотр. Пример: var pager = new PagerViewBuilder { /* параметры */ }.Build()
	/// </summary>
    /// <typeparam name="T">Тип списка</typeparam>
    public sealed class PagerViewBuilder<T>
	{
	    private bool _isEmpty;

	    public PagerViewBuilder()
		{
			FirstPage = 1;
			ItemsOnPage = 8;
			CountOfVisiblePages = 5;
		}

        public bool IsDebug { get; set; }
		
		/// <summary>
        /// Типизированный запрос. Пример: From x in ctx orderby x.Name where x.Type == 1 select x
		/// </summary>
		public IQueryable<T> Query { get; set; }

		
        /// <summary>
        /// Первая страница (default: 1)
        /// </summary>
		public int FirstPage { get; set; }

		/// <summary>
        /// Текущая страница (id / page)
		/// </summary>
		public int CurrentPage { get; set; }

		
		/// <summary>
        /// Число элементов на странице (default: 8)
		/// </summary>
		public int ItemsOnPage { get; set; }

		
		/// <summary>
        /// Число отображаемых страниц (default: 5)
		/// </summary>
		public int CountOfVisiblePages { get; set; }

		
		/// <summary>
        /// Валидность данных
		/// </summary>
		public bool IsValid
		{
			get
			{
			    return (CurrentPage >= FirstPage && CountOfVisiblePages > 0 && ItemsOnPage > 0 && Query != null);
			}
		}

		/// <summary>
		/// Строим:
		/// Построить постраничную модель данных
		/// </summary>
        /// <returns>Model - список, Pager - страницы, IsValid - валидность данных</returns>
		public PagerView<T> Build()
		{
			var pager = new PagerView<T> {IsEmpty = _isEmpty, IsDebug = this.IsDebug};
            if (IsValid)
			{
				pager.IsValid = true;
			    pager.Pager = CreatePager();
				pager.Model = Query
							  .Skip((CurrentPage - FirstPage)*ItemsOnPage)
							  .Take(ItemsOnPage)
							  .ToList();
			}
			return pager;
		}

		// Логика пэйджинга
		private Pager CreatePager()
		{
			var pager = new Pager();
			int total = Query.Count();

		    _isEmpty = total == 0;

			pager.First = FirstPage;
			pager.Current = CurrentPage;
			pager.Next = pager.Current + 1;
			pager.Previous = pager.Current - 1;
			pager.Last = (int)Math.Ceiling((double)total / ItemsOnPage);

			#region <Математика>
			if (CountOfVisiblePages < pager.Last)
			{
				var midPages = CountOfVisiblePages / 2;
				pager.PagesStart = (pager.Current - midPages <= pager.First) ? pager.First : pager.Current - midPages;
				midPages += midPages - (pager.Current - pager.PagesStart);
				pager.PagesEnd = (pager.Current + midPages >= pager.Last) ? pager.Last : pager.Current + midPages;
				pager.PagesStart -= midPages - (pager.PagesEnd - pager.Current);
			}
			else
			{
				pager.PagesStart = pager.First;
				pager.PagesEnd = pager.Last;
			}
			#endregion </Математика>

			pager.PagesMore = (pager.PagesEnd < pager.Last) ? (int?)pager.PagesEnd + 1 : null;
			pager.PagesLess = (pager.PagesStart > pager.First) ? (int?)pager.PagesStart - 1 : null;

			return pager;
		}

		
        /// <summary>
        /// Очистка для последующего применения. Настройки сохраняются.
        /// </summary>
		public void Clear()
		{
            Query = null;
		    _isEmpty = false;
		    CurrentPage = 0;
		}
	}
}
