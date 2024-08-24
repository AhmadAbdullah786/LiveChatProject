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
                var userIds = HttpContext.Session.GetInt32("UserId") ?? 0;
                var userId = Convert.ToInt32(userIds);
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

        public IActionResult GetUserListForAgent(int agentId)
        {
            var users = _context.ChatCommunications
                                .Where(cc => cc.AgentId == agentId)
                                .Select(cc => new
                                {
                                    cc.User.UserId,
                                    cc.User.Username
                                })
                                .Distinct() // Ensure the list is unique
                                .ToList();

            return Json(users);
        }
        public IActionResult AgentChatBoard()
        {
            var agentId = HttpContext.Session.GetInt32("AgentId") ?? 0;

            if (agentId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get the list of users the agent has communicated with
            var users = _context.ChatCommunications
                                .Where(cc => cc.AgentId == agentId)
                                .Select(cc => new
                                {
                                    cc.UserId,
                                    UserName = _context.Users
                                                       .Where(u => u.UserId == cc.UserId)
                                                       .Select(u => u.Username)
                                                       .FirstOrDefault()
                                })
                                .Distinct() // Ensure no duplicate users
                                .ToList();

            if (users.Count == 0)
            {
                // If no users are found, log or debug
                Console.WriteLine("No users found for agentId: " + agentId);
            }

            // Retrieve the agent's name
            var agentName = _context.Agents
                                    .Where(a => a.AgentId == agentId)
                                    .Select(a => a.Username)
                                    .FirstOrDefault();

            // Pass the agentId, agentName, and user list to the view
            ViewData["AgentId"] = agentId;
            ViewData["AgentName"] = agentName;
            ViewData["Users"] = users;

            return View(); // Render the agent chat board view
        }



        public IActionResult UserChatBoard()
        {
            // Retrieve UserId from session
           
            return View(); // Render agent chat board

        }

    }
}
