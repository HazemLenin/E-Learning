using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using E_Learning.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Learning.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            // Get all users except logged in users
            var currentUser = await _userManager.GetUserAsync(User);
            List<ApplicationUser> users = await _userManager
                .Users
                .Where(user => user.Id != currentUser.Id)
                .ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = _roleManager.Roles;
            var userRoles = await _userManager.GetRolesAsync(user);
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            foreach (var role in roles)
            {
                selectListItems.Add(new SelectListItem() {
                    Text = role.Name,
                    Value = role.Id,
                    Selected = userRoles.Contains(role.Name)
                });
            }

            ViewBag.Roles = selectListItems;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Details(string Id, List<string> rolesIds)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            // Removing all user roles
            var userRolesNames = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRolesNames);

            // Add selected roles
            List<String> selectedRoles = new();
            foreach (string roleId in rolesIds)
            {
                selectedRoles.Add((await _roleManager.FindByIdAsync(roleId)).Name);
            }

            await _userManager.AddToRolesAsync(
                user,
                selectedRoles
            );

            ViewBag.Succeeded = true;
            var roles = _roleManager.Roles;
            var userRoles = await _userManager.GetRolesAsync(user);
            List<SelectListItem> selectListItems = new();

            foreach (var role in roles)
            {
                selectListItems.Add(new SelectListItem()
                {
                    Text = role.Name,
                    Value = role.Id,
                    Selected = userRoles.Contains(role.Name)
                });
            }

            ViewBag.Roles = selectListItems;
            return View(user);
        }
    }
}
