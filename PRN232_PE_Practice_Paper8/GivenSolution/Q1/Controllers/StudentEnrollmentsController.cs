using Microsoft.AspNetCore.Mvc;
using Q1.Models;

namespace Q1.Controllers
{
    [Route("api/student-enrollments")]
    [ApiController]
    public class StudentEnrollmentsController : ControllerBase
    {
        private readonly PePrn26spP8Context _context;

        public StudentEnrollmentsController(PePrn26spP8Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetStudentEnrollments(
            [FromQuery] string? semester,
            [FromQuery] string? studentName,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // 1. Kiểm tra tham số phân trang
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Invalid pagination parameters.");
            }

            // 2. Bắt đầu build query từ bảng Students
            var query = _context.Students.AsQueryable();

            // Lọc theo tên sinh viên (nếu có)
            if (!string.IsNullOrEmpty(studentName))
            {
                query = query.Where(s => s.StudentName.Contains(studentName));
            }

            // Lọc theo học kỳ: Chỉ lấy những sinh viên có học trong kỳ đó (nếu có truyền semester)
            if (!string.IsNullOrEmpty(semester))
            {
                query = query.Where(s => s.Enrollments.Any(e => e.Semester == semester));
            }

            // 3. Map sang DTO để đếm Total Courses và tính Average Grade
            var statsQuery = query.Select(s => new
            {
                studentId = s.StudentId,
                studentName = s.StudentName,
                email = s.Email,
                // Đếm số khóa học
                totalEnrolledCourses = string.IsNullOrEmpty(semester)
                    ? s.Enrollments.Count()
                    : s.Enrollments.Count(e => e.Semester == semester),
                // Tính điểm trung bình (Tường minh vụ Ignore NULL)
                averageSemesterGrade = string.IsNullOrEmpty(semester)
                    ? s.Enrollments.Where(e => e.Grade != null).Average(e => e.Grade) ?? 0
                    : s.Enrollments.Where(e => e.Semester == semester && e.Grade != null).Average(e => e.Grade) ?? 0
            });

            // 4. Tính toán Pagination Metadata
            int totalRecords = statsQuery.Count();
            int totalPages = totalRecords > 0 ? (int)Math.Ceiling(totalRecords / (double)pageSize) : 0;

            // 5. Cắt data cho trang hiện tại (Skip & Take)
            var pagedData = statsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 6. Trả về đúng format Object bọc ngoài mà đề bài yêu cầu
            return Ok(new
            {
                data = pagedData,
                totalRecords = totalRecords,
                totalPages = totalPages,
                currentPage = page,
                pageSize = pageSize
            });
        }
    }
}
