using Core;
using System.Collections.Generic;
using MetricsManager.Models;

namespace MetricsManager.DAL
{
	// Маркировочный интерфейс
	// используется, чтобы проверять работу репозитория на тесте-заглушке
	public interface IAgentInfoRepository
	{
		IList<AgentInfo> GetAll();

		AgentInfo GetById(int id);

		void Create(AgentInfo item);

		void ChangeEnabled(int id, bool enable);

		void Delete(int id);
	}
}
