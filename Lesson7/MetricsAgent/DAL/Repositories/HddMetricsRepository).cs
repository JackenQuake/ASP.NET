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
	/// Репозиторий метрики HDD (свободное пространство на диске C:)
	/// </summary>
	public class HddMetricsRepository : IHddMetricsRepository
	{
		private readonly NLog.Logger _logger;
		private readonly string ConnectionString;

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="ConnectionString">Параметры подключения к базе данных</param>
		public HddMetricsRepository(string ConnectionString)
		{
			this.ConnectionString = ConnectionString;
			_logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
			_logger.Debug($"HddMetricsRepository created with {ConnectionString}.");
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

		/// <summary>
		/// Подключается к базе данных и создает новую запись
		/// </summary>
		/// <param name="item">Новые данные для записи в базу данных</param>
		public void Create(HddMetric item)
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
			int res = connection.Execute("DELETE FROM hddmetrics WHERE id=@id", new { id = id });
			_logger.Debug($"HddMetricsRepository: delete {id} result {res}.");
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

		/// <summary>
		/// Подключается к базе данных и изменяет существующую запись
		/// </summary>
		/// <param name="item">Новые данные для записи в базу данных</param>
		public void Update(HddMetric item)
		{
			using var connection = OpenConnection();
			Update(connection, item);
		}

		/// <summary>
		/// Получает все метрики из базы данных для уже созданного подключения к БД
		/// </summary>
		/// <param name="connection">Подключение к базе данных</param>
		/// <returns>Список всех сохранённых метрик</returns>
		public IList<HddMetric> GetAll(SQLiteConnection connection)
		{
			// Читаем, используя Query, и в шаблон подставляем тип данных,
			// объект которого Dapper, он сам заполнит его поля
			// в соответствии с названиями колонок
			var returnList = connection.Query<HddMetric>("SELECT * FROM hddmetrics").ToList();
			_logger.Debug($"HddMetricsRepository: GetAll read {returnList.Count} records.");
			return returnList;
		}

		/// <summary>
		/// Подключается к базе данных и получает все метрики
		/// </summary>
		/// <returns>Список всех сохранённых метрик</returns>
		public IList<HddMetric> GetAll()
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
		public IList<HddMetric> GetByTimePeriod(SQLiteConnection connection, TimeSpan from, TimeSpan to)
		{

			var returnList = connection.Query<HddMetric>("SELECT * FROM hddmetrics WHERE time BETWEEN @from AND @to", new { from = from.TotalSeconds, to = to.TotalSeconds }).ToList();
			_logger.Debug($"HddMetricsRepository: GetByTimePeriod({from.TotalSeconds}, {to.TotalSeconds}) read {returnList.Count} records.");
			return returnList;
		}

		/// <summary>
		/// Подключается к базе данных и получает метрики на заданном диапазоне времени
		/// </summary>
		/// <param name="from">начальная метрика времени в секундах с 01.01.1970</param>
		/// <param name="to">конечная метрика времени в секундах с 01.01.1970</param>
		/// <returns>Список метрик, сохранённых в заданном диапазоне времени</returns>
		public IList<HddMetric> GetByTimePeriod(TimeSpan from, TimeSpan to)
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
		public HddMetric GetById(SQLiteConnection connection, int id)
		{
			var Data = connection.QuerySingle<HddMetric>("SELECT Id, Time, Value FROM hddmetrics WHERE id=@id", new { id = id });
			if (Data != null) _logger.Debug($"HddMetricsRepository: GetById({id}) read 1 record."); else _logger.Debug($"HddMetricsRepository: GetById({id}) read NO records.");
			return Data;
		}

		/// <summary>
		/// Подключается к базе данных и получает одну запись по идентификатору
		/// </summary>
		/// <param name="id">идентификатор записи</param>
		/// <returns>Запрошенная запись</returns>
		public HddMetric GetById(int id)
		{
			using var connection = OpenConnection();
			return GetById(connection, id);
		}
	}
}
