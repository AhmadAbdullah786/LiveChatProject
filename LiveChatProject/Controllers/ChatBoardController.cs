using LiveChatProject.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.AccessControl;

namespace LiveChatProject.Controllers
{
    public class ChatBoardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatBoardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var isAgent = HttpContext.Session.GetBool("IsAgent");

            if (isAgent)
            {
                var agentId = HttpContext.Session.GetInt32("AgentId") ?? 0;

                // Retrieve the related userId and username
                var userInfo = _context.ChatCommunications
                                       .Where(cc => cc.AgentId == agentId)
                                       .Select(cc => new { cc.UserId}) // Assuming User entity has a UserName property
                                       .FirstOrDefault();

                var userId = userInfo?.UserId ?? 0;
                var userName = _context.Users.Where(u => u.UserId == userInfo.UserId).Select(u => u.Username).FirstOrDefault();

                var agentName = _context.Agents
                                        .Where(a => a.AgentId == agentId)
                                        .Select(a => a.Username) // Assuming Agent entity has an AgentName property
                                        .FirstOrDefault();

                // Pass the agentId, userId, userName, and agentName to the view
                ViewData["AgentId"] = agentId;
                ViewData["UserId"] = userId;
                ViewData["UserName"] = userName;
                ViewData["AgentName"] = agentName;
                return View("AgentChatBoard"); // Render agent chat board
            }
            else
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

                var userName = _context.Users
                                       .Where(u => u.UserId == userId)
                                       .Select(u => u.Username) // Assuming User entity has a UserName property
                                       .FirstOrDefault();

                // Pass the userId and userName to the view
                ViewData["UserId"] = userId;
                ViewData["UserName"] = userName;
                return View("UserChatBoard"); // Render user chat board
            }
        }

        public IActionResult AgentChatBoard()
        {
            return View(); // Render agent chat board
            
        }
        public IActionResult UserChatBoard()
        {
            // Retrieve UserId from session
           
            return View(); // Render agent chat board

        }
    }
}
