using System.Collections.Generic;

namespace Pager
{
	public class PagerView<T>
	{
		public Pager Pager { get; set; }
		
		public IList<T> Model { get; set; }

	    public bool IsEmpty { get; set; }

		public bool IsDebug { get; set; }

		private bool _isValid;
		
		public bool IsValid
		{
			get { return _isValid && Pager.IsValid || IsDebug; }
			set { _isValid = value; }
		}
	}

	public class Pager
	{
		public int Current { get; set; }
		public int Next { get; set; }
		public int Previous { get; set; }

		public int Last { get; set; }
		public int First { get; set; }

		// Первая отображаемая страница
		public int PagesStart { get; set; }
		
		// Последняя отображаемая страница
		public int PagesEnd { get; set; }

		// Страница за отображаемыми
		public int? PagesMore { get; set; }
		
        // Страница перед отображаемыми
		public int? PagesLess { get; set; }

		// Валидность
        public bool IsValid { get { return (Current >= First && Current <= Last) || (Current == First); } }

		// Имеет смысл показывать
		public bool IsVisible { get { return Last > 1 && IsValid; } }

        
	}
}