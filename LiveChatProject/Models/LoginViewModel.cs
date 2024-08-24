using System.ComponentModel.DataAnnotations;
namespace LiveChatProject.Models
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100)]
        public string Password { get; set; } = "";

        public bool IsAgent { get; set; } // Checkbox to distinguish between User and Agent
    }
}
