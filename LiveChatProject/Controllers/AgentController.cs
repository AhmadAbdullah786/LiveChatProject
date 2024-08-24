using LiveChatProject.Context;
using Microsoft.AspNetCore.Mvc;

namespace LiveChatProject.Controllers
{
    public class AgentController : Controller

    {
        private readonly ApplicationDbContext _context;

        public AgentController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult AgentProfile()
        {
            var agentId = HttpContext.Session.GetInt32("AgentId");
            if (agentId == null) return RedirectToAction("Index", "Login");

            var agent = _context.Agents.Find(agentId);
            return View(agent);
        }
    }
}
