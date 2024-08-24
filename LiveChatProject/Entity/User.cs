using System.ComponentModel.DataAnnotations;
namespace LiveChatProject.Entity
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = "";


        [Required]
        [StringLength(100)]
        public string Email { get; set; } = "";
    }
}
