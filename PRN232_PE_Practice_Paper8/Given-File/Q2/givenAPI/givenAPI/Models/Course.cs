namespace givenAPI.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public int Credits { get; set; }
        public int InstructorId { get; set; }
    }
}
