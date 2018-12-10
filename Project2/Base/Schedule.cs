using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Common.Utilities;
using Quartz;
using Quartz.Impl;
using Common.SqlService;
using Project2.DAL;
using Common;

namespace Project2.Base
{
    public class Schedule : IJob
    {
        protected SqlDbProvider DbProvider { get; private set; }
        public Task Execute(IJobExecutionContext context)
        {

            try
            {
                DbProvider = new SqlDbProvider();

                OrderDAL orderDAL = new OrderDAL(DbProvider);
                orderDAL.DailyCheck();
                DbProvider.Dispose();
            }
            catch (Exception)
            {
                DbProvider.Dispose();
                Libs.WriteLog("Error", "ErrorToExec");
              
            }

            return null;
        }
    }

    public class ExecSchedule
    {
        public static async Task StartAsync()
        {
            IScheduler mscheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await mscheduler.Start();
            IJobDetail job = JobBuilder.Create<Schedule>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                    .WithDailyTimeIntervalSchedule
                        (s => s.WithIntervalInHours(24)
                             .OnEveryDay()
                             .StartingDailyAt(TimeOfDay.HourMinuteAndSecondOfDay(0, 0, 0))
                        ).Build();

            await mscheduler.ScheduleJob(job, trigger);
        }
    }
}