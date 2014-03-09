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

		// ������ ������������ ��������
		public int PagesStart { get; set; }
		
		// ��������� ������������ ��������
		public int PagesEnd { get; set; }

		// �������� �� �������������
		public int? PagesMore { get; set; }
		
        // �������� ����� �������������
		public int? PagesLess { get; set; }

		// ����������
        public bool IsValid { get { return (Current >= First && Current <= Last) || (Current == First); } }

		// ����� ����� ����������
		public bool IsVisible { get { return Last > 1 && IsValid; } }

        
	}
}