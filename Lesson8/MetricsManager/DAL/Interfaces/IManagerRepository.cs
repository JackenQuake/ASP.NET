using Core;
using System;
using System.Collections.Generic;

namespace MetricsManager.DAL.Interfaces
{
	/// <summary>
	/// Интерфейс репозитория метрик в менеджере
	/// </summary>
	public interface IManagerRepository<T> : IRepository<T> where T : class
	{
		/// <summary>
		/// Получает метрики на заданном диапазоне времени для определенного агента
		/// </summary>
		/// <param name="agentId">идентификатор агента</param>
		/// <param name="fromTime">начальная метрика времени в секундах с 01.01.1970</param>
		/// <param name="toTime">конечная метрика времени в секундах с 01.01.1970</param>
		/// <returns>Список метрик, сохранённых в заданном диапазоне времени</returns>
		IList<T> GetByTimePeriodFromAgent(TimeSpan from, TimeSpan to, int id);
	}
}
