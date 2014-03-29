using System.IO;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace Mvc_Schedule.Models.DataModels.Entities
{
    public class ExcelTemplate
    {
        /// <summary>
        /// Генерация пути шаблона по ID
        /// </summary>
        /// <param name="id">ID факультета</param>
        /// <returns>Путь к файлу шаблону</returns>
        static public string Path(int id)
        {
            return GetPath("template_" + id + ".xlsx");
        }

        static public string Path(int id, int week)
        {//week == 1 ? week : 2
            var sb = new StringBuilder();
            sb.Append("sch_").Append(id).Append("_").Append(week).Append(".xlsx");
            return GetPath(sb.ToString());
        }

        static private string GetPath(string name)
        {
            //return HttpContext.Current.Server.MapPath("~/Content/Excel/" + name);
            return HttpContext.Current.Server.MapPath("~/App_Data/Excel/" + name);
            //return HostingEnvironment.MapPath("~/Content/Excel/" + name);
        }

        static public void Save(HttpPostedFileBase template, Facult facult)
        {
            template.SaveAs(Path(facult.FacultId));
        }

        static public void Delete(Facult facult)
        {
            var path = new string[3];
            path[0] = Path(facult.FacultId);
            path[1] = Path(facult.FacultId, 1);
            path[2] = Path(facult.FacultId, 2);
            foreach (var p in path)
            {
                if (File.Exists(p))
                    File.Delete(p);
            }
        }

        static public Facult Update(HttpPostedFileBase template, Facult facult)
        {
            if (template == null) return facult;
            Delete(facult);
            Save(template, facult);
            facult.IsReady = false;
            return facult;
        }
        
        static public bool IsValid(HttpPostedFileBase template)
        {
            //TODO
            return true;
        }

        static public Facult Update(HttpPostedFileBase template, Facult facult, out bool isValid)
        {
            isValid = IsValid(template);
            return !isValid ? facult : Update(template, facult);
        }
        static public void Save(HttpPostedFileBase template, Facult facult, out bool isValid)
        {
            isValid = IsValid(template);
            if (isValid)
                Save(template, facult);
        }
    }
}