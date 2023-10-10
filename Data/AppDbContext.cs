using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebTechnologiesTestTask.Model;

namespace WebTechnologiesTestTask.Data
{
	public class AppDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Role>().HasData(
				new Role { Id = 1, Name = "User", },
				new Role { Id = 2, Name = "Admin" },
				new Role { Id = 3, Name = "Support" },
				new Role { Id = 4, Name = "SuperAdmin" }
				);

			modelBuilder.Entity<UserRole>()
				.HasKey(ur => new { ur.UserId, ur.RoleId });
			modelBuilder.Entity<UserRole>()
				.HasOne(ur => ur.User)
				.WithMany(u => u.UserRoles)
				.HasForeignKey(ur => ur.UserId);

			modelBuilder.Entity<UserRole>()
				.HasOne(ur => ur.Role)
				.WithMany(r => r.UserRoles)
				.HasForeignKey(ur => ur.RoleId);

			
		}
	}
}
