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
        int parsedUserId = int.Parse(userId);

        // Check if the user has a previous chat history
        var lastChat = await _context.ChatCommunications
                                      .Where(cc => cc.UserId == parsedUserId)
                                      .OrderByDescending(cc => cc.Timestamp)
                                      .FirstOrDefaultAsync();

        Agent agent = null;

        if (lastChat != null)
        {
            // Retrieve the last agent the user interacted with
            var previousAgent = await _context.Agents
                                              .Where(a => a.AgentId == lastChat.AgentId)
                                              .FirstOrDefaultAsync();

            // Check if the previous agent is available
            if (previousAgent != null && IsAgentAvailable(previousAgent.AgentId))
            {
                agent = previousAgent;
            }
        }

        // If no previous agent or the previous agent is unavailable, assign a new agent
        // If no previous agent or the previous agent is unavailable, assign a new agent
        if (agent == null)
        {
            agent = await _context.Agents.FirstOrDefaultAsync(a => a.AgentStatus == true);
        }

        if (agent != null)
        {
            // Set agent status to busy
            await SetAgentStatus(agent.AgentId, false);

            var chat = new UserAndAgentChatCommunication
            {
                UserId = parsedUserId,
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

    private bool IsAgentAvailable(int agentId)
    {
        var agent = _context.Agents
                            .Where(a => a.AgentId == agentId && a.AgentStatus)
                            .FirstOrDefault();

        return agent != null;
    }

    public async Task SetAgentStatus(int agentId, bool status)
    {
        var agent = await _context.Agents
                                  .Where(a => a.AgentId == agentId)
                                  .FirstOrDefaultAsync();

        if (agent != null)
        {
            agent.AgentStatus = status;
            await _context.SaveChangesAsync();
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

        // Notify the user
        await Clients.All.SendAsync("ReceiveMessage", agentId, message, true);
      
    }
    //public async Task LoadChatHistory(string userId)
    //{
    //    var chatHistory = await (from cc in _context.ChatCommunications
    //                             where cc.UserId == Convert.ToInt32(userId)
    //                             orderby cc.Timestamp
    //                             select new
    //                             {
    //                                 cc.UserId,
    //                                 cc.AgentId,
    //                                 cc.Message,
    //                                 cc.IsFromAgent,
    //                                 UserName = _context.Users
    //                                                    .Where(u => u.UserId == cc.UserId)
    //                                                    .Select(u => u.Username)
    //                                                    .FirstOrDefault(),
    //                                 AgentName = _context.Agents
    //                                                     .Where(a => a.AgentId == cc.AgentId)
    //                                                     .Select(a => a.Username)
    //                                                     .FirstOrDefault()
    //                             }).ToListAsync();
    //    await Clients.Caller.SendAsync("ReceiveChatHistory", chatHistory);
    //}

    public async Task LoadChatHistory(int userId, int? agentId)
    {
        var chatHistory = await (from cc in _context.ChatCommunications
                                 join u in _context.Users on cc.UserId equals u.UserId
                                 join a in _context.Agents on cc.AgentId equals a.AgentId
                                 where cc.UserId == userId && cc.AgentId == agentId
                                 orderby cc.Timestamp
                                 select new
                                 {
                                     cc.UserId,
                                     cc.AgentId,
                                     cc.Message,
                                     cc.IsFromAgent,
                                     UserName = u.Username,
                                     AgentName = a.Username
                                 }).ToListAsync();

        await Clients.Caller.SendAsync("ReceiveChatHistory", chatHistory);
    }



}
