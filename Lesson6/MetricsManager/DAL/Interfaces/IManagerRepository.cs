using Core;
using System;
using System.Collections.Generic;

namespace MetricsManager.DAL.Interfaces
{
	public interface IManagerRepository<T> : IRepository<T> where T : class
	{
		IList<T> GetByTimePeriodFromAgent(TimeSpan from, TimeSpan to, int id);
	}
}
