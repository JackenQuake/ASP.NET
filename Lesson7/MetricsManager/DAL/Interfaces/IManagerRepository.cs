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
		/// <param name="from">начальная метрика времени в секундах с 01.01.1970</param>
		/// <param name="to">конечная метрика времени в секундах с 01.01.1970</param>
		/// <param name="id">идентификатор агента</param>
		/// <returns>Список метрик, сохранённых в заданном диапазоне времени для определенного агента</returns>
		IList<T> GetByTimePeriodFromAgent(TimeSpan from, TimeSpan to, int id);
	}
}
