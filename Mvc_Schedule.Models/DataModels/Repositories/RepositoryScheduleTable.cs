using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Mvc_Schedule.Models.DataModels.Entities;
using System.Data.Entity;
using Mvc_Schedule.Models.DataModels.ModelViews;
using OfficeOpenXml;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
    public class Sc
    {
        public Lesson Key { get; set; }

        public IEnumerable<ScheduleTable> Group { get; set; }
    }

    public class Ws
    {
        public Weekday Key { get; set; }

        public IEnumerable<IEnumerable<Sc>> Group { get; set; }
    }

    public class RepositoryScheduleTable : RepositoryBase<ConnectionContext>
    {
        public RepositoryScheduleTable(ConnectionContext ctx) : base(ctx) { }

        public List<Ws> GetWeekdaysWithSchedule(int groupid)
        {
            var result = _ctx.Weekdays.Include(x => x.ScheduleTables).GroupBy(w => w, w => w.ScheduleTables.Where(x => x.GroupId == groupid)
                .GroupBy(x => x.Lesson, x => x, (k, g) => new Sc { Key = k, Group = g.OrderBy(x => x.IsWeekOdd) }), //.Where(x => x.GroupId == groupid)
                (k, g) => new Ws { Key = k, Group = g }).OrderBy(x => x.Key.WeekdayId);

            return result.ToList();
        }

        public ScheduleTableIndex ListForIndex(int groupId, bool week)
        {
            var group = _ctx.StudGroups.Find(groupId);

            if (group == null) return null;
            var result = new ScheduleTableIndex
                            {
                                Group = group,
                                IsWeekOdd = week,
                                Schedule = List(groupId, week),
                                Lessons = _ctx.Lessons.OrderBy(x => x.Time).ToList(),
                                Weekdays = _ctx.Weekdays.OrderBy(x => x.WeekdayId).ToList(),
                            };

            return result;
        }

        public ScheduleTableCreate ListForCreate(int groupId, bool week)
        {
            var group = _ctx.StudGroups.Find(groupId);
            if (group == null || (!Roles.IsUserInRole(group.FacultId.ToString(CultureInfo.InvariantCulture)) && !Roles.IsUserInRole("Admin")))
                return null;

            var result = new ScheduleTableCreate
                            {
                                GroupId = group.GroupId,
                                GroupName = group.Name,
                                IsWeekOdd = week,
                                Lessons = _ctx.Lessons.OrderBy(x => x.Time).ToList(),
                                Weekdays = _ctx.Weekdays.OrderBy(x => x.WeekdayId).ToList(),
                                ScheduleTableRows = List(groupId, week)
                            };

            return result;
        }

        public IList<ScheduleTable> List(int groupId, bool week)
        {
            return _ctx.ScheduleTables.Where(x => x.GroupId == groupId && week == x.IsWeekOdd).ToList();
        }

        public ScheduleTableSearch Search(string keyword, int searchType, bool week)
        {
            var q = (from x in _ctx.ScheduleTables
                     where x.IsWeekOdd == week
                     select x);

            var table = searchType == 1 ? q.Where(x => x.LectorName.Contains(keyword)).Include(x => x.StudGroup) : q.Where(x => x.Auditory.Contains(keyword)).Include(x => x.StudGroup);
            var result = new ScheduleTableSearch
            {
                IsWeekOdd = week,
                Keyword = keyword,
                SearchType = searchType,
                Lessons = _ctx.Lessons.OrderBy(x => x.Time).ToList(),
                Weekdays = _ctx.Weekdays.OrderBy(x => x.WeekdayId).ToList(),
                Schedule = table
            };
            return result;
        }


        #region old
        //public ScheduleTableSearch ListByLector(string name, bool week)
        //{
        //    var table = _ctx.ScheduleTables.Where(x => x.LectorName.Contains(name) && x.IsWeekOdd == week)
        //        .Include(x => x.StudGroup).ToList();
        //    var result = new ScheduleTableSearch
        //        {
        //            IsWeekOdd = week,
        //            Keyword = name,
        //            Lessons = _ctx.Lessons.OrderBy(x => x.Time).ToList(),
        //            Weekdays = _ctx.Weekdays.OrderBy(x => x.WeekdayId).ToList(),
        //            Schedule = table
        //        };

        //    return result;
        //}
        //public ScheduleTableSearch ListByAuditory(string auditory, bool week)
        //{
        //    var table = _ctx.ScheduleTables.Where(x => x.Auditory.Contains(auditory) && x.IsWeekOdd == week)
        //        .Include(x => x.StudGroup).ToList();
        //    var result = new ScheduleTableSearch
        //    {
        //        IsWeekOdd = week,
        //        Keyword = auditory,
        //        Lessons = _ctx.Lessons.OrderBy(x => x.Time).ToList(),
        //        Weekdays = _ctx.Weekdays.OrderBy(x => x.WeekdayId).ToList(),
        //        Schedule = table
        //    };

        //    return result;
        //}
        // алгоритм добавления расписания с уникальникализацией дисциплин
        //public void ListAdd(ScheduleTableCreate table)
        //{
        //    // Аутентификация
        //    var group = _ctx.StudGroups.Find(table.GroupId);
        //    if (group == null || (!Roles.IsUserInRole(group.FacultId.ToString(CultureInfo.InvariantCulture)) && !Roles.IsUserInRole("Admin")))
        //        return;

        //    // Старое расписание (увы без него нельзя удалить [LINQ2SQL])
        //    var list = List(table.GroupId, table.IsOddWeek);

        //    // Очищаем расписание на неделю
        //    foreach (var row in list)
        //    {
        //        _ctx.ScheduleTables.Remove(row);
        //    }

        //    // Если есть новые записи, то Добавляем их
        //    if (table.ScheduleTableRows != null)
        //    {
        //        // Очередь дисциплин
        //        var subjectsQueue = new Dictionary<string, Subject>();

        //        // Блок "уникализации" дисциплин
        //        foreach (var subjectName in table.ScheduleTableRows.Select(x => x.Subject.Name.Trim()).Distinct())
        //        {
        //            subjectsQueue[subjectName] = _ctx.Subjects.SingleOrDefault(x => x.Name == subjectName);
        //            if (subjectsQueue[subjectName] == null)
        //            {
        //                subjectsQueue[subjectName] = new Subject { Name = subjectName };
        //                _ctx.Subjects.Add(subjectsQueue[subjectName]);
        //            }
        //        }

        //        // Добавляем новые дисциплины
        //        _ctx.SaveChanges();

        //        // Теперь можно добавить информацию о расписнии в таблицу
        //        foreach (var row in table.ScheduleTableRows)
        //        {
        //            row.Subject = subjectsQueue[row.Subject.Name.Trim()];
        //            _ctx.ScheduleTables.Add(row);
        //        }
        //    }
        //}
        #endregion


        public ScheduleTableCreate FormToTable(FormCollection scheduleRows, out bool isValid)
        {
            var result = new ScheduleTableCreate
            {
                IsWeekOdd = bool.Parse(scheduleRows[2]),
                GroupId = int.Parse(scheduleRows[1]),
                ScheduleTableRows = new List<ScheduleTable>()
            };

            isValid = true;

            for (var i = 3; i < scheduleRows.Count; i++)
            {
                //if (scheduleRows.GetKey(i).EndsWith("ScheduleTableId")) i++;
                var id = 0; // * 0 id не существует *
                if (scheduleRows.GetKey(i).EndsWith("ScheduleTableId"))
                    int.TryParse(scheduleRows[i++], out id);

                var item = new ScheduleTable
                {
                    ScheduleTableId = id,
                    Auditory = scheduleRows[i++].Trim(),
                    SubjectName = scheduleRows[i++].Trim(),
                    LectorName = scheduleRows[i++].Trim(),
                    LessonId = int.Parse(scheduleRows[i++]),
                    GroupId = int.Parse(scheduleRows[i++]),
                    WeekdayId = int.Parse(scheduleRows[i]),
                    IsWeekOdd = result.IsWeekOdd
                };

                if (item.Auditory.Trim() == string.Empty || item.SubjectName.Trim() == string.Empty)
                    isValid = false;

                result.ScheduleTableRows.Add(item);
            }

            return result;
        }

        //
        // TODO Всё переделать на AJAX ! - упростит мне всю работу с SQL
        //
        public void ListAdd(ScheduleTableCreate table)
        {
            // Валидация
            var group = _ctx.StudGroups.Find(table.GroupId);
            if (group == null || (!Roles.IsUserInRole(group.FacultId.ToString(CultureInfo.InvariantCulture)) && !Roles.IsUserInRole("Admin")))
                return;

            // Старая таблица - для сравнения по изменению
            var oldTable = List(table.GroupId, table.IsWeekOdd);

            foreach (var tRow in table.ScheduleTableRows)
            {
                // Убираем измененные
                var updatedItem = oldTable.SingleOrDefault(x => x.ScheduleTableId == tRow.ScheduleTableId);
                oldTable.Remove(updatedItem);

                // Если ID == 0, то у нас новый экземпляр - Добавляем
                if (tRow.ScheduleTableId == 0)
                    _ctx.ScheduleTables.Add(tRow);
                else // Иначе изменяем по ID
                {
                    var old = _ctx.ScheduleTables.Find(tRow.ScheduleTableId);
                    old.LectorName = tRow.LectorName;
                    old.SubjectName = tRow.SubjectName;
                    old.Auditory = tRow.Auditory;
                }
            }

            // Очистить удалённые
            foreach (var t in oldTable)
                _ctx.ScheduleTables.Remove(t);
        }

        public string[] ListSubjects(string firstLetters)
        {
            return (from x in _ctx.Subjects
                    orderby x.Title
                    where x.Title.ToLower().StartsWith(firstLetters.ToLower())
                    select x.Title).Distinct().ToArray();
        }

        public string[] ListLectors(string firstLetters)
        {
            var list = (from x in _ctx.Lectors
                        orderby x.SecondName
                        where
                            (x.SecondName.ToLower().StartsWith(firstLetters.ToLower())) ||
                            (x.Name.ToLower().StartsWith(firstLetters.ToLower()))
                        select x).Distinct().ToList();

            return list.Select(x => x.FullName).ToArray();
        }

        public string[] ListAuditory(string firstLetters)
        {
            return (from x in _ctx.ScheduleTables
                    orderby x.Auditory
                    where x.Auditory.ToLower().StartsWith(firstLetters.ToLower())
                    select x.Auditory).Distinct().ToArray();
        }

        public string IsAvailableLector(int timeId, string value, bool week)
        {

            return _ctx.ScheduleTables
                    .Where(x => x.LessonId == timeId && x.LectorName == value && x.IsWeekOdd == week)
                    .Select(x => x.GroupId)
                    .FirstOrDefault()
                    .ToString(CultureInfo.InvariantCulture);

        }

        public string IsAvailableAuditory(int timeId, string value, bool week)
        {
            return _ctx.ScheduleTables
                    .Where(x => x.LessonId == timeId && x.Auditory == value && x.IsWeekOdd == week)
                    .Select(x => x.GroupId).FirstOrDefault().ToString(CultureInfo.InvariantCulture);
        }

        public void UpdateExcel(int facultid, int week)
        {
            var newpath = HttpContext.Current.Server.MapPath("~/Content/xls/new.xlsx");
            var templatename = "template.xlsx";
            var path = HttpContext.Current.Server.MapPath("~/Content/xls/" + templatename);

            var source = new FileInfo(path);
            using (var package = new ExcelPackage(source))
            {
                var result = new FileInfo(newpath);

                if (result.Exists)
                    result.Delete();

                var sheet = package.Workbook.Worksheets[1];

                var schTab = this.GetWeekdaysWithScheduleByFacult(facultid, week);

                foreach (var weekday in schTab)
                {
                    // поиск день недели
                    var weekdayCell = (from x in sheet.Cells["A:X"]
                                       where x.Value != null && x.Value.ToString().Trim().ToLower() == weekday.Key.Name.Trim().ToLower()
                                       select x).FirstOrDefault();
                    if (weekdayCell == null) continue;
                    foreach (var lesson in weekday.Group)
                    {
                        foreach (var sc in lesson.ToArray())
                        {
                            // поиск время урока
                            var timeCell = (from x in sheet.Cells[weekdayCell.Start.Row, weekdayCell.Start.Column + 1, weekdayCell.End.Row + 10, weekdayCell.Start.Column + 10] // TODO
                                            where x.Value != null
                                                && x.Value.ToString().Trim().StartsWith(sc.Key.TimeString.Remove(2))
                                            select x).FirstOrDefault();
                            foreach (var ws in sc.Group)
                            {
                                var counter = sc.Group.Count(x => x.GroupId == ws.GroupId);
                                // поиск группы
                                var groupCell = (from x in sheet.Cells
                                                 where x.Value != null
                                                       && x.Value.ToString() == ws.StudGroup.Name
                                                 select x).FirstOrDefault();

                                if (timeCell == null || groupCell == null) continue;

                                var startColumn = groupCell.Start.Column;
                                if (sheet.Cells[timeCell.Start.Row, groupCell.Start.Column].Value != null)
                                {
                                    startColumn++;
                                }
                                sheet.Cells[timeCell.Start.Row, startColumn].Value = ws.SubjectName + "    " + ws.Auditory;
                                sheet.Cells[timeCell.Start.Row + 1, startColumn].Value = ws.LectorName;
                                
                                if (counter == 1)
                                {
                                    sheet.Cells[
                                        timeCell.Start.Row, groupCell.Start.Column,
                                        timeCell.Start.Row, groupCell.Start.Column + 1].Merge = true;
                                    sheet.Cells[
                                        timeCell.Start.Row + 1, groupCell.Start.Column,
                                        timeCell.Start.Row + 1, groupCell.Start.Column + 1].Merge = true;
                                }
                            }
                        }
                    }
                }

                package.SaveAs(result);
            }
        }

        public List<Ws> GetWeekdaysWithScheduleByFacult(int facultid, int week)
        {
            var result = _ctx.Weekdays
                        .Include(x => x.ScheduleTables) // псевдо JOIN
                        .GroupBy(w => w, w => (from x in w.ScheduleTables // подзапрос.выборка SELECT
                                               join studGroup in _ctx.StudGroups on x.GroupId equals studGroup.GroupId
                                               join facult in _ctx.Facults on studGroup.FacultId equals facult.FacultId
                                               where x.StudGroup.FacultId == facultid && x.IsWeekOdd == (week == 1) // УСЛОВИЕ
                                               select x)
                                              .GroupBy(l => l.Lesson, l => l, (k, g) => new Sc() { Key = k, Group = g }),
                                              (k, g) => new Ws { Key = k, Group = g }); // .OrderBy(x => x.Key.WeekdayId);
            return result.ToList();
        }

        public void RenderToExcel(int id, int week)
        {
            var path = HttpContext.Current.Server.MapPath("~/Content/xls/render.xlsx");
            using (var package = new ExcelPackage())
            {
                package.Workbook.Worksheets.Add("Чётная");
                var sheet = package.Workbook.Worksheets[1];

                sheet.Cells[1, 1].Value = "Привет Мир";

                Byte[] bin = package.GetAsByteArray();

                File.WriteAllBytes(path, bin);
            }
        }
    }

    public class ScheduleExcel
    {
        public Dictionary<string, ExcelRangeBase> Map { get; set; }

        public ScheduleExcel(ExcelWorksheet sheet)
        {
            Map = (from x in sheet.Cells["a:x"]
                   where x.Value != null
                   group x by x.Value.ToString() into g
                   where Tags.All.Contains(g.Key)
                   select new
                   {
                       Key = g.Key,
                       Value = g.FirstOrDefault()
                   }).ToDictionary(x => x.Key, x => x.Value);
        }

        internal static class Tags
        {
            public const string Week = "%week%";
            public const string Weekday = "%wd%";
            public const string Lesson = "%les%";
            public const string Time = "%time%";
            readonly static public string[] All = new[] { Lesson, Time, Week, Weekday };
        }
    }
}
