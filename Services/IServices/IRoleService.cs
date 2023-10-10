using WebTechnologiesTestTask.Model;
using WebTechnologiesTestTask.Model.Dto;

namespace WebTechnologiesTestTask.Services.IServices
{
	public interface IRoleService
	{
		Task<Role> GetRoleByIdAsync(int id);
		Task<IEnumerable<Role>> GetAllRolesAsync();
		Task<Role> CreateRoleAsync(Role role);
		Task<Role> UpdateRoleAsync(Role role);
		Task<bool> DeleteRoleAsync(int id);
		Task<IEnumerable<Role>> GetRolesAsync(PageParameters pageParameters,
			string? name = null,
			string? sort = null);
	}
}
