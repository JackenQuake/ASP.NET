using Dapper;
using MetricsManager.Models;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace MetricsManager.DAL
{
	public class AgentInfoRepository : IAgentInfoRepository
	{
		private readonly NLog.Logger _logger;
		private readonly string ConnectionString;

		public AgentInfoRepository(string ConnectionString)
		{
			this.ConnectionString = ConnectionString;
			_logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
			_logger.Debug($"AgentInfoRepository created with {ConnectionString}.");
			// Добавляем парсилку типа TimeSpan в качестве подсказки для SQLite
			SqlMapper.AddTypeHandler(new TimeSpanHandler());
		}

		public SQLiteConnection OpenConnection()
		{
			var connection = new SQLiteConnection(ConnectionString);
			connection.Open();
			return connection;
		}

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

		public void Create(AgentInfo item)
		{
			using var connection = OpenConnection();
			Create(connection, item);
		}

		public void Delete(SQLiteConnection connection, int id)
		{
			int res = connection.Execute("DELETE FROM agents WHERE agentid=@id", new { id = id });
			_logger.Debug($"AgentInfoRepository: delete {id} result {res}.");
		}

		public void Delete(int id)
		{
			using var connection = OpenConnection();
			Delete(connection, id);
		}

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

		public void ChangeEnabled(int id, bool enable)
		{
			using var connection = OpenConnection();
			ChangeEnabled(connection, id, enable);
		}

		public IList<AgentInfo> GetAll(SQLiteConnection connection)
		{
			var returnList = connection.Query<AgentInfo>("SELECT * FROM agents").ToList();
			_logger.Debug($"AgentInfoRepository: GetAll read {returnList.Count} records.");
			return returnList;
		}

		public IList<AgentInfo> GetAll()
		{
			using var connection = OpenConnection();
			return GetAll(connection);
		}

		public AgentInfo GetById(SQLiteConnection connection, int id)
		{
			var Data = connection.QuerySingle<AgentInfo>("SELECT * FROM agents WHERE agentid=@id", new { id = id });
			if (Data != null) _logger.Debug($"AgentInfoRepository: GetById({id}) read 1 record."); else _logger.Debug($"AgentInfoRepository: GetById({id}) read NO records.");
			return Data;
		}

		public AgentInfo GetById(int id)
		{
			using var connection = OpenConnection();
			return GetById(connection, id);
		}
	}
}
