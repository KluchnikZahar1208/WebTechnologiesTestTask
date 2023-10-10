using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebTechnologiesTestTask.Data;
using WebTechnologiesTestTask.Model;
using WebTechnologiesTestTask.Model.Dto;
using WebTechnologiesTestTask.Services.IServices;

namespace WebTechnologiesTestTask.Services
{
	public class RoleService : IRoleService
	{
		private readonly AppDbContext _db;
        public RoleService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<Role> CreateRoleAsync(Role role)
		{
			_db.Roles.Add(role);
			await _db.SaveChangesAsync();
			return role;
		}

		public async Task<bool> DeleteRoleAsync(int id)
		{
			var role = await _db.Roles.FindAsync(id);
			if (role == null)
			{
				return false;
			}

			_db.Roles.Remove(role);
			await _db.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<Role>> GetAllRolesAsync()
		{
			return await _db.Roles.ToListAsync();
		}

		public async Task<Role> GetRoleByIdAsync(int id)
		{
			return await _db.Roles.FindAsync(id);
		}

		public async Task<IEnumerable<Role>> GetRolesAsync(PageParameters pageParameters, string? name = null, string? sort = null)
		{
			var role = await GetAllRolesAsync();

			//filter
			if (!string.IsNullOrEmpty(name))
			{
				role = role.Where(u => u.Name.Contains(name));
			}
			
			//sort
			if (!string.IsNullOrEmpty(sort))
			{
				var sortParams = sort.Split(',');

				foreach (var sortParam in sortParams)
				{
					switch (sortParam)
					{
						case "Id_desc":
							role = role.OrderByDescending(u => u.Id);
							break;
						case "Id":
							role = role.OrderBy(u => u.Id);
							break;
						case "Name_desc":
							role = role.OrderByDescending(u => u.Name);
							break;
						case "Name":
							role = role.OrderBy(u => u.Name);
							break;
					}
				}
			}

			//pagination
			return role
				.Skip((pageParameters.PageNumber - 1) * pageParameters.PageSize)
				.Take(pageParameters.PageSize)
				.ToList();
		}

		public async Task<Role> UpdateRoleAsync(Role role)
		{
			_db.Roles.Update(role);
			await _db.SaveChangesAsync();
			return role;
		}
	}
}
