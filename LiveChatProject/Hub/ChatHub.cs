using LiveChatProject.Context;
using LiveChatProject.Entity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private readonly ApplicationDbContext _context;

    public ChatHub(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SendMessageToAgent(string userId, string message)
    {
        // Get a random agent from the database
        var agent = await _context.Agents.FirstOrDefaultAsync();

        if (agent != null)
        {
            var chat = new UserAndAgentChatCommunication
            {
                UserId = int.Parse(userId),
                AgentId = agent.AgentId,
                Message = message,
                IsFromAgent = false,
                Timestamp = DateTime.Now
            };

            _context.ChatCommunications.Add(chat);
            await _context.SaveChangesAsync();

            // Notify the agent
            await Clients.All.SendAsync("ReceiveMessage", userId, message, false);
            
        }
    }

    public async Task SendMessageToUser(string agentId, string userId, string message)
    {
        var chat = new UserAndAgentChatCommunication
        {
            UserId = int.Parse(userId),
            AgentId = int.Parse(agentId),
            Message = message,
            IsFromAgent = true,
            Timestamp = DateTime.Now
        };

        _context.ChatCommunications.Add(chat);
        await _context.SaveChangesAsync();

        await LoadChatHistory(userId);
        // Notify the user
        await Clients.All.SendAsync("ReceiveMessage", agentId, message, true);
      
    }
    public async Task LoadChatHistory(string userId)
    {
        var chatHistory = await (from cc in _context.ChatCommunications
                                 where cc.UserId == Convert.ToInt32(userId)
                                 orderby cc.Timestamp
                                 select new
                                 {
                                     cc.UserId,
                                     cc.AgentId,
                                     cc.Message,
                                     cc.IsFromAgent,
                                     UserName = _context.Users
                                                        .Where(u => u.UserId == cc.UserId)
                                                        .Select(u => u.Username)
                                                        .FirstOrDefault(),
                                     AgentName = _context.Agents
                                                         .Where(a => a.AgentId == cc.AgentId)
                                                         .Select(a => a.Username)
                                                         .FirstOrDefault()
                                 }).ToListAsync();
        await Clients.Caller.SendAsync("ReceiveChatHistory", chatHistory);
    }


}
