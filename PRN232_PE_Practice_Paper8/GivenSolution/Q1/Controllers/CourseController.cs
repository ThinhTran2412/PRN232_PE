using Microsoft.AspNetCore.Mvc;
using Q1.Models;

namespace Q1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly PePrn26spP8Context _context;

        public CoursesController(PePrn26spP8Context context)
        {
            _context = context;
        }

        [HttpGet("catalog")]
        public IActionResult GetCatalog([FromQuery] string? department, [FromQuery] int minCredits = 0)
        {
            var query = _context.Courses.Select(c => new
            {
                courseId = c.CourseId,
                title = c.Title,
                credits = c.Credits,
                department = c.Instructor != null ? c.Instructor.Department : null,
                totalEnrolled = c.Enrollments.Count(),
                gradeSum = c.Enrollments.Sum(e => e.Grade),
                averageGrade = c.Enrollments.Average(e => e.Grade)
            }).AsQueryable();

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(q => q.department != null && q.department.Contains(department));
            }

            if (minCredits > 0)
            {
                query = query.Where(q => q.credits >= minCredits);
            }

            return Ok(query.ToList());
        }
    }
}
