using LiveChatProject.Entity;
using Microsoft.EntityFrameworkCore;

namespace LiveChatProject.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<UserAndAgentChatCommunication> ChatCommunications { get; set; }
    }
}
