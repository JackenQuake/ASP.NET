using Dapper;
using MetricsAgent.Models;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace MetricsAgent.DAL
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

        public void CreateDatabase(SQLiteConnection connection, bool TestData)
        {
            // Удаляем таблицу с метриками, если она есть в базе данных
            int res1 = connection.Execute("DROP TABLE IF EXISTS networkmetrics");
            // Создаем таблицу заново
            int res2 = connection.Execute("CREATE TABLE networkmetrics(id INTEGER PRIMARY KEY, value INT, time INT)");
            _logger.Debug($"NetworkMetricsRepository: database created {res1}, {res2}.");
            // Если нужны тестовые данные - вносим в базу
            if (TestData)
            {
                Create(connection, new NetworkMetric { Id = 1, Value = 5, Time = TimeSpan.FromSeconds(5) });
                _logger.Debug("NetworkMetricsRepository: test data added.");
            }
        }

        public void CreateDatabase(bool TestData)
        {
            using var connection = OpenConnection();
            CreateDatabase(connection, TestData);
        }

        public void Create(SQLiteConnection connection, NetworkMetric item)
        {
            //  Запрос на вставку данных с плейсхолдерами для параметров
            int res = connection.Execute("INSERT INTO networkmetrics(value, time) VALUES(@value, @time)",
                // Анонимный объект с параметрами запроса
                new
                {
                    // Value подставится на место "@value" в строке запроса
                    // Значение запишется из поля Value объекта item
                    value = item.Value,
                    // Записываем в поле time количество секунд
                    time = item.Time.TotalSeconds
                });
            _logger.Debug($"NetworkMetricsRepository: create ({item.Value}, {item.Time.TotalSeconds}) result {res}.");
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
            int res = connection.Execute("UPDATE networkmetrics SET value = @value, time = @time WHERE id=@id",
                new
                {
                    value = item.Value,
                    time = item.Time.TotalSeconds,
                    id = item.Id
                });
            _logger.Debug($"NetworkMetricsRepository: update ({item.Id}, {item.Value}, {item.Time.TotalSeconds}) result {res}.");
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

        public NetworkMetric GetById(SQLiteConnection connection, int id)
        {
            var Data = connection.QuerySingle<NetworkMetric>("SELECT Id, Time, Value FROM networkmetrics WHERE id=@id", new { id = id });
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
