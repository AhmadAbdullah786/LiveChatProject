using LiveChatProject.Context;
using LiveChatProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace LiveChatProject.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LoginIndex(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsAgent)
                {
                    var agent = _context.Agents.FirstOrDefault(a => a.Username == model.Username && a.Password == model.Password);
                    if (agent != null)
                    {
                        // Store agent details in session
                        HttpContext.Session.SetInt32("AgentId", agent.AgentId);
                        HttpContext.Session.SetBool("IsAgent", true);
                        return RedirectToAction("AgentProfile", "Agent");
                    }
                }
                else
                {
                    var user = _context.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
                    if (user != null)
                    {
                        // Store user details in session
                        HttpContext.Session.SetInt32("UserId", user.UserId);
                        HttpContext.Session.SetBool("IsAgent", false);
                        return RedirectToAction("UserProfile", "User");
                    }
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }
            return View(model);
        }
}
}
