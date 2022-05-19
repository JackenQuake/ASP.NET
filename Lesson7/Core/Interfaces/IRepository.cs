using System;
using System.Collections.Generic;

namespace Core
{
	/// <summary>
	/// Интерфейс репозитория метрик
	/// </summary>
	public interface IRepository<T> where T : class
	{
		/// <summary>
		/// Получает все метрики из базы данных
		/// </summary>
		/// <returns>Список всех сохранённых метрик</returns>
		IList<T> GetAll();

		/// <summary>
		/// Получает метрики на заданном диапазоне времени
		/// </summary>
		/// <param name="fromTime">начальная метрика времени в секундах с 01.01.1970</param>
		/// <param name="toTime">конечная метрика времени в секундах с 01.01.1970</param>
		/// <returns>Список метрик, сохранённых в заданном диапазоне времени</returns>
		IList<T> GetByTimePeriod(TimeSpan from, TimeSpan to);

		/// <summary>
		/// Получает одну запись из базы данных по идентификатору
		/// </summary>
		/// <param name="id">идентификатор записи</param>
		/// <returns>Запрошенная запись</returns>
		T GetById(int id);

		/// <summary>
		/// Создает новую запись в базе данных
		/// </summary>
		/// <param name="item">Новые данные для записи в базу данных</param>
		void Create(T item);

		/// <summary>
		/// Изменяет существующую запись в базе данных
		/// </summary>
		/// <param name="item">Новые данные для записи в базу данных</param>
		void Update(T item);

		/// <summary>
		/// Удаляет запись из базы данных
		/// </summary>
		/// <param name="id">идентификатор записи</param>
		void Delete(int id);
	}
}
