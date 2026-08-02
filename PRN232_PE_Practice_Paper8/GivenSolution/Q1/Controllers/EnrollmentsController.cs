using Microsoft.AspNetCore.Mvc;
using Q1.Models;

namespace Q1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly PePrn26spP8Context _context;

        public EnrollmentsController(PePrn26spP8Context context)
        {
            _context = context;
        }

        public class EnrollmentRequest
        {
            public int StudentId { get; set; }
            public int CourseId { get; set; }
            public string Semester { get; set; } = null!;
        }

        [HttpPost]
        public IActionResult CreateEnrollment([FromBody] EnrollmentRequest req)
        {
            var studentExists = _context.Students.Any(s => s.StudentId == req.StudentId);
            var course = _context.Courses.FirstOrDefault(c => c.CourseId == req.CourseId);

            if (!studentExists || course == null)
            {
                return NotFound("Student or Course not found");
            }

            // Business Rule 1: Trùng lặp
            bool isDuplicate = _context.Enrollments.Any(e => 
                e.StudentId == req.StudentId && 
                e.CourseId == req.CourseId && 
                e.Semester == req.Semester);

            if (isDuplicate)
            {
                return BadRequest("Student is already enrolled in this course for this semester");
            }

            // Business Rule 2: Limit 15 credits
            var currentCredits = _context.Enrollments
                .Where(e => e.StudentId == req.StudentId && e.Semester == req.Semester)
                .Sum(e => e.Course != null ? e.Course.Credits : 0) ?? 0;

            if (currentCredits + (course.Credits ?? 0) > 15)
            {
                return BadRequest("Enrolling in this course exceeds the limit of 15 credits per semester");
            }

            var newEnrollment = new Enrollment
            {
                StudentId = req.StudentId,
                CourseId = req.CourseId,
                Semester = req.Semester,
                Grade = null
            };

            _context.Enrollments.Add(newEnrollment);
            _context.SaveChanges();

            // Ignore object cycle when returning JSON
            return StatusCode(201, new
            {
                enrollmentId = newEnrollment.EnrollmentId,
                studentId = newEnrollment.StudentId,
                courseId = newEnrollment.CourseId,
                semester = newEnrollment.Semester,
                grade = newEnrollment.Grade
            });
        }

        [HttpDelete("{enrollmentId}")]
        public IActionResult DropEnrollment(int enrollmentId)
        {
            var enrollment = _context.Enrollments.FirstOrDefault(e => e.EnrollmentId == enrollmentId);
            if (enrollment == null)
            {
                return NotFound("No enrollment found with provided EnrollmentId");
            }

            if (enrollment.Grade != null)
            {
                return BadRequest("Cannot drop an enrollment that has already been graded");
            }

            _context.Enrollments.Remove(enrollment);
            _context.SaveChanges();

            return NoContent();
        }

        public class UpdateGradeRequest
        {
            public double? Grade { get; set; }
        }

        [HttpPatch("{enrollmentId}")]
        public IActionResult UpdateGrade(int enrollmentId, [FromBody] UpdateGradeRequest req)
        {
            var enrollment = _context.Enrollments.FirstOrDefault(e => e.EnrollmentId == enrollmentId);
            if (enrollment == null)
            {
                return NotFound("No enrollment found with provided EnrollmentId");
            }

            if (req.Grade < 0 || req.Grade > 10)
            {
                return BadRequest("Grade must be between 0 and 10");
            }

            enrollment.Grade = req.Grade;
            _context.SaveChanges();

            return Ok(new
            {
                enrollmentId = enrollment.EnrollmentId,
                studentId = enrollment.StudentId,
                courseId = enrollment.CourseId,
                semester = enrollment.Semester,
                grade = enrollment.Grade
            });
        }
    }
}
