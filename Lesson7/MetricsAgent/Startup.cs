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
	/// ��������� ���� ��������� � ��������
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
					// ��������� ��������� SQLite 
					.AddSQLite()
					// ������������� ������ �����������
					.WithGlobalConnectionString(ConnectionString)
					// ������������, ��� ������ ������ � ����������
					.ScanIn(typeof(Startup).Assembly).For.Migrations()
				).AddLogging(lb => lb
					.AddFluentMigratorConsole());
			// ��������� �������
			services.AddSingleton<IJobFactory, SingletonJobFactory>();
			services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
			// ��������� ���� ������
			services.AddSingleton<CpuMetricJob>();
			services.AddSingleton(new JobSchedule(
				jobType: typeof(CpuMetricJob),
				cronExpression: "0/5 * * * * ?")); // ��������� ������ 5 ������
			services.AddSingleton<DotNetMetricJob>();
			services.AddSingleton(new JobSchedule(
				jobType: typeof(DotNetMetricJob),
				cronExpression: "0/5 * * * * ?")); // ��������� ������ 5 ������
			services.AddSingleton<HddMetricJob>();
			services.AddSingleton(new JobSchedule(
				jobType: typeof(HddMetricJob),
				cronExpression: "0/5 * * * * ?")); // ��������� ������ 5 ������
			services.AddSingleton<NetworkMetricJob>();
			services.AddSingleton(new JobSchedule(
				jobType: typeof(NetworkMetricJob),
				cronExpression: "0/5 * * * * ?")); // ��������� ������ 5 ������
			services.AddSingleton<RamMetricJob>();
			services.AddSingleton(new JobSchedule(
				jobType: typeof(RamMetricJob),
				cronExpression: "0/5 * * * * ?")); // ��������� ������ 5 ������
			services.AddHostedService<QuartzHostedService>();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "API ������� ������ ����� ������",
					Description = "����� ����� �������� � api ������ �������",
				});
				// ��������� ����, �� �������� ����� ����� ����������� ��� Swagger UI
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

			// ��������� ��������
			migrationRunner.MigrateUp();

			// ��������� middleware � �������� ��� ��������� Swagger-��������.
			app.UseSwagger();
			// ��������� middleware ��� ��������� swagger-ui
			// ��������� �������� Swagger JSON (���� ���������� �� ��������������� �������������,
			// �� ������� ����� �������� UI).
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Metrics Agent API");
				c.RoutePrefix = string.Empty;
			});
		}
	}
}
