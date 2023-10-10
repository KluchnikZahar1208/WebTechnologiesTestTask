using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebTechnologiesTestTask.Model;
using WebTechnologiesTestTask.Model.Dto;
using WebTechnologiesTestTask.Services;
using WebTechnologiesTestTask.Services.IServices;

namespace WebTechnologiesTestTask.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class RoleController : ControllerBase
	{
		private readonly IRoleService _roleService;
		private readonly IMapper _mapper;
		private readonly ILogger<RoleController> _logger;
		public RoleController(IRoleService roleService, IMapper mapper, ILogger<RoleController> logger)
		{
			_roleService = roleService;
			_mapper = mapper;
			_logger = logger;
		}
		[HttpGet]
		public async Task<IActionResult> GetAllRoles()
		{
			try
			{
				var roles = await _roleService.GetAllRolesAsync();
				_logger.LogInformation("Get all roles, RoleController.GetAllRoles");
				return new ObjectResult(roles);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, RoleController.GetAllRoles");
				throw;
			}
		}

		[HttpPost]
		public async Task<ActionResult<Role>> CreateRole([FromBody]RoleDto roleDto)
		{
			try
			{
				var role = _mapper.Map<Role>(roleDto);
				var createdRole = await _roleService.CreateRoleAsync(role);
				_logger.LogInformation("Role was created, RoleController.CreateRole");
				return CreatedAtRoute(new { id = createdRole.Id }, createdRole);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, RoleController.CreateRole");
				throw;
			}
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<Role>> GetRoleById(int id)
		{
			try
			{
				var role = await _roleService.GetRoleByIdAsync(id);
				if (role == null)
				{
					_logger.LogError($"Role by id = {id} not found, RoleController.GetRoleById");
					return NotFound();
				}
				_logger.LogInformation($"Get role id = {id}, RoleController.GetRoleById");
				return new ObjectResult(role);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, RoleController.GetRoleById");
				throw;
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateRole(int id, Role role)
		{
			try
			{
				if (id != role.Id)
				{
					_logger.LogError("Wrong id, RoleController.GetRoleById");
					return BadRequest();
				}
				if (await _roleService.GetRoleByIdAsync(id) == null)
				{
					_logger.LogError($"Role by id = {id} not found, RoleController.UpdateRole");
					return NotFound();
				}
				await _roleService.UpdateRoleAsync(role);
				_logger.LogInformation($"Update role on id = {id}, RoleController.UpdateRole");
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, RoleController.UpdateRole");
				throw;
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteRole(int id)
		{
			try
			{
				var result = await _roleService.DeleteRoleAsync(id);
				if (!result)
				{
					_logger.LogError("Wrong id on RoleController.DeleteRole");
					return NotFound();
				}
				_logger.LogInformation($"Deleted role with id = {id} on RoleController.DeleteRole");
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error,  RoleController.DeleteRole");
				throw;
			}
		}
	}
}
