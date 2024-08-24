using System.ComponentModel.DataAnnotations;

namespace LiveChatProject.Entity
{
    public class UserAndAgentChatCommunication
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AgentId { get; set; }
        public string Message { get; set; } = "";
        public bool IsFromAgent { get; set; } // True if the message is from the agent, false if from the user
        public DateTime Timestamp { get; set; }

        public User User { get; set; }
        public Agent Agent { get; set; }
    }
}
