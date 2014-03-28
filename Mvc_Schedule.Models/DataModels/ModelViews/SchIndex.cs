using System.Web.UI.WebControls;

namespace Mvc_Schedule.Models.DataModels.ModelViews
{
    public class SchIndex
    {
        private bool _isValid;

        public SchIndex()
        {
            
        }

        public SchIndex(string keyword)
        {
            _isValid = (keyword != null && keyword.Trim() != string.Empty);
            if (!_isValid) return;
            this.Keyword = keyword.Trim();
            this.Title = "Поиск: " + ((Keyword.Length > 10) ? Keyword.Substring(0, 10) : Keyword);
        }


        public bool IsValid { get { return _isValid; } }

        public string Title { get; set; }

        public string Keyword { get; set; }

        public int GroupId { get; set; }

        public bool IsAvailable { get; set; }
    }
}
