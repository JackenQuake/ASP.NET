using Dapper;
using MetricsAgent.Models;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace MetricsAgent.DAL
{
    public class RamMetricsRepository : IRamMetricsRepository
    {
        private readonly NLog.Logger _logger;
        private readonly string ConnectionString;

        public RamMetricsRepository(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
            _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            _logger.Debug($"RamMetricsRepository created with {ConnectionString}.");
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
            int res1 = connection.Execute("DROP TABLE IF EXISTS rammetrics");
            // Создаем таблицу заново
            int res2 = connection.Execute("CREATE TABLE rammetrics(id INTEGER PRIMARY KEY, value INT, time INT)");
            _logger.Debug($"RamMetricsRepository: database created {res1}, {res2}.");
            // Если нужны тестовые данные - вносим в базу
            if (TestData)
            {
                Create(connection, new RamMetric { Id = 1, Value = 5, Time = TimeSpan.FromSeconds(5) });
                _logger.Debug("RamMetricsRepository: test data added.");
            }
        }

        public void CreateDatabase(bool TestData)
        {
            using var connection = OpenConnection();
            CreateDatabase(connection, TestData);
        }

        public void Create(SQLiteConnection connection, RamMetric item)
        {
            //  Запрос на вставку данных с плейсхолдерами для параметров
            int res = connection.Execute("INSERT INTO rammetrics(value, time) VALUES(@value, @time)",
                // Анонимный объект с параметрами запроса
                new
                {
                    // Value подставится на место "@value" в строке запроса
                    // Значение запишется из поля Value объекта item
                    value = item.Value,
                    // Записываем в поле time количество секунд
                    time = item.Time.TotalSeconds
                });
            _logger.Debug($"RamMetricsRepository: create ({item.Value}, {item.Time.TotalSeconds}) result {res}.");
        }

        public void Create(RamMetric item)
        {
            using var connection = OpenConnection();
            Create(connection, item);
        }

        public void Delete(SQLiteConnection connection, int id)
        {
            int res = connection.Execute("DELETE FROM rammetrics WHERE id=@id", new { id = id });
            _logger.Debug($"RamMetricsRepository: delete {id} result {res}.");
        }

        public void Delete(int id)
        {
            using var connection = OpenConnection();
            Delete(connection, id);
        }

        public void Update(SQLiteConnection connection, RamMetric item)
        {
            int res = connection.Execute("UPDATE rammetrics SET value = @value, time = @time WHERE id=@id",
                new
                {
                    value = item.Value,
                    time = item.Time.TotalSeconds,
                    id = item.Id
                });
            _logger.Debug($"RamMetricsRepository: update ({item.Id}, {item.Value}, {item.Time.TotalSeconds}) result {res}.");
        }

        public void Update(RamMetric item)
        {
            using var connection = OpenConnection();
            Update(connection, item);
        }

        public IList<RamMetric> GetAll(SQLiteConnection connection)
        {
            // Читаем, используя Query, и в шаблон подставляем тип данных,
            // объект которого Dapper, он сам заполнит его поля
            // в соответствии с названиями колонок
            var returnList = connection.Query<RamMetric>("SELECT * FROM rammetrics").ToList();
            _logger.Debug($"RamMetricsRepository: GetAll read {returnList.Count} records.");
            return returnList;
        }

        public IList<RamMetric> GetAll()
        {
            using var connection = OpenConnection();
            return GetAll(connection);
        }

        public IList<RamMetric> GetByTimePeriod(SQLiteConnection connection, TimeSpan from, TimeSpan to)
        {

            var returnList = connection.Query<RamMetric>("SELECT * FROM rammetrics WHERE time BETWEEN @from AND @to", new { from = from.TotalSeconds, to = to.TotalSeconds }).ToList();
            _logger.Debug($"RamMetricsRepository: GetByTimePeriod({from.TotalSeconds}, {to.TotalSeconds}) read {returnList.Count} records.");
            return returnList;
        }

        public IList<RamMetric> GetByTimePeriod(TimeSpan from, TimeSpan to)
        {
            using var connection = OpenConnection();
            return GetByTimePeriod(connection, from, to);
        }

        public RamMetric GetById(SQLiteConnection connection, int id)
        {
            var Data = connection.QuerySingle<RamMetric>("SELECT Id, Time, Value FROM rammetrics WHERE id=@id", new { id = id });
            if (Data != null) _logger.Debug($"RamMetricsRepository: GetById({id}) read 1 record."); else _logger.Debug($"RamMetricsRepository: GetById({id}) read NO records.");
            return Data;
        }

        public RamMetric GetById(int id)
        {
            using var connection = OpenConnection();
            return GetById(connection, id);
        }
    }
}
