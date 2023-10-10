using Microsoft.AspNetCore.Identity;
using System.Drawing.Drawing2D;
using WebTechnologiesTestTask.Data;
using WebTechnologiesTestTask.Model.Dto;
using WebTechnologiesTestTask.Services.IServices;

namespace WebTechnologiesTestTask.Services
{
	public class AuthService : IAuthService
	{

		private readonly AppDbContext _db;
		private readonly IJwtTokenGenerator _jwtTokenGenerator;
		public AuthService(AppDbContext db,
			IJwtTokenGenerator jwtTokenGenerator)
		{
			_db = db;
			_jwtTokenGenerator = jwtTokenGenerator;
		}

		public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
		{
			var user = _db.Users.FirstOrDefault(u => u.Email.ToLower() ==
				loginRequestDto.Email.ToLower());
			if (user == null)
			{
				return new LoginResponseDto()
				{
					Token = ""
				};
			}
			var token = _jwtTokenGenerator.GenerateToken(user);
			UserDto userDto = new UserDto()
			{
				Email = user.Email,
			};
			LoginResponseDto loginResponseDto = new LoginResponseDto()
			{
				Token = token
			};
			return loginResponseDto;
		}
	}
}
