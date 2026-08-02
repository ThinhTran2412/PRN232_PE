using Microsoft.AspNetCore.Mvc;
using givenAPI.Models;
using System.Linq;
using System;

namespace givenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorsController : ControllerBase
    {
        // 1. GET /api/instructors/search?name={name}&department={department}
        [HttpGet("search")]
        public IActionResult Search(string name = "", string department = "")
        {
            var result = DataInitializer.Instructors
                .Where(i => (string.IsNullOrEmpty(name) || i.InstructorName.Contains(name, StringComparison.OrdinalIgnoreCase))
                         && (string.IsNullOrEmpty(department) || i.Department.Contains(department, StringComparison.OrdinalIgnoreCase)))
                .Select(i => new {
                    InstructorId = i.InstructorId,
                    InstructorName = i.InstructorName,
                    Department = i.Department,
                    HireDate = i.HireDate,
                    TotalCourses = DataInitializer.Courses.Count(c => c.InstructorId == i.InstructorId)
                }).ToList();

            return Ok(result);
        }

        // 2. GET /api/instructors/{instructorId}
        [HttpGet("{instructorId}")]
        public IActionResult GetDetails(int instructorId)
        {
            var instructor = DataInitializer.Instructors.FirstOrDefault(i => i.InstructorId == instructorId);
            if (instructor == null) return NotFound(new { Message = "Instructor not found." });

            var taughtCourses = DataInitializer.Courses
                                   .Where(c => c.InstructorId == instructorId)
                                   .Select(c => new
                                   {
                                       CourseId = c.CourseId,
                                       Title = c.Title,
                                       Credits = c.Credits
                                   }).ToList();

            return Ok(new
            {
                InstructorId = instructor.InstructorId,
                InstructorName = instructor.InstructorName,
                Department = instructor.Department,
                Courses = taughtCourses
            });
        }
    }
}
