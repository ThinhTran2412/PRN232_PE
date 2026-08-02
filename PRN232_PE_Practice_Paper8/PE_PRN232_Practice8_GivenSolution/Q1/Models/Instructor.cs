using System;
using System.Collections.Generic;

namespace Q1.Models
{
    public partial class Instructor
    {
        public Instructor()
        {
            Courses = new HashSet<Course>();
        }

        public int InstructorId { get; set; }
        public string InstructorName { get; set; } = null!;
        public string? Department { get; set; }
        public DateTime? HireDate { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}
