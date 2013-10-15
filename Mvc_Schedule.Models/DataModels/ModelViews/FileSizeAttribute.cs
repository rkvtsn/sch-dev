using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Mvc_Schedule.Models.DataModels.ModelViews
{
    public class FileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSize;

        public FileSizeAttribute(int maxSize)
        {
            _maxSize = maxSize * 1024; // переводим в МБ
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;

            return _maxSize > (value as HttpPostedFileBase).ContentLength;
        }

        public override string FormatErrorMessage(string name)
        {
            return "Размер файла должен быть меньше " + _maxSize;
        }
    }
}