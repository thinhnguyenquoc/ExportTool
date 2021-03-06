﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AZReport.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Configuration;
    using System.Data.SqlServerCe;

    public partial class AZModelContainer : DbContext
    {
        public AZModelContainer()
            : base("name=AZModelContainer")
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""));
            //disable initializer
            Database.SetInitializer(new MyDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //throw new UnintentionalCodeFirstException();
        }

        protected class MyDbInitializer : CreateDatabaseIfNotExists<AZModelContainer>
        {
            protected override void Seed(AZModelContainer context)
            {
//                string GetProductivity = @"CREATE PROCEDURE [dbo].[GetProductivity]
//	                                            @param1 datetime ,
//	                                            @param2 datetime
//                                            AS
//	                                            select distinct p.Code, p.Name, p.Note, p.Category, p.Duration, p.Price 
//	                                            from Schedules s join Programs p on s.Code = p.Code
//	                                            where s.Date between @param1 and @param2
//
//                                            RETURN ";
                //context.Database.ExecuteSqlCommand(GetProductivity);

//                string GetTotalQuantity = @"CREATE PROCEDURE [dbo].[GetTotalQuantity]
//	                                            @param1 datetime ,
//	                                            @param2 datetime
//                                            AS
//	                                            Declare @program Table(
//		                                            code varchar(max) not null
//	                                            ) 
//
//	                                            insert into @program select distinct p.Code 
//	                                            from Schedules s join Programs p on s.Code = p.Code
//	                                            where s.Date between @param1 and @param2
//
//	                                            --select * from @program
//
//	                                            Declare @quantity Table(
//		                                            code varchar(max) not null,
//		                                            quantity int
//	                                            )
//
//	                                            insert into @quantity select s.Code, s.Quantity
//	                                            --from Sales s join @program p on s.Code = p.code 
//	                                            from Sales s
//	                                            where s.Date between @param1 and @param2 
//
//	                                            select q.code as Code, Sum(q.quantity) as Quantity from @quantity q
//	                                            group by q.code
//
//                                            RETURN 0 ";
//                context.Database.ExecuteSqlCommand(GetTotalQuantity);

//                string GetTotalFrequency = @"CREATE PROCEDURE [dbo].[GetTotalFrequency]
//	                                            @param1 datetime ,
//	                                            @param2 datetime
//                                            AS
//	                                            Declare @temp Table(
//		                                            Code varchar(max) not null,
//		                                            Date date
//	                                            ) 
//	                                            insert into @temp select s.Code, CONVERT(date, s.Date) 
//	                                            from Schedules s join Programs p on s.Code = p.Code
//	                                            where s.Date between @param1 and @param2
//
//	                                            select t.Code, t.Date, COUNT(t.Code) Freq
//	                                            from @temp t
//	                                            group by t.Code, t.Date
//                                            RETURN ";
//                context.Database.ExecuteSqlCommand(GetTotalFrequency);

//                string GetReport = @"CREATE PROCEDURE [dbo].[GetReport]
//	                                            @param1 datetime ,
//	                                            @param2 datetime
//                                            AS
//	                                            Declare @totaltime Table(
//		                                            code varchar(max) not null,
//		                                            freq int,
//		                                            Duration datetime,
//		                                            ScheduleDate datetime,
//		                                            Quantity int
//	                                            ) 
//	                                            while @param1 <= @param2
//	                                            begin
//		                                            insert into @totaltime select s.code, Count(s.code) freq, p.Duration, CAST(s.Date AS DATE), x.Quantity
//		                                            from Schedules s join Programs p on s.Code = p.Code join Sales x on x.Code = p.Code
//		                                            where CAST(s.Date AS DATE) = @param1 and DATEPART(minute,p.Duration) > 4
//		                                            group by s.Code, p.Duration, CAST(s.Date AS DATE), x.Quantity
//	                                            set @param1 = DATEADD(day,1,@param1)
//	                                            end
//	                                            select * from @totaltime;
//                                            RETURN ";
//                context.Database.ExecuteSqlCommand(GetReport);

//                string GetTotalTime = @"CREATE PROCEDURE [dbo].[GetTotalTime]
//	                                            @param1 datetime ,
//	                                            @param2 datetime
//                                            AS
//	                                            Declare @totaltime Table(
//		                                            code varchar(max) not null,
//		                                            freq int,
//		                                            Duration datetime
//	                                            ) 
//	                                            insert into @totaltime select s.code, Count(s.code) freq, p.Duration
//	                                            from Schedules s join Programs p on s.Code = p.Code
//	                                            where s.Date between @param1 and @param2
//	                                            group by s.Code, p.Duration
//
//	                                            select distinct p.code, p.Name, t.freq, t.Duration, p.Category, p.Price, p.Note, s.Quantity, s.Date from @totaltime t join Programs p on t.code = p.code join Sales s on s.Code = p.Code
//	                                            where s.Date between @param1 and @param2
//	                                            group by s.Date,p.code, p.Name, t.freq, t.Duration, p.Category, p.Price, p.Note, s.Quantity
//                                            RETURN ";
//                context.Database.ExecuteSqlCommand(GetTotalTime);
                base.Seed(context);
            }
        }

        public DbSet<Level> Levels { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TimeSetting> TimeSettings { get; set; }
      
    }
}
