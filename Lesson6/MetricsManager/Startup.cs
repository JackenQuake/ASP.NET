using AutoMapper;
using MetricsManager.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentMigrator.Runner;
using System;
using Quartz.Spi;
using Quartz;
using Quartz.Impl;
using MetricsManager.Jobs;
using MetricsManager.Client;
using Polly;

namespace MetricsManager
{
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
			// Добавляем базу данных
			services.AddSingleton<IAgentInfoRepository>(s => new AgentInfoRepository(ConnectionString));
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
			// Добавляем HTTP клиента
			services.AddHttpClient<IMetricsAgentClient, MetricsAgentClient>().AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(1000)));
			// Добавляем контроллеры
			services.AddControllers();
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

			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
			});

			// Запускаем миграции
			migrationRunner.MigrateUp();
		}
	}
}
