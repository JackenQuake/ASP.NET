using Dapper;
using MetricsManager.Models;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace MetricsManager.DAL
{
	/// <summary>
	/// Репозиторий списка агентов
	/// </summary>
	public class AgentInfoRepository : IAgentInfoRepository
	{
		private readonly NLog.Logger _logger;
		private readonly string ConnectionString;

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="ConnectionString">Параметры подключения к базе данных</param>
		public AgentInfoRepository(string ConnectionString)
		{
			this.ConnectionString = ConnectionString;
			_logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
			_logger.Debug($"AgentInfoRepository created with {ConnectionString}.");
			// Добавляем парсилку типа TimeSpan в качестве подсказки для SQLite
			SqlMapper.AddTypeHandler(new TimeSpanHandler());
		}

		/// <summary>
		/// Открывает подключение к базе данных
		/// </summary>
		public SQLiteConnection OpenConnection()
		{
			var connection = new SQLiteConnection(ConnectionString);
			connection.Open();
			return connection;
		}

		/// <summary>
		/// Создает новую запись в базе данных для уже созданного подключения к БД
		/// </summary>
		/// <param name="connection">Подключение к базе данных</param>
		/// <param name="item">Новые данные для записи в базу данных</param>
		public void Create(SQLiteConnection connection, AgentInfo item)
		{
			//  Запрос на вставку данных с плейсхолдерами для параметров
			int res = connection.Execute("INSERT INTO agents(agentid, agenturl, agentenabled) VALUES(@agentid, @agenturl, @agentenabled)",
				// Анонимный объект с параметрами запроса
				new
				{
					agentid = item.AgentId,
					agenturl = item.AgentUrl,
					agentenabled = item.AgentEnabled
				});
			_logger.Debug($"AgentInfoRepository: create ({item.AgentId}, {item.AgentUrl}, {item.AgentEnabled}) result {res}.");
		}

		/// <summary>
		/// Подключается к базе данных и создает новую запись
		/// </summary>
		/// <param name="item">Новые данные для записи в базу данных</param>
		public void Create(AgentInfo item)
		{
			using var connection = OpenConnection();
			Create(connection, item);
		}

		/// <summary>
		/// Удаляет запись из базы данных для уже созданного подключения к БД
		/// </summary>
		/// <param name="connection">Подключение к базе данных</param>
		/// <param name="id">Идентификатор записи</param>
		public void Delete(SQLiteConnection connection, int id)
		{
			int res = connection.Execute("DELETE FROM agents WHERE agentid=@id", new { id = id });
			_logger.Debug($"AgentInfoRepository: delete {id} result {res}.");
		}

		/// <summary>
		/// Подключается к базе данных и удаляет запись
		/// </summary>
		/// <param name="id">Идентификатор записи</param>
		public void Delete(int id)
		{
			using var connection = OpenConnection();
			Delete(connection, id);
		}

		/// <summary>
		/// Разрешает или запрещает опрос указанного агента для уже созданного подключения к БД
		/// </summary>
		/// <param name="connection">Подключение к базе данных</param>
		/// <param name="id">Идентификатор записи</param>
		/// <param name="enable">Флаг разрешения работы агента</param>
		public void ChangeEnabled(SQLiteConnection connection, int id, bool enable)
		{
			int res = connection.Execute("UPDATE agents SET agentenabled = @agentenabled WHERE agentid=@agentid",
				new
				{
					agentid = id,
					agentenabled = enable
				});
			_logger.Debug($"AgentInfoRepository: update ({id}, {enable}) result {res}.");
		}

		/// <summary>
		/// Подключается к базе данных и разрешает или запрещает опрос указанного агента
		/// </summary>
		/// <param name="id">Идентификатор записи</param>
		/// <param name="enable">Флаг разрешения работы агента</param>
		public void ChangeEnabled(int id, bool enable)
		{
			using var connection = OpenConnection();
			ChangeEnabled(connection, id, enable);
		}

		/// <summary>
		/// Получает список всех агентов из базы данных для уже созданного подключения к БД
		/// </summary>
		/// <param name="connection">Подключение к базе данных</param>
		/// <returns>Список всех сохранённых агентов</returns>
		public IList<AgentInfo> GetAll(SQLiteConnection connection)
		{
			var returnList = connection.Query<AgentInfo>("SELECT * FROM agents").ToList();
			_logger.Debug($"AgentInfoRepository: GetAll read {returnList.Count} records.");
			return returnList;
		}

		/// <summary>
		/// Подключается к базе данных и получает список всех агентов
		/// </summary>
		/// <returns>Список всех сохранённых агентов</returns>
		public IList<AgentInfo> GetAll()
		{
			using var connection = OpenConnection();
			return GetAll(connection);
		}

		/// <summary>
		/// Получает одну запись из базы данных по идентификатору для уже созданного подключения к БД
		/// </summary>
		/// <param name="connection">Подключение к базе данных</param>
		/// <param name="id">Идентификатор записи</param>
		/// <returns>Запрошенная запись</returns>
		public AgentInfo GetById(SQLiteConnection connection, int id)
		{
			var Data = connection.QuerySingle<AgentInfo>("SELECT * FROM agents WHERE agentid=@id", new { id = id });
			if (Data != null) _logger.Debug($"AgentInfoRepository: GetById({id}) read 1 record."); else _logger.Debug($"AgentInfoRepository: GetById({id}) read NO records.");
			return Data;
		}

		/// <summary>
		/// Подключается к базе данных и получает одну запись по идентификатору
		/// </summary>
		/// <param name="id">Идентификатор записи</param>
		/// <returns>Запрошенная запись</returns>
		public AgentInfo GetById(int id)
		{
			using var connection = OpenConnection();
			return GetById(connection, id);
		}
	}
}
