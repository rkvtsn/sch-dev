using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Mvc;
using System.Web.Security;
using Mvc_Schedule.Models.DataModels.Entities;
using Mvc_Schedule.Models.DataModels.ModelViews;
using OfficeOpenXml;
using OfficeOpenXml.Style;

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
                .GroupBy(x => x.Lesson, x => x, (k, g) => new Sc { Key = k, Group = g.OrderBy(x => x.IsWeekOdd) })
                .OrderBy(x => x.Key.Time.Hour).ThenBy(x => x.Key.Time.Minute), //.Where(x => x.GroupId == groupid)
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

        public ScheduleTableCreate ListForCreate(StudGroup group, bool week)
        {
            //var group = _ctx.StudGroups.Find(groupId);
            //if (group == null || (!Roles.IsUserInRole(group.FacultId.ToString(CultureInfo.InvariantCulture)) && !Roles.IsUserInRole("Admin")))
            //    return null;
            if (group == null) return null;

            var result = new ScheduleTableCreate
                            {
                                GroupId = group.GroupId,
                                GroupName = group.Name,
                                IsWeekOdd = week,
                                Lessons = _ctx.Lessons.OrderBy(x => x.Time.Hour).ToList(),
                                Weekdays = _ctx.Weekdays.OrderBy(x => x.WeekdayId).ToList(),
                                ScheduleTableRows = List(group.GroupId, week)
                            };

            return result;
        }

        public IList<ScheduleTable> List(int groupId, bool week)
        {
            return _ctx.ScheduleTables.Where(x => x.GroupId == groupId && week == x.IsWeekOdd).ToList();
        }


        public List<Ws> Search(string keyword)
        {
            var result = _ctx.Weekdays.Include(x => x.ScheduleTables).GroupBy(w => w, w => w.ScheduleTables.Where(x => x.LectorName.Contains(keyword) || x.Auditory.Contains(keyword))
                .GroupBy(x => x.Lesson, x => x, (k, g) => new Sc { Key = k, Group = g.OrderBy(x => x.IsWeekOdd) }).OrderBy(x => x.Key.Time), //.Where(x => x.GroupId == groupid)
                (k, g) => new Ws { Key = k, Group = g }).OrderBy(x => x.Key.WeekdayId);

            return result.ToList();
        }


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
                var id = 0; // * 0 == id -> не существует *
                if (scheduleRows.GetKey(i).EndsWith("ScheduleTableId"))
                    int.TryParse(scheduleRows[i++], out id);

                var item = new ScheduleTable
                {
                    ScheduleTableId = id,

                    Date = DateTime.Now,

                    Auditory = scheduleRows[i++].Trim(),
                    LectorName = scheduleRows[i++].Trim(),
                    SubjectName = scheduleRows[i++].Trim(),
                    LessonType = int.Parse(scheduleRows[i++]),
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


        class ComparerSchedule : IEqualityComparer<ScheduleTable>
        {
            public bool Equals(ScheduleTable x, ScheduleTable y)
            {
                return x.ScheduleTableId == y.ScheduleTableId;
            }

            public int GetHashCode(ScheduleTable obj)
            {
                return obj.GetHashCode();
            }
        }


        //
        // TODO AJAX => упростит всЁ
        //
        public bool ListAdd(ScheduleTableCreate table, StudGroup studgroup)
        {
            // Валидация// Перенос
            //var group = _ctx.StudGroups.Find(table.GroupId);
            //if (group == null || (!Roles.IsUserInRole(group.FacultId.ToString(CultureInfo.InvariantCulture)) && !Roles.IsUserInRole("Admin")))
            //    return false;
            if (studgroup == null) return false;

            // Старая таблица - для сравнения по изменению
            var oldids = table.ScheduleTableRows.Select(x => x.ScheduleTableId);
            var oldTable = (from x in _ctx.ScheduleTables
                            where studgroup.GroupId == x.GroupId
                                  && table.IsWeekOdd == x.IsWeekOdd
                                  && oldids.All(i => i != x.ScheduleTableId)
                            select x).ToList();

            foreach (var tRow in table.ScheduleTableRows)
            {
                //// Убираем 
                //var updatedItem = oldTable.SingleOrDefault(x => x.ScheduleTableId == tRow.ScheduleTableId);
                //oldTable.Remove(updatedItem);

                // Если ID == 0, то у нас новый экземпляр - Добавляем
                if (tRow.ScheduleTableId == 0)
                    _ctx.ScheduleTables.Add(tRow);
                else // Иначе изменяем по ID
                {
                    var old = _ctx.ScheduleTables.Find(tRow.ScheduleTableId);
                    old.LectorName = tRow.LectorName;
                    old.SubjectName = tRow.SubjectName;
                    old.Auditory = tRow.Auditory;
                    old.LessonType = tRow.LessonType;
                }
            }

            // Очистить удалённые
            foreach (var t in oldTable)
                _ctx.ScheduleTables.Remove(t);

            // Обновление данных для экспорта
            var facult = _ctx.Facults.Find(studgroup.FacultId);
            facult.IsReady = false;

            // Все этапы пройдены успешно
            return true;
        }



        public List<Ws> GetWeekdaysWithScheduleByFacult(int facultid, int week)
        {
            var result = _ctx.Weekdays
                        .Include(x => x.ScheduleTables)
                        .GroupBy(w => w, w => (from x in w.ScheduleTables
                                               join studGroup in _ctx.StudGroups on x.GroupId equals studGroup.GroupId
                                               join facult in _ctx.Facults on studGroup.FacultId equals facult.FacultId
                                               where x.StudGroup.FacultId == facultid && x.IsWeekOdd == (week == 1)
                                               select x)
                                              .GroupBy(l => l.Lesson, l => l, (k, g) => new Sc { Key = k, Group = g }),
                                              (k, g) => new Ws { Key = k, Group = g }); // .OrderBy(x => x.Key.WeekdayId);
            return result.ToList();
        }


        public string CheckExcel(int facultid, int week)
        {
            var templatePath = ExcelTemplate.Path(facultid);

            var resultPath = ExcelTemplate.Path(facultid, week);
            var source = new FileInfo(templatePath);
            using (var package = new ExcelPackage(source))
            {
                var result = new FileInfo(resultPath);
                if (result.Exists) result.Delete();
                var sheet = package.Workbook.Worksheets[1];
                var schTab = this.GetWeekdaysWithScheduleByFacult(facultid, week);

                // определение чётности недели
                var weekCell = (from x in sheet.Cells["A:X"]
                                where x.Value != null && x.Value.ToString().Trim().ToLower() == ("неделя")
                                select x).FirstOrDefault();

                if (weekCell == null) throw new Exception("В шаблоне нет метки недели");
                weekCell.Value = ((week == 2) ? "ч ё т н а я    н е д е л я" : "н е ч ё т н а я     н е д е л я");

                // высота линейки дня
                var lessonsRaw = _ctx.Lessons.Select(x => x).ToList();
                var lessons = lessonsRaw.Select(x => x.TimeString.Remove(2, 1)).ToArray();
                if (lessons.Length == 0) throw new Exception("Данные о звонках не найдены");
                var rowCount = lessons.Count() * 2; // TODO =>

                // Тот самый TODO
                //var k = -1;
                //int weekdayRowHeight = Math.Abs((from x in sheet.Cells["A:X"]
                //                                 where x.Value != null &&
                //                                 (x.Value.ToString().Trim().ToLower().Equals("понедельник")
                //                                 || x.Value.ToString().Trim().ToLower().Equals("вторник"))
                //                                 select x.Start.Row * (k *= -1)).Sum());

                // колонка звонков
                var firstLessonCell = (from x in sheet.Cells["A:X"]
                                       where x.Value != null && lessons.Contains(x.Value.ToString().Substring(0, 4))
                                       select x).FirstOrDefault();
                if (firstLessonCell == null) throw new Exception("В шаблоне нет временных меток");
                var lessonColumn = firstLessonCell.Start.Column;
                
                foreach (var weekday in schTab)
                {
                    var weekdayCell = (from x in sheet.Cells["A:X"]
                                       where x.Value != null && x.Value.ToString().Trim().ToLower() == weekday.Key.Name.Trim().ToLower()
                                       select x).FirstOrDefault();
                    if (weekdayCell == null) continue;
                    foreach (var lesson in weekday.Group)
                    {
                        foreach (var sc in lesson.ToArray())
                        {
                            // поиск времени урока
                            var timeCell = (from x in sheet.Cells[weekdayCell.Start.Row, lessonColumn,
                                                                  weekdayCell.Start.Row + rowCount, lessonColumn]
                                            where x.Value != null
                                                && x.Value.ToString().Trim().StartsWith(sc.Key.TimeString.Remove(2, 1))
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


                                // group-sub
                                var offset = 0;

                                if (counter == 1)
                                {
                                    if (ws.GroupSub == 0)
                                    {
                                        sheet.Cells[
                                            timeCell.Start.Row, groupCell.Start.Column,
                                            timeCell.Start.Row, groupCell.Start.Column + 1].Merge = true;
                                        sheet.Cells[
                                            timeCell.Start.Row + 1, groupCell.Start.Column,
                                            timeCell.Start.Row + 1, groupCell.Start.Column + 1].Merge = true;
                                    }
                                    else
                                    {
                                        // group-sub
                                        offset = (ws.GroupSub - 1);
                                    }
                                }

                                sheet.Cells[timeCell.Start.Row, startColumn + offset].Value = ws.SubjectName + "    " + ws.Auditory;
                                sheet.Cells[timeCell.Start.Row + 1, startColumn + offset].Value = ws.LectorName;
                                sheet.Cells[timeCell.Start.Row, startColumn + offset].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                sheet.Cells[timeCell.Start.Row + 1, startColumn + offset].Style
                                    .HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }
                        }
                    }
                }

                package.SaveAs(result);
            }
            return resultPath;
        }


        public void UpdateSchCache(int facultId)
        {
            var facult = _ctx.Facults.Find(facultId);
            facult.IsReady = false;
        }


        public void Add(ScheduleTable sch)
        {
            this._ctx.ScheduleTables.Add(sch);
            this.UpdateSchCache(sch.StudGroup.FacultId);
            this.UpdatePlan(sch, -2);
        }


        public void Edit(ScheduleTable sch)
        {
            var schOld = _ctx.ScheduleTables
                             .Include(x => x.StudGroup)
                             .SingleOrDefault(x => x.ScheduleTableId == sch.ScheduleTableId);

            if (schOld == null) return;


            schOld.Auditory = sch.Auditory;
            schOld.LectorName = sch.LectorName;
            schOld.LessonType = sch.LessonType;
            schOld.GroupSub = sch.GroupSub;

            if (schOld.SubjectName == sch.SubjectName)
            {
                this.UpdatePlan(schOld);
            }
            else
            {
                this.UpdatePlanOnDelete(schOld);
                this.UpdatePlan(sch, -2);
            }

            schOld.SubjectName = sch.SubjectName;

            this.UpdateSchCache(sch.StudGroup.FacultId);
        }

        public string Delete(int id)
        {
            var x = _ctx.ScheduleTables.Find(id);
            if (x == null) return null;
            var result = x.SubjectName + " " + x.Auditory;
            this.UpdatePlanOnDelete(x);
            _ctx.ScheduleTables.Remove(x);
            return result;
        }


        private DateTime GetSchDateTime(ScheduleTable table)
        {
            var schDate = new DateTime(table.Date.Year, table.Date.Month, table.Date.Day,
                                    table.Lesson.Time.Hour, table.Lesson.Time.Minute, table.Lesson.Time.Second);

            var weekdayForSch = NormalizationWeekday((int)schDate.DayOfWeek);
            var weekdayOffset = table.WeekdayId - weekdayForSch;
            var daysOffset = (table.WeekdayId >= weekdayForSch) ? weekdayOffset : weekdayOffset + 7;
            schDate = schDate.AddDays(daysOffset);
            return schDate;
        }

        // Скажем нет длиннокоду и CultureInfo (:
        private static int NormalizationWeekday(int weekday) { return (weekday == 0) ? 7 : weekday; }

        private void UpdatePlanOnDelete(ScheduleTable schOld)
        {
            if (!UpdatePlan(schOld)) UpdatePlan(schOld, +2);
        }

        private bool UpdatePlan(ScheduleTable sch)
        {
            var df = DateTime.Now - GetSchDateTime(sch);
            var h = (df.Days / 7 - 1) * 2;

            if (df.Hours < 2) return false;

            UpdatePlan(sch, -1 * (h + 2));
            return true;
        }

        private void UpdatePlan(ScheduleTable sch, int h)
        {
            if (sch.LessonType > 3) return;

            sch.Date = DateTime.Now;

            var plan =
                _ctx.Plans.Include(x => x.Subject)
                .SingleOrDefault(x => x.GroupId == sch.GroupId && x.Subject.Title == sch.SubjectName);

            if (plan == null || (plan.LabH == 0 && plan.LectionH == 0 && plan.PracticeH == 0)) return;

            UpdatePlanTime(plan, sch.LessonType, h);
        }


        private void UpdatePlanTime(Plan plan, int lessonType, int h)
        {
            if (lessonType == 1)
            {
                plan.LectionH = Math.Max(plan.LectionH + h, 0);
            }
            else if (lessonType == 2)
            {
                plan.LabH = Math.Max(plan.LabH + h, 0);
            }
            else
            {
                plan.PracticeH = Math.Max(plan.PracticeH + h, 0);
            }
        }


        public void UpdateAllPlans(int id)
        {
            // Select all sch
            var schTable = _ctx.ScheduleTables.Include(x => x.Lesson).Where(x => x.GroupId == id);
            // Update sch
            foreach (var x in schTable)
            {
                UpdatePlan(x);
            }
        }
    }
    #region @tagged
    //public class ScheduleExcel
    //{
    //    public Dictionary<string, ExcelRangeBase> Map { get; set; }

    //    public ScheduleExcel(ExcelWorksheet sheet)
    //    {
    //        Map = (from x in sheet.Cells["a:x"]
    //               where x.Value != null
    //               group x by x.Value.ToString() into g
    //               where Tags.All.Contains(g.Key)
    //               select new
    //               {
    //                   Key = g.Key,
    //                   Value = g.FirstOrDefault()
    //               }).ToDictionary(x => x.Key, x => x.Value);
    //    }

    //    internal static class Tags
    //    {
    //        public const string Week = "%week%";
    //        public const string Weekday = "%wd%";
    //        public const string Lesson = "%les%";
    //        public const string Time = "%time%";
    //        readonly static public string[] All = new[] { Lesson, Time, Week, Weekday };
    //    }
    //}
    #endregion

}
