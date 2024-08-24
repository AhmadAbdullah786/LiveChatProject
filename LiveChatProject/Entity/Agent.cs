using System.ComponentModel.DataAnnotations;
namespace LiveChatProject.Entity
{
    public class Agent
    {
        [Key]
        public int AgentId { get; set; }

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
