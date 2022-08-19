using Quartz;
using Quartz.Spi;

namespace Recorder
{
    public class QuartzFactory
    {
        // 声明一个调度工厂
        private ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler = default;
        private IJobFactory _IOCjobFactory;

        public QuartzFactory(ISchedulerFactory schedulerFactory, IJobFactory jobFactory)
        {
            _schedulerFactory = schedulerFactory;
            _IOCjobFactory = jobFactory;
        }
        public async Task<string> Start()
        {
            // 通过调度工厂获得调度器
            _scheduler = await _schedulerFactory.GetScheduler();
            // 这里是指定容器仓库
            _scheduler.JobFactory = _IOCjobFactory;
            // 开启调度器
            await _scheduler.Start();
            // 创建一个触发器
            var trigger = TriggerBuilder.Create()
                            .WithSimpleSchedule(x => x.WithIntervalInSeconds(3600).RepeatForever()) // 每n秒执行一次
                            .Build();

            // 创建任务
            var jobDetail = JobBuilder.Create<SynchronizeJob>()
                            .WithIdentity("job", "group")
                            .Build();
            // 将触发器和任务器绑定到调度器中
            await _scheduler.ScheduleJob(jobDetail, trigger);
            return await Task.FromResult("将触发器和任务器绑定到调度器中完成");
        }
    }
}
