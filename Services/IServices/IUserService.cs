using WebTechnologiesTestTask.Model;
using WebTechnologiesTestTask.Model.Dto;

namespace WebTechnologiesTestTask.Services.IServices
{
	public interface IUserService
	{
		Task<User> GetUserByIdAsync(int id);
		Task<IEnumerable<User>> GetAllUsersAsync();
		Task<IEnumerable<string>> GetAllUserRolesByIdAsync(int id);
		Task<IEnumerable<User>> GetAllUsersByRoleIdAsync(int id);
		Task<User> CreateUserAsync(User user);
		Task<User> UpdateUserAsync(User user);
		Task<bool> DeleteUserAsync(int id);
		Task<bool> AddRole(User user, Role role);
		Task<IEnumerable<User>> GetUsersAsync(PageParameters pageParameters, 
			string? name = null, 
			string? email = null, 
			int age = 0,
			string? sort = null);
	}
}
