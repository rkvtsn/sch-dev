using System;
using System.IO;
using System.Text;
using System.Web;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
    static public class TxtUploader
    {

        // sub
        public static TxtUploaderResultModel AddListFromTxt(HttpPostedFileBase data, int minlen, Encoding encoding, Func<string[], bool> work)
        {
            return TxtUploader.AddListFromTxt(data, encoding, line =>
                {
                    var splitLine = line.Split(' ');
                    return splitLine.Length >= minlen && work(splitLine);
                });
        }

        // main
        public static TxtUploaderResultModel AddListFromTxt(HttpPostedFileBase data, Encoding encoding, Func<string, bool> work)
        {
            var result = new TxtUploaderResultModel();
            using (TextReader reader = new StreamReader(data.InputStream, encoding))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Total++;
                    if (line.Trim() == string.Empty)
                    {
                        result.Failed++;
                        continue;
                    }

                    if (work(line))
                        result.Succeed++;
                    else
                        result.Failed++;
                }
                return result;
            }
        }

    }
    public class TxtUploaderResultModel
    {
        public int Duplicates { get; set; }

        public int Total { get; set; }

        public int Succeed { get; set; }

        public int Failed { get; set; }
    }
}
