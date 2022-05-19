using Core;
using System.Collections.Generic;
using MetricsManager.Models;

namespace MetricsManager.DAL
{
	/// <summary>
	/// Интерфейс репозитория списка агентов
	/// </summary>
	public interface IAgentInfoRepository
	{
		/// <summary>
		/// Получает список всех агентов из базы данных
		/// </summary>
		/// <returns>Список всех сохранённых агентов</returns>
		IList<AgentInfo> GetAll();

		/// <summary>
		/// Получает одну запись из базы данных по идентификатору
		/// </summary>
		/// <param name="id">Идентификатор записи</param>
		/// <returns>Запрошенная запись</returns>
		AgentInfo GetById(int id);

		/// <summary>
		/// Создает новую запись в базе данных
		/// </summary>
		/// <param name="item">Новые данные для записи в базу данных</param>
		void Create(AgentInfo item);

		/// <summary>
		/// Разрешает или запрещает опрос указанного агента
		/// </summary>
		/// <param name="id">Идентификатор записи</param>
		/// <param name="enable">Флаг разрешения работы агента</param>
		void ChangeEnabled(int id, bool enable);

		/// <summary>
		/// Удаляет запись из базы данных
		/// </summary>
		/// <param name="id">Идентификатор записи</param>
		void Delete(int id);
	}
}
