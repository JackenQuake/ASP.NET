using Dapper;
using MetricsAgent.Models;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace MetricsAgent.DAL
{
    public class HddMetricsRepository : IHddMetricsRepository
    {
        private readonly NLog.Logger _logger;
        private readonly string ConnectionString;

        public HddMetricsRepository(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
            _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            _logger.Debug($"HddMetricsRepository created with {ConnectionString}.");
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
            int res1 = connection.Execute("DROP TABLE IF EXISTS hddmetrics");
            // Создаем таблицу заново
            int res2 = connection.Execute("CREATE TABLE hddmetrics(id INTEGER PRIMARY KEY, value INT, time INT)");
            _logger.Debug($"HddMetricsRepository: database created {res1}, {res2}.");
            // Если нужны тестовые данные - вносим в базу
            if (TestData)
            {
                Create(connection, new HddMetric { Id = 1, Value = 5, Time = TimeSpan.FromSeconds(5) });
                _logger.Debug("HddMetricsRepository: test data added.");
            }
        }

        public void CreateDatabase(bool TestData)
        {
            using var connection = OpenConnection();
            CreateDatabase(connection, TestData);
        }

        public void Create(SQLiteConnection connection, HddMetric item)
        {
            //  Запрос на вставку данных с плейсхолдерами для параметров
            int res = connection.Execute("INSERT INTO hddmetrics(value, time) VALUES(@value, @time)",
                // Анонимный объект с параметрами запроса
                new
                {
                    // Value подставится на место "@value" в строке запроса
                    // Значение запишется из поля Value объекта item
                    value = item.Value,
                    // Записываем в поле time количество секунд
                    time = item.Time.TotalSeconds
                });
            _logger.Debug($"HddMetricsRepository: create ({item.Value}, {item.Time.TotalSeconds}) result {res}.");
        }

        public void Create(HddMetric item)
        {
            using var connection = OpenConnection();
            Create(connection, item);
        }

        public void Delete(SQLiteConnection connection, int id)
        {
            int res = connection.Execute("DELETE FROM hddmetrics WHERE id=@id", new { id = id });
            _logger.Debug($"HddMetricsRepository: delete {id} result {res}.");
        }

        public void Delete(int id)
        {
            using var connection = OpenConnection();
            Delete(connection, id);
        }

        public void Update(SQLiteConnection connection, HddMetric item)
        {
            int res = connection.Execute("UPDATE hddmetrics SET value = @value, time = @time WHERE id=@id",
                new
                {
                    value = item.Value,
                    time = item.Time.TotalSeconds,
                    id = item.Id
                });
            _logger.Debug($"HddMetricsRepository: update ({item.Id}, {item.Value}, {item.Time.TotalSeconds}) result {res}.");
        }

        public void Update(HddMetric item)
        {
            using var connection = OpenConnection();
            Update(connection, item);
        }

        public IList<HddMetric> GetAll(SQLiteConnection connection)
        {
            // Читаем, используя Query, и в шаблон подставляем тип данных,
            // объект которого Dapper, он сам заполнит его поля
            // в соответствии с названиями колонок
            var returnList = connection.Query<HddMetric>("SELECT * FROM hddmetrics").ToList();
            _logger.Debug($"HddMetricsRepository: GetAll read {returnList.Count} records.");
            return returnList;
        }

        public IList<HddMetric> GetAll()
        {
            using var connection = OpenConnection();
            return GetAll(connection);
        }

        public IList<HddMetric> GetByTimePeriod(SQLiteConnection connection, TimeSpan from, TimeSpan to)
        {

            var returnList = connection.Query<HddMetric>("SELECT * FROM hddmetrics WHERE time BETWEEN @from AND @to", new { from = from.TotalSeconds, to = to.TotalSeconds }).ToList();
            _logger.Debug($"HddMetricsRepository: GetByTimePeriod({from.TotalSeconds}, {to.TotalSeconds}) read {returnList.Count} records.");
            return returnList;
        }

        public IList<HddMetric> GetByTimePeriod(TimeSpan from, TimeSpan to)
        {
            using var connection = OpenConnection();
            return GetByTimePeriod(connection, from, to);
        }

        public HddMetric GetById(SQLiteConnection connection, int id)
        {
            var Data = connection.QuerySingle<HddMetric>("SELECT Id, Time, Value FROM hddmetrics WHERE id=@id", new { id = id });
            if (Data != null) _logger.Debug($"HddMetricsRepository: GetById({id}) read 1 record."); else _logger.Debug($"HddMetricsRepository: GetById({id}) read NO records.");
            return Data;
        }

        public HddMetric GetById(int id)
        {
            using var connection = OpenConnection();
            return GetById(connection, id);
        }
    }
}
