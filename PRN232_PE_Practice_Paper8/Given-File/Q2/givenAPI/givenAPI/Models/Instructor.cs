using System;

namespace givenAPI.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public string InstructorName { get; set; } = null!;
        public string? Department { get; set; }
        public DateTime? HireDate { get; set; }
    }
}
