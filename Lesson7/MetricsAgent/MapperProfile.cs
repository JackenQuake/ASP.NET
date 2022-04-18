using AutoMapper;
using MetricsAgent.Models;
using MetricsAgent.Responses;

namespace MetricsAgent
{
	/// <summary>
	/// Описание преобразования типов для AutoMapper
	/// </summary>
	public class MapperProfile : Profile
	{
		public MapperProfile()
		{
			// Добавлять сопоставления в таком стиле надо для всех объектов 
			CreateMap<CpuMetric, CpuMetricDto>();
			CreateMap<DotNetMetric, DotNetMetricDto>();
			CreateMap<HddMetric, HddMetricDto>();
			CreateMap<NetworkMetric, NetworkMetricDto>();
			CreateMap<RamMetric, RamMetricDto>();
		}
	}
}
