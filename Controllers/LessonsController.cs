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
    [Route("Courses/{CourseId:int}/Lessons")]
    [Authorize(Roles = "Student,Teacher")]
    public class LessonsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LessonsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Lessons
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.Lesson.Include(l => l.Course);
        //    return View(await applicationDbContext.ToListAsync());
        //}

        // GET: Lessons/Details/5
        [Route("Details/{LessonId:int}")]
        public async Task<IActionResult> Details([FromRoute]int? CourseId, [FromRoute]int? LessonId)
        {
            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.Id == CourseId);

            if (course == null)
            {

                return NotFound();
            }

            if (LessonId == null || _context.Lesson == null)
            {

                return NotFound();
            }            

            var lesson = await _context.Lesson
                .Include(l => l.Course)
                .FirstOrDefaultAsync(m => m.Id == LessonId);

            if (lesson == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (!CanAccessLesson(currentUser.Id, lesson)) {
                return Forbid();
            }

            ViewData["IsCourseOwner"] = course.TeacherId == currentUser.Id;

            return View(lesson);
        }

        // GET: Lessons/Create
        [Route("Create")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create([FromRoute]int? CourseId)
        {
            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.Id == CourseId);

            if (course == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (course.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            ViewData["CourseId"] = CourseId;
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create([FromRoute] int? CourseId, [Bind("Id,Title,Video,Description,CourseId")] Lesson lesson)
        {
            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.Id == CourseId);

            if (course == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (course.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                _context.Add(lesson);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Courses", new { Id = CourseId });
            }

            ViewData["CourseId"] = CourseId;
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        [Route("Edit/{LessonId:int}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit([FromRoute]int? CourseId, [FromRoute]int? LessonId)
        {
            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.Id == CourseId);

            if (course == null)
            {
                return NotFound();
            }

            if (LessonId == null || _context.Lesson == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lesson.FindAsync(LessonId);
            if (lesson == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (lesson.Course.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            ViewData["CourseId"] = lesson.CourseId;

            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{LessonId:int}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit([FromRoute]int? CourseId, [FromRoute]int? LessonId, [Bind("Id,Title,Video,Description,CourseId")] Lesson lesson)
        {

            if (LessonId == null || _context.Lesson == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var course = await _context.Course.FindAsync(CourseId);

            if (course.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lesson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonExists(lesson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { CourseId = course.Id, LessonId = lesson.Id});
            }
            ViewData["CourseId"] = lesson.CourseId;
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        [Authorize(Roles = "Teacher")]
        [Route("Delete/{LessonId:int}")]
        public async Task<IActionResult> Delete([FromRoute]int? CourseId, [FromRoute]int? LessonId)
        {
            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.Id == CourseId);

            if (course == null)
            {
                return NotFound();
            }
            if (LessonId == null || _context.Lesson == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lesson
                .Include(l => l.Course)
                .FirstOrDefaultAsync(m => m.Id == LessonId);
            if (lesson == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (lesson.Course.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        [Route("Delete/{LessonId:int}")]
        public async Task<IActionResult> DeleteConfirmed([FromRoute]int? CourseId, [FromRoute]int? LessonId)
        {
            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.Id == CourseId);

            if (course == null)
            {
                return NotFound();
            }
            if (LessonId == null || _context.Lesson == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lesson.FindAsync(LessonId);
            if (lesson == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (lesson.Course.TeacherId != currentUser.Id)
            {
                return Forbid();
            }

            if (_context.Lesson == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Lesson'  is null.");
            }

            if (lesson != null)
            {
                _context.Lesson.Remove(lesson);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Courses", new { Id = CourseId });
        }

        private bool LessonExists(int id)
        {
          return _context.Lesson.Any(e => e.Id == id);
        }

        private bool PurchasedCourse(string userdId, int courseId)
        {
            return _context.Enrollment.Where(
                        e => e.StudentId == userdId && e.CourseId == courseId
                    ).Count() > 0;
        }

        private bool CanAccessLesson(string userId, Lesson lesson)
        {
            return (User.IsInRole("Student") && PurchasedCourse(userId, lesson.CourseId)) ||
                lesson.Course.TeacherId == userId;
        }
    }
}
