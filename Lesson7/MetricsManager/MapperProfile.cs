using AutoMapper;
using MetricsManager.Models;
using MetricsManager.Responses;

namespace MetricsManager
{
	public class MapperProfile : Profile
	{
		public MapperProfile()
		{
			// Добавлять сопоставления в таком стиле надо для всех объектов 
			CreateMap<AgentInfo, AgentInfoDto>();
			CreateMap<CpuMetric, CpuMetricDto>();
			CreateMap<DotNetMetric, DotNetMetricDto>();
			CreateMap<HddMetric, HddMetricDto>();
			CreateMap<NetworkMetric, NetworkMetricDto>();
			CreateMap<RamMetric, RamMetricDto>();
			CreateMap<CpuMetricDto, CpuMetric>();
			CreateMap<DotNetMetricDto, DotNetMetric>();
			CreateMap<HddMetricDto, HddMetric>();
			CreateMap<NetworkMetricDto, NetworkMetric>();
			CreateMap<RamMetricDto, RamMetric>();
		}
	}
}
