using AutoMapper;
using FluentMigrator.Runner;
using MetricsAgent.DAL;
using MetricsAgent.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.IO;
using System.Reflection;

namespace MetricsAgent
{
	/// <summary>
	/// Настройка всех подсистем и сервисов
	/// </summary>
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }
		private const string ConnectionString = @"Data Source=metrics.db; Version=3;";

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddSingleton<ICpuMetricsRepository>(s => new CpuMetricsRepository(ConnectionString));
			services.AddSingleton<IDotNetMetricsRepository>(s => new DotNetMetricsRepository(ConnectionString));
			services.AddSingleton<IHddMetricsRepository>(s => new HddMetricsRepository(ConnectionString));
			services.AddSingleton<INetworkMetricsRepository>(s => new NetworkMetricsRepository(ConnectionString));
			services.AddSingleton<IRamMetricsRepository>(s => new RamMetricsRepository(ConnectionString));
			var mapperConfiguration = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile()));
			var mapper = mapperConfiguration.CreateMapper();
			services.AddSingleton(mapper);
			services.AddFluentMigratorCore()
				.ConfigureRunner(rb => rb
					// Добавляем поддержку SQLite 
					.AddSQLite()
					// Устанавливаем строку подключения
					.WithGlobalConnectionString(ConnectionString)
					// Подсказываем, где искать классы с миграциями
					.ScanIn(typeof(Startup).Assembly).For.Migrations()
				).AddLogging(lb => lb
					.AddFluentMigratorConsole());
			// Добавляем сервисы
			services.AddSingleton<IJobFactory, SingletonJobFactory>();
			services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
			// Добавляем наши задачи
			services.AddSingleton<CpuMetricJob>();
			services.AddSingleton(new JobSchedule(
				jobType: typeof(CpuMetricJob),
				cronExpression: "0/5 * * * * ?")); // Запускать каждые 5 секунд
			services.AddSingleton<DotNetMetricJob>();
			services.AddSingleton(new JobSchedule(
				jobType: typeof(DotNetMetricJob),
				cronExpression: "0/5 * * * * ?")); // Запускать каждые 5 секунд
			services.AddSingleton<HddMetricJob>();
			services.AddSingleton(new JobSchedule(
				jobType: typeof(HddMetricJob),
				cronExpression: "0/5 * * * * ?")); // Запускать каждые 5 секунд
			services.AddSingleton<NetworkMetricJob>();
			services.AddSingleton(new JobSchedule(
				jobType: typeof(NetworkMetricJob),
				cronExpression: "0/5 * * * * ?")); // Запускать каждые 5 секунд
			services.AddSingleton<RamMetricJob>();
			services.AddSingleton(new JobSchedule(
				jobType: typeof(RamMetricJob),
				cronExpression: "0/5 * * * * ?")); // Запускать каждые 5 секунд
			services.AddHostedService<QuartzHostedService>();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "API сервиса агента сбора метрик",
					Description = "Здесь можно поиграть с api нашего сервиса",
				});
				// Указываем файл, из которого будем брать комментарии для Swagger UI
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMigrationRunner migrationRunner)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			// Запускаем миграции
			migrationRunner.MigrateUp();

			// Включение middleware в пайплайн для обработки Swagger-запросов.
			app.UseSwagger();
			// включение middleware для генерации swagger-ui
			// указываем эндпоинт Swagger JSON (куда обращаться за сгенерированной спецификацией,
			// по которой будет построен UI).
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Metrics Agent API");
				c.RoutePrefix = string.Empty;
			});
		}
	}
}
