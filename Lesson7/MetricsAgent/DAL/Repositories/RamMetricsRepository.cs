using Dapper;
using MetricsAgent.Models;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace MetricsAgent.DAL
{
	/// <summary>
	/// Репозиторий метрики RAM (объем свободной памяти)
	/// </summary>
	public class RamMetricsRepository : IRamMetricsRepository
	{
		private readonly NLog.Logger _logger;
		private readonly string ConnectionString;

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="ConnectionString">Параметры подключения к базе данных</param>
		public RamMetricsRepository(string ConnectionString)
		{
			this.ConnectionString = ConnectionString;
			_logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
			_logger.Debug($"RamMetricsRepository created with {ConnectionString}.");
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
		/// Создает таблицу в базе данных для уже созданного подключения к БД
		/// </summary>
		/// <param name="connection">Подключение к базе данных</param>
		/// <param name="TestData">Отладочный параметр: указывает, требуется ли занести в базу тестовую запись</param>
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

		/// <summary>
		/// Подключается к базе данных и создает таблицу
		/// </summary>
		/// <param name="TestData">Отладочный параметр: указывает, требуется ли занести в базу тестовую запись</param>
		public void CreateDatabase(bool TestData)
		{
			using var connection = OpenConnection();
			CreateDatabase(connection, TestData);
		}

		/// <summary>
		/// Создает новую запись в базе данных для уже созданного подключения к БД
		/// </summary>
		/// <param name="connection">Подключение к базе данных</param>
		/// <param name="item">Новые данные для записи в базу данных</param>
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

		/// <summary>
		/// Подключается к базе данных и создает новую запись
		/// </summary>
		/// <param name="item">Новые данные для записи в базу данных</param>
		public void Create(RamMetric item)
		{
			using var connection = OpenConnection();
			Create(connection, item);
		}

		/// <summary>
		/// Удаляет запись из базы данных для уже созданного подключения к БД
		/// </summary>
		/// <param name="connection">Подключение к базе данных</param>
		/// <param name="id">идентификатор записи</param>
		public void Delete(SQLiteConnection connection, int id)
		{
			int res = connection.Execute("DELETE FROM rammetrics WHERE id=@id", new { id = id });
			_logger.Debug($"RamMetricsRepository: delete {id} result {res}.");
		}

		/// <summary>
		/// Подключается к базе данных и удаляет запись
		/// </summary>
		/// <param name="id">идентификатор записи</param>
		public void Delete(int id)
		{
			using var connection = OpenConnection();
			Delete(connection, id);
		}

		/// <summary>
		/// Изменяет существующую запись в базе данных для уже созданного подключения к БД
		/// </summary>
		/// <param name="connection">Подключение к базе данных</param>
		/// <param name="item">Новые данные для записи в базу данных</param>
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

		/// <summary>
		/// Подключается к базе данных и изменяет существующую запись
		/// </summary>
		/// <param name="item">Новые данные для записи в базу данных</param>
		public void Update(RamMetric item)
		{
			using var connection = OpenConnection();
			Update(connection, item);
		}

		/// <summary>
		/// Получает все метрики из базы данных для уже созданного подключения к БД
		/// </summary>
		/// <param name="connection">Подключение к базе данных</param>
		/// <returns>Список всех сохранённых метрик</returns>
		public IList<RamMetric> GetAll(SQLiteConnection connection)
		{
			// Читаем, используя Query, и в шаблон подставляем тип данных,
			// объект которого Dapper, он сам заполнит его поля
			// в соответствии с названиями колонок
			var returnList = connection.Query<RamMetric>("SELECT * FROM rammetrics").ToList();
			_logger.Debug($"RamMetricsRepository: GetAll read {returnList.Count} records.");
			return returnList;
		}

		/// <summary>
		/// Подключается к базе данных и получает все метрики
		/// </summary>
		/// <returns>Список всех сохранённых метрик</returns>
		public IList<RamMetric> GetAll()
		{
			using var connection = OpenConnection();
			return GetAll(connection);
		}

		/// <summary>
		/// Получает метрики на заданном диапазоне времени для уже созданного подключения к БД
		/// </summary>
		/// <param name="connection">Подключение к базе данных</param>
		/// <param name="from">начальная метрика времени в секундах с 01.01.1970</param>
		/// <param name="to">конечная метрика времени в секундах с 01.01.1970</param>
		/// <returns>Список метрик, сохранённых в заданном диапазоне времени</returns>
		public IList<RamMetric> GetByTimePeriod(SQLiteConnection connection, TimeSpan from, TimeSpan to)
		{

			var returnList = connection.Query<RamMetric>("SELECT * FROM rammetrics WHERE time BETWEEN @from AND @to", new { from = from.TotalSeconds, to = to.TotalSeconds }).ToList();
			_logger.Debug($"RamMetricsRepository: GetByTimePeriod({from.TotalSeconds}, {to.TotalSeconds}) read {returnList.Count} records.");
			return returnList;
		}

		/// <summary>
		/// Подключается к базе данных и получает метрики на заданном диапазоне времени
		/// </summary>
		/// <param name="from">начальная метрика времени в секундах с 01.01.1970</param>
		/// <param name="to">конечная метрика времени в секундах с 01.01.1970</param>
		/// <returns>Список метрик, сохранённых в заданном диапазоне времени</returns>
		public IList<RamMetric> GetByTimePeriod(TimeSpan from, TimeSpan to)
		{
			using var connection = OpenConnection();
			return GetByTimePeriod(connection, from, to);
		}

		/// <summary>
		/// Получает одну запись из базы данных по идентификатору для уже созданного подключения к БД
		/// </summary>
		/// <param name="connection">Подключение к базе данных</param>
		/// <param name="id">идентификатор записи</param>
		/// <returns>Запрошенная запись</returns>
		public RamMetric GetById(SQLiteConnection connection, int id)
		{
			var Data = connection.QuerySingle<RamMetric>("SELECT Id, Time, Value FROM rammetrics WHERE id=@id", new { id = id });
			if (Data != null) _logger.Debug($"RamMetricsRepository: GetById({id}) read 1 record."); else _logger.Debug($"RamMetricsRepository: GetById({id}) read NO records.");
			return Data;
		}

		/// <summary>
		/// Подключается к базе данных и получает одну запись по идентификатору
		/// </summary>
		/// <param name="id">идентификатор записи</param>
		/// <returns>Запрошенная запись</returns>
		public RamMetric GetById(int id)
		{
			using var connection = OpenConnection();
			return GetById(connection, id);
		}
	}
}
