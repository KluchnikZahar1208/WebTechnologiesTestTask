using AutoMapper;
using WebTechnologiesTestTask.Model;
using WebTechnologiesTestTask.Model.Dto;

namespace WebTechnologiesTestTask
{
	public class MappingProfile : Profile
	{
		public static MapperConfiguration RegisterMaps()
		{
			var mappingConfig = new MapperConfiguration(config =>
			{
				config.CreateMap<UserDto, User>().ReverseMap();
				config.CreateMap<RoleDto, Role>().ReverseMap();
			});
			return mappingConfig;
		}
	}
}
