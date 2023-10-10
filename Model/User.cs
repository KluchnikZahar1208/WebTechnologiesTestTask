using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebTechnologiesTestTask.Model
{
	[Index(nameof(Email), IsUnique = true)]
	public class User
	{
		[Key]
        public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		[Range(1.00, 100.00, ErrorMessage = "AccountBalance must be between 1 and 100")]
		public double Age { get; set; }
		public ICollection<UserRole>? UserRoles { get; } = new List<UserRole>();
	}
}
