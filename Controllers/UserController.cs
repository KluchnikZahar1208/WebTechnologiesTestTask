using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WebTechnologiesTestTask.Model;
using WebTechnologiesTestTask.Model.Dto;
using WebTechnologiesTestTask.Services;
using WebTechnologiesTestTask.Services.IServices;

namespace WebTechnologiesTestTask.Controllers
{
	[Route("api/user")]
	[ApiController]
	[Authorize]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IRoleService _roleService;
		private readonly ILogger<UserController> _logger;
		private readonly IMapper _mapper;
		public UserController(IUserService userService, IRoleService roleService, IMapper mapper, ILogger<UserController> logger)
		{
			_userService = userService;
			_roleService = roleService;
			_mapper = mapper;
			_logger = logger;
		}
		[HttpGet]
		public async Task<IActionResult> GetAllUsers()
		{
			try
			{
				var users = await _userService.GetAllUsersAsync();
				_logger.LogInformation("Get all users, UserController.GetAllUsers");
				return new ObjectResult(users);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, UserController.GetAllUsers");
				throw;
			}
		}

		[HttpPost]
		public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
		{
			try
			{
				if (userDto == null)
				{
					_logger.LogError("UserDto is null from body, UserController.CreateUser");
					return BadRequest();
				}
				var user = _mapper.Map<User>(userDto);
				if (ModelState.IsValid)
				{
					var createdUser = await _userService.CreateUserAsync(user);

					if (createdUser == null)
					{
						_logger.LogError("User email already exists, UserController.CreateUser");
						return BadRequest("Email already exists");
					}
					_logger.LogInformation("User was created, UserController.CreateUser");
					return CreatedAtRoute(new { id = createdUser.Id }, createdUser);
				}
				_logger.LogError("User data is invalid, UserController.CreateUser");
				return BadRequest("Invalid data");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, UserController.CreateUser");
				throw;
			}

		}
		[HttpGet("{id}")]
		public async Task<ActionResult<User>> GetUserById(int id)
		{
			try
			{
				var user = await _userService.GetUserByIdAsync(id);
				if (user == null)
				{
					_logger.LogError($"User by id = {id} not found, UserController.GetUserById");
					return NotFound();
				}
				_logger.LogInformation($"Get user id = {id}, UserController.GetUserById");
				return new ObjectResult(user);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, UserController.GetUserById");
				throw;
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUser(int id, User user)
		{
			try
			{
				if (id != user.Id || user == null)
				{
					_logger.LogError("Wrong id or user is null, UserController.GetUserById");
					return BadRequest("User is null");
				}
				if (await _userService.GetUserByIdAsync(id) == null)
				{
					_logger.LogError($"User by id = {id} not found, UserController.UpdateUser");
					return NotFound();
				}
				if (ModelState.IsValid)
				{
					var result = await _userService.UpdateUserAsync(user);
					if (result == null)
					{
						_logger.LogError("User email already exists, UserController.UpdateUser");
						return BadRequest("Email already exists");
					}
					return NoContent();
				}
				_logger.LogError("User data is invalid, UserController.UpdateUser");
				return BadRequest("Invalid data");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, UserController.UpdateUser");
				throw;
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteUser(int id)
		{
			try
			{
				var result = await _userService.DeleteUserAsync(id);
				if (!result)
				{
					_logger.LogError($"User by id = {id} not found, UserController.DeleteUser");
					return NotFound();
				}
				_logger.LogInformation($"Deleted user with id = {id} on UserController.DeleteUser");
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, UserController.DeleteUser");
				throw;
			}
		}
		[HttpPost]
		[Route("AddRole")]
		public async Task<IActionResult> AddRole(int userId, int roleId)
		{
			try
			{
				var user = await _userService.GetUserByIdAsync(userId);
				var role = await _roleService.GetRoleByIdAsync(roleId);

				if (user == null || role == null)
				{
					_logger.LogError("Wrong id on user or role, UserController.AddRole");
					return NotFound();
				}
				await _userService.AddRole(user, role);
				_logger.LogInformation($"Role with id = {roleId} added to user with id = {userId}, UserController.AddRole");
				return new NoContentResult();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, UserController.AddRole.");
				throw;
			}
		}
		[HttpGet]
		[Route("GetUserRoleById")]
		public async Task<IActionResult> GetUserRoleById(int id)
		{
			try
			{
				if (await _userService.GetUserByIdAsync(id) == null)
				{
					_logger.LogError($"User with id = {id} not found, UserController.GetUserRoleById");
					return NotFound();
				}
				var roles = await _userService.GetAllUserRolesByIdAsync(id);
				_logger.LogInformation($"Get all roles of user with id = {id}, UserController.GetUserRoleById");
				return new ObjectResult(roles);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, UserController.GetUserRoleById");
				throw;
			}
		}

		[HttpGet]
		[Route("GetUsersByRoleId")]
		public async Task<IActionResult> GetUsersByRoleId(int id)
		{
			try
			{
				if (await _roleService.GetRoleByIdAsync(id) == null)
				{
					_logger.LogError($"Role with id = {id} not found, UserController.GetUsersByRoleId");
					return NotFound();
				}
				var users = await _userService.GetAllUsersByRoleIdAsync(id);
				var result = users.Select(u => _mapper.Map<UserDto>(u)).ToList();
				_logger.LogInformation($"Get all users with role with id = {id}, UserController.GetUsersByRoleId");
				return new ObjectResult(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, UserController.GetUsersByRoleId");
				throw;
			}
		}

		[HttpGet("GetUsers")]
		public async Task<IActionResult> GetUsers([FromQuery] PageParameters pageParameters,
			string? name = null, string? email = null, int age = 0, string? sort = null)
		{
			try
			{
				var users = await _userService.GetUsersAsync(pageParameters, name, email, age, sort);
				_logger.LogInformation($"Get {users.Count()} users, UserController.GetUsers");
				return new ObjectResult(users);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, UserController.GetUsers");
				throw;
			}
		}

		[HttpGet("roles/GetRoles")]
		public async Task<IActionResult> GetRoles([FromQuery] PageParameters pageParameters,
			string? name = null, string? sort = null)
		{
			try
			{
				var roles = await _roleService.GetRolesAsync(pageParameters, name, sort?.ToLower());
				_logger.LogInformation($"Get {roles.Count()} roles, UserController.GetRoles");
				return new ObjectResult(roles);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error, UserController.GetRoles");
				throw;
			}
		}
	}
}
