using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebTechnologiesTestTask.Data;
using WebTechnologiesTestTask.Model;
using WebTechnologiesTestTask.Model.Dto;
using WebTechnologiesTestTask.Services.IServices;

namespace WebTechnologiesTestTask.Services
{
	public class UserService : IUserService
	{
		private readonly AppDbContext _db;
        public UserService(AppDbContext db)
        {
            _db = db;
        }

		public async Task<bool> AddRole(User user, Role role)
		{
			user.UserRoles.Add(new UserRole
			{
				UserId = user.Id,
				User = user,
				Role = role,
				RoleId = role.Id
			});
			await _db.SaveChangesAsync();
			return true;
		}

		public async Task<User> CreateUserAsync(User user)
		{
			try
			{
				_db.Users.Add(user);
				await _db.SaveChangesAsync();
				return user;
			}
			catch (Exception ex)
			{
				return null;
			}
			
		}

		public async Task<bool> DeleteUserAsync(int id)
		{
			var user = await _db.Users.FindAsync(id);
			if (user == null)
			{
				return false;
			}

			_db.Users.Remove(user);
			await _db.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<string>> GetAllUserRolesByIdAsync(int id)
		{
			var query = from userRole in _db.UserRoles
						join role in _db.Roles on userRole.RoleId equals role.Id
						where userRole.UserId == id
						select role.Name;

			var result =await query.ToListAsync();
			return result;
		}

		

		public async Task<IEnumerable<User>> GetAllUsersByRoleIdAsync(int id)
		{
			var query = (from ur in _db.UserRoles
						 join u in _db.Users on ur.UserId equals u.Id
						 where ur.RoleId == 2
						 select new User
						 {
							 Id = u.Id,
							 Name = u.Name,
							 Email = u.Email,
							 Age = u.Age
						 }).Distinct();
			var result = await query.ToListAsync();
			return result;

		}

		public async Task<User> GetUserByIdAsync(int id)
		{
			return await _db.Users.FindAsync(id);
		}

		public async Task<User> UpdateUserAsync(User user)
		{
			try
			{
				_db.ChangeTracker.Clear();
				_db.Users.Update(user);
				await _db.SaveChangesAsync();
				return user;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		
		public async Task<IEnumerable<User>> GetUsersAsync(PageParameters userParameters, 
			string? name = null, string? email = null,int age = 0, string? sort = null)
		{
			var users = await GetAllUsersAsync();

			//filter
			if (!string.IsNullOrEmpty(name))
			{
				users = users.Where(u => u.Name.Contains(name));
			}
			if (!string.IsNullOrEmpty(email))
			{
				users = users.Where(u => u.Email.Contains(email));
			}
			if (age > 0)
			{
				users = users.Where(u => u.Age == age);
			}

			//sort
			if (!string.IsNullOrEmpty(sort))
			{
				var sortParams = sort.Split(',');

				foreach (var sortParam in sortParams)
				{
					switch (sortParam)
					{
						case "id_desc":
							users = users.OrderByDescending(u => u.Id);
							break;
						case "id":
							users = users.OrderBy(u => u.Id);
							break;
						case "name_desc":
							users = users.OrderByDescending(u => u.Name);
							break;
						case "name":
							users = users.OrderBy(u => u.Name);
							break;
						case "email_desc":
							users = users.OrderByDescending(u => u.Email);
							break;
						case "email":
							users = users.OrderBy(u => u.Email);
							break;
						case "age_desc":
							users = users.OrderByDescending(u => u.Age);
							break;
						case "age":
							users = users.OrderBy(u => u.Age);
							break;
					}
				}
			}

			//pagination
			return users
				.Skip((userParameters.PageNumber - 1) * userParameters.PageSize)
				.Take(userParameters.PageSize)
				.ToList();
		}
		public async Task<IEnumerable<User>> GetAllUsersAsync()
		{
			return await _db.Users.ToListAsync();
		}
	}
}
