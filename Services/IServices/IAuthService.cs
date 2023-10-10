using WebTechnologiesTestTask.Model.Dto;

namespace WebTechnologiesTestTask.Services.IServices
{
	public interface IAuthService
	{
		Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
	}
}
