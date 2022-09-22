using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Learning.Data;
using E_Learning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace E_Learning.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var courses = _context.Course.Include(c => c.Teacher).ToList();

            if (_signInManager.IsSignedIn(User))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                ViewData["IsTeacher"] = await _userManager.IsInRoleAsync(currentUser, "Teacher");
            }
            else
            {
                ViewData["IsTeacher"] = false;
            }

            return View(courses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Course == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);

            ViewData["TeacherUserName"] = (await _userManager.FindByIdAsync(course.TeacherId)).NormalizedUserName;

            // Check if user is the owner
            var currentUser = await _userManager.GetUserAsync(User);

            ViewData["CanEditCourse"] = currentUser != null && course.TeacherId == currentUser.Id;

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Teacher")]
        public IActionResult Create()
        {
            //ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["CurrentUserId"] = _userManager.GetUserId(User);
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Price,TeacherId")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Course.Add(course);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            //ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["CurrentUserId"] = _userManager.GetUserId(User);
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Course == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            // Check if user is the owner

            if (course.TeacherId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            //ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id", course.TeacherId);
            ViewData["CurrentUserId"] = _userManager.GetUserId(User);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,TeacherId")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            // Check if user is the owner

            if (course.TeacherId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Course.Update(course);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            //ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id", course.TeacherId);
            ViewData["CurrentUserId"] = _userManager.GetUserId(User);
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Course == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            // Check if user is the owner

            if (course.TeacherId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Course == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Course'  is null.");
            }
            var course = await _context.Course.FindAsync(id);
            if (course != null && course.TeacherId == _userManager.GetUserId(User))
            {
                _context.Course.Remove(course);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
          return _context.Course.Any(e => e.Id == id);
        }
    }
}
