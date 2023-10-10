using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnologiesTestTask.Model.Dto;
using WebTechnologiesTestTask.Services.IServices;

namespace WebTechnologiesTestTask.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly ILogger<AuthController> _logger;
		protected ResponseDto _responseDto;
		public AuthController(IAuthService authService, ILogger<AuthController> logger)
		{
			_authService = authService;
			_responseDto = new();
			_logger = logger;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
		{
			try
			{
				var loginResponse = await _authService.Login(loginRequestDto);
				if (loginResponse.User == null)
				{
					_responseDto.IsSeccess = false;
					_responseDto.Message = "User data incorrect";
					_logger.LogError("User data incorrect. JwtToken not created, AuthController.Login");
					return BadRequest(_responseDto);
				}
				_responseDto.Result = loginResponse;
				_logger.LogInformation("JwtToken Created, AuthController.Login");
				return new ObjectResult(_responseDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, AuthController.Login");
				throw;
			}
		}
	}
}
