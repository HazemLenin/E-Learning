using E_Learning.Data;
using E_Learning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Learning.Controllers
{
    [Authorize(Roles = "Student,Teacher")]
    public class WalletController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WalletController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // We need current user to get his wallet data
            var currentUser = await _userManager.GetUserAsync(User);
            ViewData["WalletValue"] = currentUser.Wallet;
            return View();
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Charge()
        {
            // Charge wallet logic
            var currentUser = await _userManager.GetUserAsync(User);
            currentUser.Wallet += 100;
            _context.SaveChanges();
            return Redirect(nameof(Index));
        }
    }
}
