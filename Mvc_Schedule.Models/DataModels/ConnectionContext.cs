using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Transactions;
using Mvc_Schedule.Models.DataModels.Entities;


namespace Mvc_Schedule.Models.DataModels
{
	public sealed class ConnectionContext : DbContext
	{
		public DbSet<ScheduleTable> ScheduleTables { get; set; }

		public DbSet<Lesson> Lessons { get; set; }

		public DbSet<StudGroup> StudGroups { get; set; }

		public DbSet<Facult> Facults { get; set; }

		public DbSet<Weekday> Weekdays { get; set; }
	}

    // TODO Замена на миграцию EF
	#region DbIni:

	//public class DbInitializer : DropCreateDatabaseIfModelChanges<ConnectionContext>
	//{
	//    protected override void Seed(ConnectionContext context)
	//    {
	//        new List<string> { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" }
	//            .ForEach(x => context.Weekdays.Add(new Weekday { Name = x }));

	//        base.Seed(context);
	//    }
	//}

	//public class DbReCreate : DropCreateDatabaseAlways<ConnectionContext>
	//{
	//    protected override void Seed(ConnectionContext context)
	//    {
	//        new List<string> { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" }
	//            .ForEach(x => context.Weekdays.Add(new Weekday { Name = x }));

	//        base.Seed(context);
	//    }
	//}
	//public class DbSetup : CreateDatabaseIfNotExists<ConnectionContext>
	//{
	//    protected override void Seed(ConnectionContext context)
	//    {
	//        new List<string> { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" }
	//            .ForEach(x => context.Weekdays.Add(new Weekday { Name = x }));

	//        base.Seed(context);
	//    }
	//}


	///// ver 2.2 TODO Нужно тестиро
	///// <summary>
	///// Миграция БД без ASP.NET Membership
	///// </summary>
    public class DbMigrate : IDatabaseInitializer<ConnectionContext>
    {
        /// <summary>
        /// Стратегия при миграции
        /// </summary>
        public enum MigrateStrategy
        {
            /// <summary>
            /// Очистить БД и записать таблицы
            /// </summary>
            ClearDb,
            /// <summary>
            /// Перезаписать таблицы
            /// </summary>
            DropTables,
            /// <summary>
            /// Добавить таблицы без удалений
            /// </summary>
            OnlyAddTables
        }
        private readonly MigrateStrategy _migrateType;
        public DbMigrate(MigrateStrategy migrateType = MigrateStrategy.OnlyAddTables) { _migrateType = migrateType; }
        protected virtual void Seed(ConnectionContext context)
        {
            new List<string> { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" }
                .ForEach(x => context.Weekdays.Add(new Weekday { Name = x }));
        }
        public void InitializeDatabase(ConnectionContext context)
        {
            if (context.Database.Exists())
            {
                try
                {
                    if (_migrateType != MigrateStrategy.OnlyAddTables)
                    {
                        using (var scope = new TransactionScope(TransactionScopeOption.Required))
                        {
                            if (_migrateType == MigrateStrategy.DropTables)
                            {
                                #region Список таблиц БД
                                var listOfTables = new List<String>
	                                                {
	                                                    "ScheduleTables",
	                                                    "Weekdays",
	                                                    "StudGroups",
	                                                    "Lessons",
	                                                    "Facults",
	                                                    "aspnet_PersonalizationAllUsers",
	                                                    "aspnet_PersonalizationPerUser",
	                                                    "aspnet_Profile",
	                                                    "aspnet_SchemaVersions",
	                                                    "aspnet_UsersInRoles",
	                                                    "aspnet_WebEvent_Events",
	                                                    "aspnet_Paths",
	                                                    "aspnet_Membership",
	                                                    "aspnet_Roles",
	                                                    "aspnet_Users",
	                                                    "aspnet_Applications",
	                                                    "EdmMetadata"
	                                                };
                                #endregion
                                foreach (var tableName in listOfTables) context.Database.ExecuteSqlCommand("IF OBJECT_ID ('" + tableName + "', 'U') IS NOT NULL DROP TABLE " + tableName);
                            }
                            else
                            {
                                #region Команды на удаление всех сущностей БД (табл, проц, функ, ключи)
                                /*sqldropallprocs*/
                                context.Database.ExecuteSqlCommand(
                                    "DECLARE @name VARCHAR(128) DECLARE @SQL VARCHAR(254) SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'P' AND category = 0 ORDER BY [name]) WHILE @name is not null BEGIN SELECT @SQL = 'DROP PROCEDURE [dbo].[' + RTRIM(@name) +']' EXEC (@SQL) PRINT 'Dropped Procedure: ' + @name SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'P' AND category = 0 AND [name] > @name ORDER BY [name]) END");
                                /*sqldropallviews*/
                                context.Database.ExecuteSqlCommand(
                                    "DECLARE @name VARCHAR(128) DECLARE @SQL VARCHAR(254) SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'V' AND category = 0 ORDER BY [name]) WHILE @name IS NOT NULL BEGIN SELECT @SQL = 'DROP VIEW [dbo].[' + RTRIM(@name) +']' EXEC (@SQL) PRINT 'Dropped View: ' + @name SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'V' AND category = 0 AND [name] > @name ORDER BY [name]) END ");
                                /*sqldropallfuncs*/
                                context.Database.ExecuteSqlCommand(
                                    "DECLARE @name VARCHAR(128) DECLARE @SQL VARCHAR(254) SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] IN (N'FN', N'IF', N'TF', N'FS', N'FT') AND category = 0 ORDER BY [name]) WHILE @name IS NOT NULL BEGIN SELECT @SQL = 'DROP FUNCTION [dbo].[' + RTRIM(@name) +']' EXEC (@SQL) PRINT 'Dropped Function: ' + @name SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] IN (N'FN', N'IF', N'TF', N'FS', N'FT') AND category = 0 AND [name] > @name ORDER BY [name]) END ");
                                /*sqldropallfkeys*/
                                context.Database.ExecuteSqlCommand(
                                    "DECLARE @name VARCHAR(128) DECLARE @constraint VARCHAR(254) DECLARE @SQL VARCHAR(254) SELECT @name = (SELECT TOP 1 TABLE_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'FOREIGN KEY' ORDER BY TABLE_NAME) WHILE @name is not null BEGIN SELECT @constraint = (SELECT TOP 1 CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'FOREIGN KEY' AND TABLE_NAME = @name ORDER BY CONSTRAINT_NAME) WHILE @constraint IS NOT NULL BEGIN SELECT @SQL = 'ALTER TABLE [dbo].[' + RTRIM(@name) +'] DROP CONSTRAINT ' + RTRIM(@constraint) EXEC (@SQL) PRINT 'Dropped FK Constraint: ' + @constraint + ' on ' + @name SELECT @constraint = (SELECT TOP 1 CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'FOREIGN KEY' AND CONSTRAINT_NAME <> @constraint AND TABLE_NAME = @name ORDER BY CONSTRAINT_NAME) END SELECT @name = (SELECT TOP 1 TABLE_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'FOREIGN KEY' ORDER BY TABLE_NAME) END");
                                /*sqldropallpkeys*/
                                context.Database.ExecuteSqlCommand(
                                    "DECLARE @name VARCHAR(128) DECLARE @constraint VARCHAR(254) DECLARE @SQL VARCHAR(254) SELECT @name = (SELECT TOP 1 TABLE_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'PRIMARY KEY' ORDER BY TABLE_NAME) WHILE @name IS NOT NULL BEGIN SELECT @constraint = (SELECT TOP 1 CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'PRIMARY KEY' AND TABLE_NAME = @name ORDER BY CONSTRAINT_NAME) WHILE @constraint is not null BEGIN SELECT @SQL = 'ALTER TABLE [dbo].[' + RTRIM(@name) +'] DROP CONSTRAINT ' + RTRIM(@constraint) EXEC (@SQL) PRINT 'Dropped PK Constraint: ' + @constraint + ' on ' + @name SELECT @constraint = (SELECT TOP 1 CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'PRIMARY KEY' AND CONSTRAINT_NAME <> @constraint AND TABLE_NAME = @name ORDER BY CONSTRAINT_NAME) END SELECT @name = (SELECT TOP 1 TABLE_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'PRIMARY KEY' ORDER BY TABLE_NAME) END");
                                /*sqldropalltabls*/
                                context.Database.ExecuteSqlCommand(
                                    "DECLARE @name VARCHAR(128) DECLARE @SQL VARCHAR(254) SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'U' AND category = 0 ORDER BY [name]) WHILE @name IS NOT NULL BEGIN SELECT @SQL = 'DROP TABLE [dbo].[' + RTRIM(@name) +']' EXEC (@SQL) PRINT 'Dropped Table: ' + @name SELECT @name = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'U' AND category = 0 AND [name] > @name ORDER BY [name]) END");
                                #endregion
                            }
                            scope.Complete();
                        }
                    }
                    context.Database.ExecuteSqlCommand(((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript());

                    Seed(context);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Увы, у нас ошибка MSSQL::Context->" + e.Message);
                }
            }
            else
            {
                throw new ApplicationException("Увы, у нас ошибка MSSQL::Context->База Данных отсутсвует");
            }
        }

    }



	#endregion
}
