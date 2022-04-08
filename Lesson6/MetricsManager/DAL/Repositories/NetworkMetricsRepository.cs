using Dapper;
using MetricsManager.Models;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace MetricsManager.DAL
{
	public class NetworkMetricsRepository : INetworkMetricsRepository
	{
		private readonly NLog.Logger _logger;
		private readonly string ConnectionString;

		public NetworkMetricsRepository(string ConnectionString)
		{
			this.ConnectionString = ConnectionString;
			_logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
			_logger.Debug($"NetworkMetricsRepository created with {ConnectionString}.");
			// Добавляем парсилку типа TimeSpan в качестве подсказки для SQLite
			SqlMapper.AddTypeHandler(new TimeSpanHandler());
		}

		public SQLiteConnection OpenConnection()
		{
			var connection = new SQLiteConnection(ConnectionString);
			connection.Open();
			return connection;
		}

		public void Create(SQLiteConnection connection, NetworkMetric item)
		{
			//  Запрос на вставку данных с плейсхолдерами для параметров
			int res = connection.Execute("INSERT INTO networkmetrics(agentid, value, time) VALUES(@agentid, @value, @time)",
				// Анонимный объект с параметрами запроса
				new
				{
					agentid = item.AgentId,
					value = item.Value,
					time = item.Time.TotalSeconds
				});
			_logger.Debug($"NetworkMetricsRepository: create ({item.AgentId}, {item.Value}, {item.Time.TotalSeconds}) result {res}.");
		}

		public void Create(NetworkMetric item)
		{
			using var connection = OpenConnection();
			Create(connection, item);
		}

		public void Delete(SQLiteConnection connection, int id)
		{
			int res = connection.Execute("DELETE FROM networkmetrics WHERE id=@id", new { id = id });
			_logger.Debug($"NetworkMetricsRepository: delete {id} result {res}.");
		}

		public void Delete(int id)
		{
			using var connection = OpenConnection();
			Delete(connection, id);
		}

		public void Update(SQLiteConnection connection, NetworkMetric item)
		{
			int res = connection.Execute("UPDATE networkmetrics SET agentid = @agentid, value = @value, time = @time WHERE id=@id",
				new
				{
					agentid = item.AgentId,
					value = item.Value,
					time = item.Time.TotalSeconds,
					id = item.Id
				});
			_logger.Debug($"NetworkMetricsRepository: update ({item.Id}, {item.AgentId}, {item.Value}, {item.Time.TotalSeconds}) result {res}.");
		}

		public void Update(NetworkMetric item)
		{
			using var connection = OpenConnection();
			Update(connection, item);
		}

		public IList<NetworkMetric> GetAll(SQLiteConnection connection)
		{
			// Читаем, используя Query, и в шаблон подставляем тип данных,
			// объект которого Dapper, он сам заполнит его поля
			// в соответствии с названиями колонок
			var returnList = connection.Query<NetworkMetric>("SELECT * FROM networkmetrics").ToList();
			_logger.Debug($"NetworkMetricsRepository: GetAll read {returnList.Count} records.");
			return returnList;
		}

		public IList<NetworkMetric> GetAll()
		{
			using var connection = OpenConnection();
			return GetAll(connection);
		}

		public IList<NetworkMetric> GetByTimePeriod(SQLiteConnection connection, TimeSpan from, TimeSpan to)
		{

			var returnList = connection.Query<NetworkMetric>("SELECT * FROM networkmetrics WHERE time BETWEEN @from AND @to", new { from = from.TotalSeconds, to = to.TotalSeconds }).ToList();
			_logger.Debug($"NetworkMetricsRepository: GetByTimePeriod({from.TotalSeconds}, {to.TotalSeconds}) read {returnList.Count} records.");
			return returnList;
		}

		public IList<NetworkMetric> GetByTimePeriod(TimeSpan from, TimeSpan to)
		{
			using var connection = OpenConnection();
			return GetByTimePeriod(connection, from, to);
		}

		public IList<NetworkMetric> GetByTimePeriodFromAgent(SQLiteConnection connection, TimeSpan from, TimeSpan to, int id)
		{

			var returnList = connection.Query<NetworkMetric>("SELECT * FROM networkmetrics WHERE (agentid = @aid) AND (time BETWEEN @from AND @to)", new { from = from.TotalSeconds, to = to.TotalSeconds, aid = id }).ToList();
			_logger.Debug($"NetworkMetricsRepository: GetByTimePeriod({from.TotalSeconds}, {to.TotalSeconds}) read {returnList.Count} records.");
			return returnList;
		}

		public IList<NetworkMetric> GetByTimePeriodFromAgent(TimeSpan from, TimeSpan to, int id)
		{
			using var connection = OpenConnection();
			return GetByTimePeriodFromAgent(connection, from, to, id);
		}

		public NetworkMetric GetById(SQLiteConnection connection, int id)
		{
			var Data = connection.QuerySingle<NetworkMetric>("SELECT * FROM networkmetrics WHERE id=@id", new { id = id });
			if (Data != null) _logger.Debug($"NetworkMetricsRepository: GetById({id}) read 1 record."); else _logger.Debug($"NetworkMetricsRepository: GetById({id}) read NO records.");
			return Data;
		}

		public NetworkMetric GetById(int id)
		{
			using var connection = OpenConnection();
			return GetById(connection, id);
		}
	}
}
