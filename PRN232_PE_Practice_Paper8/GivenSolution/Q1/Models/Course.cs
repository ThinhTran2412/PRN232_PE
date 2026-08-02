using System;
using System.Collections.Generic;

namespace Q1.Models
{
    public partial class Course
    {
        public Course()
        {
            Enrollments = new HashSet<Enrollment>();
        }

        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public int? Credits { get; set; }
        public int? InstructorId { get; set; }

        public virtual Instructor? Instructor { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
