using System;
using System.Collections.Generic;
using System.Data.SQLite;
using MetricsAgent.Models;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace MetricsAgent.DAL
{
    // Маркировочный интерфейс
    // используется, чтобы проверять работу репозитория на тесте-заглушке
    public interface IHddMetricsRepository : IRepository<HddMetric>
    {

    }

    public class HddMetricsRepository : IHddMetricsRepository
    {
        private readonly NLog.Logger _logger;

        public HddMetricsRepository()
        {
            _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            _logger.Debug("HddMetricsRepository created.");
        }

        public SQLiteConnection OpenConnection()
        {
            var connection = new SQLiteConnection("Data Source=metrics.db;Version=3;Pooling=true;Max Pool Size=100;");
            connection.Open();
            return connection;
        }

        public void CreateDatabase(SQLiteConnection connection, bool TestData)
        {
            using var cmd = new SQLiteCommand(connection);
            // Удаляем таблицу с метриками, если она есть в базе данных
            cmd.CommandText = "DROP TABLE IF EXISTS hddmetrics";
            int res1 = cmd.ExecuteNonQuery();
            // Создаем таблицу заново
            cmd.CommandText = @"CREATE TABLE hddmetrics(id INTEGER PRIMARY KEY, value INT, time INT)";
            int res2 = cmd.ExecuteNonQuery();
            _logger.Debug($"HddMetricsRepository: database created {res1}, {res2}.");

            // Если нужны тестовые данные - вносим в базу
            if (TestData)
            {
                Create(connection, new HddMetric { Id = 1, Value = 4, Time = TimeSpan.FromSeconds(12) });
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
            // Создаём команду
            using var cmd = new SQLiteCommand(connection);
            // Прописываем в команду SQL-запрос на вставку данных
            cmd.CommandText = "INSERT INTO hddmetrics(value, time) VALUES(@value, @time)";

            // Добавляем параметры в запрос из нашего объекта
            cmd.Parameters.AddWithValue("@value", item.Value);

            // В таблице будем хранить время в секундах, поэтому преобразуем перед записью в секунды
            // через свойство
            cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
            // подготовка команды к выполнению
            cmd.Prepare();

            // Выполнение команды
            int res = cmd.ExecuteNonQuery();
            _logger.Debug($"HddMetricsRepository: create ({item.Value}, {item.Time.TotalSeconds}) result {res}.");
        }

        public void Create(HddMetric item)
        {
            using var connection = OpenConnection();
            Create(connection, item);
        }

        public void Delete(SQLiteConnection connection, int id)
        {
            using var cmd = new SQLiteCommand(connection);
            // Прописываем в команду SQL-запрос на удаление данных
            cmd.CommandText = "DELETE FROM hddmetrics WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            int res = cmd.ExecuteNonQuery();
            _logger.Debug($"HddMetricsRepository: delete {id} result {res}.");
        }

        public void Delete(int id)
        {
            using var connection = OpenConnection();
            Delete(connection, id);
        }

        public void Update(SQLiteConnection connection, HddMetric item)
        {
            using var cmd = new SQLiteCommand(connection);
            // Прописываем в команду SQL-запрос на обновление данных
            cmd.CommandText = "UPDATE hddmetrics SET value = @value, time = @time WHERE id=@id;";
            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@value", item.Value);
            cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
            cmd.Prepare();
            int res = cmd.ExecuteNonQuery();
            _logger.Debug($"HddMetricsRepository: update ({item.Id}, {item.Value}, {item.Time.TotalSeconds}) result {res}.");
        }

        public void Update(HddMetric item)
        {
            using var connection = OpenConnection();
            Update(connection, item);
        }

        public IList<HddMetric> GetAll(SQLiteConnection connection)
        {
            using var cmd = new SQLiteCommand(connection);

            // Прописываем в команду SQL-запрос на получение всех данных из таблицы
            cmd.CommandText = "SELECT * FROM hddmetrics";

            var returnList = new List<HddMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                // Пока есть что читать — читаем
                while (reader.Read())
                {
                    // Добавляем объект в список возврата
                    returnList.Add(new HddMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        // Налету преобразуем прочитанные секунды в метку времени
                        Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                    });
                }
            }

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
            using var cmd = new SQLiteCommand(connection);

            // Прописываем в команду SQL-запрос на получение всех данных из таблицы
            cmd.CommandText = "SELECT * FROM hddmetrics WHERE time BETWEEN @from AND @to";
            cmd.Parameters.AddWithValue("@from", from.TotalSeconds);
            cmd.Parameters.AddWithValue("@to", to.TotalSeconds);

            var returnList = new List<HddMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                // Пока есть что читать — читаем
                while (reader.Read())
                {
                    // Добавляем объект в список возврата
                    returnList.Add(new HddMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        // Налету преобразуем прочитанные секунды в метку времени
                        Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                    });
                }
            }

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
            using var cmd = new SQLiteCommand(connection);
            cmd.CommandText = "SELECT * FROM hddmetrics WHERE id=@id";
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                // Если удалось что-то прочитать
                if (reader.Read())
                {
                    _logger.Debug($"HddMetricsRepository: GetById({id}) read 1 record.");

                    // возвращаем прочитанное
                    return new HddMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        Time = TimeSpan.FromSeconds(reader.GetInt32(1))
                    };
                }
                else
                {
                    _logger.Debug($"HddMetricsRepository: GetById({id}) read NO records.");
                    // Не нашлась запись по идентификатору, не делаем ничего
                    return null;
                }
            }
        }

        public HddMetric GetById(int id)
        {
            using var connection = OpenConnection();
            return GetById(connection, id);
        }
    }
}
