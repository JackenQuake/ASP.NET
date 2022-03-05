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
    public interface ICpuMetricsRepository : IRepository<CpuMetric>
    {

    }

    public class CpuMetricsRepository : ICpuMetricsRepository
    {
        private readonly NLog.Logger _logger;

        public CpuMetricsRepository()
        {
            _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            _logger.Debug("CpuMetricsRepository created.");
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
            cmd.CommandText = "DROP TABLE IF EXISTS cpumetrics";
            int res1 = cmd.ExecuteNonQuery();
            // Создаем таблицу заново
            cmd.CommandText = @"CREATE TABLE cpumetrics(id INTEGER PRIMARY KEY, value INT, time INT)";
            int res2 = cmd.ExecuteNonQuery();
            _logger.Debug($"CpuMetricsRepository: database created {res1}, {res2}.");

            // Если нужны тестовые данные - вносим в базу
            if (TestData)
            {
                Create(connection, new CpuMetric { Id = 1, Value = 5, Time = TimeSpan.FromSeconds(5) });
                _logger.Debug("CpuMetricsRepository: test data added.");
            }
        }

        public void CreateDatabase(bool TestData)
        {
            using var connection = OpenConnection();
            CreateDatabase(connection, TestData);
        }

        public void Create(SQLiteConnection connection, CpuMetric item)
        {
            // Создаём команду
            using var cmd = new SQLiteCommand(connection);
            // Прописываем в команду SQL-запрос на вставку данных
            cmd.CommandText = "INSERT INTO cpumetrics(value, time) VALUES(@value, @time)";

            // Добавляем параметры в запрос из нашего объекта
            cmd.Parameters.AddWithValue("@value", item.Value);

            // В таблице будем хранить время в секундах, поэтому преобразуем перед записью в секунды
            // через свойство
            cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
            // подготовка команды к выполнению
            cmd.Prepare();

            // Выполнение команды
            int res = cmd.ExecuteNonQuery();
            _logger.Debug($"CpuMetricsRepository: create ({item.Value}, {item.Time.TotalSeconds}) result {res}.");
        }

        public void Create(CpuMetric item)
        {
            using var connection = OpenConnection();
            Create(connection, item);
        }

        public void Delete(SQLiteConnection connection, int id)
        {
            using var cmd = new SQLiteCommand(connection);
            // Прописываем в команду SQL-запрос на удаление данных
            cmd.CommandText = "DELETE FROM cpumetrics WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            int res = cmd.ExecuteNonQuery();
            _logger.Debug($"CpuMetricsRepository: delete {id} result {res}.");
        }

        public void Delete(int id)
        {
            using var connection = OpenConnection();
            Delete(connection, id);
        }

        public void Update(SQLiteConnection connection, CpuMetric item)
        {
            using var cmd = new SQLiteCommand(connection);
            // Прописываем в команду SQL-запрос на обновление данных
            cmd.CommandText = "UPDATE cpumetrics SET value = @value, time = @time WHERE id=@id;";
            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@value", item.Value);
            cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
            cmd.Prepare();
            int res = cmd.ExecuteNonQuery();
            _logger.Debug($"CpuMetricsRepository: update ({item.Id}, {item.Value}, {item.Time.TotalSeconds}) result {res}.");
        }

        public void Update(CpuMetric item)
        {
            using var connection = OpenConnection();
            Update(connection, item);
        }

        public IList<CpuMetric> GetAll(SQLiteConnection connection)
        {
            using var cmd = new SQLiteCommand(connection);

            // Прописываем в команду SQL-запрос на получение всех данных из таблицы
            cmd.CommandText = "SELECT * FROM cpumetrics";

            var returnList = new List<CpuMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                // Пока есть что читать — читаем
                while (reader.Read())
                {
                    // Добавляем объект в список возврата
                    returnList.Add(new CpuMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        // Налету преобразуем прочитанные секунды в метку времени
                        Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                    });
                }
            }

            _logger.Debug($"CpuMetricsRepository: GetAll read {returnList.Count} records.");
            return returnList;
        }

        public IList<CpuMetric> GetAll()
        {
            using var connection = OpenConnection();
            return GetAll(connection);
        }

        public IList<CpuMetric> GetByTimePeriod(SQLiteConnection connection, TimeSpan from, TimeSpan to)
        {
            using var cmd = new SQLiteCommand(connection);

            // Прописываем в команду SQL-запрос на получение всех данных из таблицы
            cmd.CommandText = "SELECT * FROM cpumetrics WHERE time BETWEEN @from AND @to";
            cmd.Parameters.AddWithValue("@from", from.TotalSeconds);
            cmd.Parameters.AddWithValue("@to", to.TotalSeconds);

            var returnList = new List<CpuMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                // Пока есть что читать — читаем
                while (reader.Read())
                {
                    // Добавляем объект в список возврата
                    returnList.Add(new CpuMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        // Налету преобразуем прочитанные секунды в метку времени
                        Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                    });
                }
            }

            _logger.Debug($"CpuMetricsRepository: GetByTimePeriod({from.TotalSeconds}, {to.TotalSeconds}) read {returnList.Count} records.");
            return returnList;
        }

        public IList<CpuMetric> GetByTimePeriod(TimeSpan from, TimeSpan to)
        {
            using var connection = OpenConnection();
            return GetByTimePeriod(connection, from, to);
        }

        public CpuMetric GetById(SQLiteConnection connection, int id)
        {
            using var cmd = new SQLiteCommand(connection);
            cmd.CommandText = "SELECT * FROM cpumetrics WHERE id=@id";
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                // Если удалось что-то прочитать
                if (reader.Read())
                {
                    _logger.Debug($"CpuMetricsRepository: GetById({id}) read 1 record.");

                    // возвращаем прочитанное
                    return new CpuMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        Time = TimeSpan.FromSeconds(reader.GetInt32(1))
                    };
                }
                else
                {
                    _logger.Debug($"CpuMetricsRepository: GetById({id}) read NO records.");
                    // Не нашлась запись по идентификатору, не делаем ничего
                    return null;
                }
            }
        }

        public CpuMetric GetById(int id)
		{
            using var connection = OpenConnection();
            return GetById(connection, id);
        }
    }
}
