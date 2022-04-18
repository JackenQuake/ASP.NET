using System;

/// <summary>
/// Расписание задач для пакета Quartz
/// </summary>
public class JobSchedule
{
	public JobSchedule(Type jobType, string cronExpression)
	{
		JobType = jobType;
		CronExpression = cronExpression;
	}

	public Type JobType { get; }
	public string CronExpression { get; }
}
