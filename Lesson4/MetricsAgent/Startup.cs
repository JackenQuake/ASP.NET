using AutoMapper;
using MetricsAgent.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MetricsAgent
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddScoped<ICpuMetricsRepository, CpuMetricsRepository>();
			services.AddScoped<IDotNetMetricsRepository, DotNetMetricsRepository>();
			services.AddScoped<IHddMetricsRepository, HddMetricsRepository>();
			services.AddScoped<INetworkMetricsRepository, NetworkMetricsRepository>();
			services.AddScoped<IRamMetricsRepository, RamMetricsRepository>();
			new CpuMetricsRepository().CreateDatabase(true);
			new DotNetMetricsRepository().CreateDatabase(true);
			new HddMetricsRepository().CreateDatabase(true);
			new NetworkMetricsRepository().CreateDatabase(true);
			new RamMetricsRepository().CreateDatabase(true);
			var mapperConfiguration = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile()));
			var mapper = mapperConfiguration.CreateMapper();
			services.AddSingleton(mapper);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
		}
	}
}