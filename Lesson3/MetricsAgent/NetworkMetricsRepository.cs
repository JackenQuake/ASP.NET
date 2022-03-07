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
    public interface INetworkMetricsRepository : IRepository<NetworkMetric>
    {

    }

    public class NetworkMetricsRepository : INetworkMetricsRepository
    {
        private readonly NLog.Logger _logger;

        public NetworkMetricsRepository()
        {
            _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            _logger.Debug("NetworkMetricsRepository created.");
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
            cmd.CommandText = "DROP TABLE IF EXISTS networkmetrics";
            int res1 = cmd.ExecuteNonQuery();
            // Создаем таблицу заново
            cmd.CommandText = @"CREATE TABLE networkmetrics(id INTEGER PRIMARY KEY, value INT, time INT)";
            int res2 = cmd.ExecuteNonQuery();
            _logger.Debug($"NetworkMetricsRepository: database created {res1}, {res2}.");

            // Если нужны тестовые данные - вносим в базу
            if (TestData)
            {
                Create(connection, new NetworkMetric { Id = 1, Value = 36, Time = TimeSpan.FromSeconds(18) });
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
            // Создаём команду
            using var cmd = new SQLiteCommand(connection);
            // Прописываем в команду SQL-запрос на вставку данных
            cmd.CommandText = "INSERT INTO networkmetrics(value, time) VALUES(@value, @time)";

            // Добавляем параметры в запрос из нашего объекта
            cmd.Parameters.AddWithValue("@value", item.Value);

            // В таблице будем хранить время в секундах, поэтому преобразуем перед записью в секунды
            // через свойство
            cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
            // подготовка команды к выполнению
            cmd.Prepare();

            // Выполнение команды
            int res = cmd.ExecuteNonQuery();
            _logger.Debug($"NetworkMetricsRepository: create ({item.Value}, {item.Time.TotalSeconds}) result {res}.");
        }

        public void Create(NetworkMetric item)
        {
            using var connection = OpenConnection();
            Create(connection, item);
        }

        public void Delete(SQLiteConnection connection, int id)
        {
            using var cmd = new SQLiteCommand(connection);
            // Прописываем в команду SQL-запрос на удаление данных
            cmd.CommandText = "DELETE FROM networkmetrics WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            int res = cmd.ExecuteNonQuery();
            _logger.Debug($"NetworkMetricsRepository: delete {id} result {res}.");
        }

        public void Delete(int id)
        {
            using var connection = OpenConnection();
            Delete(connection, id);
        }

        public void Update(SQLiteConnection connection, NetworkMetric item)
        {
            using var cmd = new SQLiteCommand(connection);
            // Прописываем в команду SQL-запрос на обновление данных
            cmd.CommandText = "UPDATE networkmetrics SET value = @value, time = @time WHERE id=@id;";
            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@value", item.Value);
            cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
            cmd.Prepare();
            int res = cmd.ExecuteNonQuery();
            _logger.Debug($"NetworkMetricsRepository: update ({item.Id}, {item.Value}, {item.Time.TotalSeconds}) result {res}.");
        }

        public void Update(NetworkMetric item)
        {
            using var connection = OpenConnection();
            Update(connection, item);
        }

        public IList<NetworkMetric> GetAll(SQLiteConnection connection)
        {
            using var cmd = new SQLiteCommand(connection);

            // Прописываем в команду SQL-запрос на получение всех данных из таблицы
            cmd.CommandText = "SELECT * FROM networkmetrics";

            var returnList = new List<NetworkMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                // Пока есть что читать — читаем
                while (reader.Read())
                {
                    // Добавляем объект в список возврата
                    returnList.Add(new NetworkMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        // Налету преобразуем прочитанные секунды в метку времени
                        Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                    });
                }
            }

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
            using var cmd = new SQLiteCommand(connection);

            // Прописываем в команду SQL-запрос на получение всех данных из таблицы
            cmd.CommandText = "SELECT * FROM networkmetrics WHERE time BETWEEN @from AND @to";
            cmd.Parameters.AddWithValue("@from", from.TotalSeconds);
            cmd.Parameters.AddWithValue("@to", to.TotalSeconds);

            var returnList = new List<NetworkMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                // Пока есть что читать — читаем
                while (reader.Read())
                {
                    // Добавляем объект в список возврата
                    returnList.Add(new NetworkMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        // Налету преобразуем прочитанные секунды в метку времени
                        Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                    });
                }
            }

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
            using var cmd = new SQLiteCommand(connection);
            cmd.CommandText = "SELECT * FROM networkmetrics WHERE id=@id";
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                // Если удалось что-то прочитать
                if (reader.Read())
                {
                    _logger.Debug($"NetworkMetricsRepository: GetById({id}) read 1 record.");

                    // возвращаем прочитанное
                    return new NetworkMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        Time = TimeSpan.FromSeconds(reader.GetInt32(1))
                    };
                }
                else
                {
                    _logger.Debug($"NetworkMetricsRepository: GetById({id}) read NO records.");
                    // Не нашлась запись по идентификатору, не делаем ничего
                    return null;
                }
            }
        }

        public NetworkMetric GetById(int id)
        {
            using var connection = OpenConnection();
            return GetById(connection, id);
        }
    }
}
