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
		IList<AgentInfo> GetAll();

		AgentInfo GetById(int id);

		void Create(AgentInfo item);

		void ChangeEnabled(int id, bool enable);

		void Delete(int id);
	}
}
