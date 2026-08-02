using Microsoft.AspNetCore.Mvc;
using Q1.Models;

namespace Q1.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CourseController : Controller
    {
        private readonly PePrn26spP8Context _context;

        public CourseController(PePrn26spP8Context context)
        {
            _context = context;
        }

        [HttpGet("catalog")]
        public IActionResult GetCourseCatalog([FromQuery] double? minAverageGrade)
        {
            var course = _context.Courses.Select(c => new
            {
                courseId = c.CourseId,
                title = c.Title,
                credit = c.Credits,
                instructorName = c.Instructor != null ? c.Instructor.InstructorName : "",
                averageGrade = c.Enrollments.Any(e => e.Grade != null) ? c.Enrollments.Where(e => e.Grade != null).Average(e => e.Grade) : 0,
                totalEnrolled = c.Enrollments.Count(),
                gradeSum = c.Enrollments.Any(e => e.Grade != null) ? c.Enrollments.Where(e => e.Grade != null).Sum(e => e.Grade) : 0

            }).AsQueryable();

            if (minAverageGrade.HasValue)
            {
                course = course.Where(c => c.averageGrade > minAverageGrade);
            }

            course = course.OrderByDescending(x => x.averageGrade).ThenBy(x => x.title);

            return Ok(course.ToList());
        }
    }
}
