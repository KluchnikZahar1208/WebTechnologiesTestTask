using WebTechnologiesTestTask.Model;

namespace WebTechnologiesTestTask.Services.IServices
{
	public interface IJwtTokenGenerator
	{
		string GenerateToken(User user);
	}
}

