using System.ComponentModel.DataAnnotations;

namespace WebTechnologiesTestTask.Model.Dto
{
	public class UserDto
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		[Range(1.00, 100.0 , ErrorMessage = "Age must be between positive")]
		public double Age { get; set; }
	}
}
