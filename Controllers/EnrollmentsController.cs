using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Learning.Data;
using E_Learning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace E_Learning.Controllers
{
    [Route("Courses/{CourseId:int}/Enrollments")]
    [Authorize]
    public class EnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrollmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Enrollments
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var applicationDbContext = User.IsInRole("Teacher") ? // Load teacher's courses enrollments
                _context.Enrollment.Include(e => e.Course).Include(e => e.Student).Where(c => c.Course.TeacherId == currentUser.Id)
                : // Load all application enrollments for admins
                _context.Enrollment.Include(e => e.Course).Include(e => e.Student);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Enrollments/Details/5
        //[Route("Details/{EnrollmentId}")]
        //public async Task<IActionResult> Details([FromRoute]int CourseId, [FromRoute]int EnrollmentId)
        //{
        //    if ((CourseId == null || _context.Course == null) && (EnrollmentId == null || _context.Enrollment == null))
        //    {
        //        return NotFound();
        //    }

        //    var enrollment = await _context.Enrollment
        //        .Include(e => e.Course)
        //        .Include(e => e.Student)
        //        .FirstOrDefaultAsync(m => m.Id == EnrollmentId);
        //    if (enrollment == null)
        //    {
        //        return NotFound();
        //    }

        //    var course = await _context.Course
        //        .FirstOrDefaultAsync(m => m.Id == CourseId);
        //    if (course == null)
        //    {
        //        return NotFound();
        //    }

        //    var currentUser = await _userManager.GetUserAsync(User);

        //    if
        //    (
        //        (User.IsInRole("Student") && enrollment.StudentId != currentUser.Id) || // User is a student but he is trying to access others enrollments
        //        (User.IsInRole("Teacher") && enrollment.Course.TeacherId != currentUser.Id) // User is a teacher but he is trying to access other teacher course enrollment
        //    )
        //    {
        //        return Forbid();
        //    }

        //    return View(enrollment);
        //}

        // GET: Enrollments/Create
        [Route("Create")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Create([FromRoute]int CourseId)
        {
            var course = await _context.Course
                .Include(e => e.Teacher)
                .FirstOrDefaultAsync(m => m.Id == CourseId);
            ViewBag.Course = course;
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if user already enrolled this course
            if (PurchasedCourse(currentUser.Id, course.Id))
            {
                return Forbid();
            }

            ViewBag.StudentId = currentUser.Id;
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Create([FromRoute] int CourseId, [Bind("Id,Cost,StudentId,CourseId")] Enrollment enrollment)
        {
            var course = await _context.Course
                .Include(e => e.Teacher)
                .FirstOrDefaultAsync(m => m.Id == CourseId);
            var currentUser = await _userManager.GetUserAsync(User);
            
            // Check if user already enrolled this course
            if (PurchasedCourse(currentUser.Id, course.Id))
            {
                return Forbid();
            }

            ViewBag.StudentId = currentUser.Id;
            if (ModelState.IsValid)
            {
                if (course.Price > currentUser.Wallet)
                {
                    TempData["ErrorMessage"] = "لا توجد لديك وحدات كافية";
                } else
                {
                    _context.Add(enrollment);
                    currentUser.Wallet -= course.Price;
                    course.Teacher.Wallet += course.Price;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "تم شراء المسار بنجاح";
                    return RedirectToAction("Details", "Courses", new { Id = CourseId });
                }
            }
            ViewBag.Course = course;
            return View();
        }

        // GET: Enrollments/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Enrollment == null)
        //    {
        //        return NotFound();
        //    }

        //    var enrollment = await _context.Enrollment.FindAsync(id);
        //    if (enrollment == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Id", enrollment.CourseId);
        //    ViewData["StudentId"] = new SelectList(_context.Users, "Id", "Id", enrollment.StudentId);
        //    return View(enrollment);
        //}

        //// POST: Enrollments/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Cost,StudentId,CourseId")] Enrollment enrollment)
        //{
        //    if (id != enrollment.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(enrollment);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!EnrollmentExists(enrollment.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Id", enrollment.CourseId);
        //    ViewData["StudentId"] = new SelectList(_context.Users, "Id", "Id", enrollment.StudentId);
        //    return View(enrollment);
        //}

        //// GET: Enrollments/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Enrollment == null)
        //    {
        //        return NotFound();
        //    }

        //    var enrollment = await _context.Enrollment
        //        .Include(e => e.Course)
        //        .Include(e => e.Student)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (enrollment == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(enrollment);
        //}

        //// POST: Enrollments/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Enrollment == null)
        //    {
        //        return Problem("Entity set 'ApplicationDbContext.Enrollment'  is null.");
        //    }
        //    var enrollment = await _context.Enrollment.FindAsync(id);
        //    if (enrollment != null)
        //    {
        //        _context.Enrollment.Remove(enrollment);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool EnrollmentExists(int id)
        {
          return _context.Enrollment.Any(e => e.Id == id);
        }

        private bool PurchasedCourse(string userdId, int courseId)
        {
            return _context.Enrollment.Where(
                        e => e.StudentId == userdId && e.CourseId == courseId
                    ).Count() > 0;
        }
    }
}
