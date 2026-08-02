using System;
using System.Collections.Generic;

namespace givenAPI.Models
{
    public static class DataInitializer
    {
        public static List<Instructor> Instructors = new List<Instructor> {
            new Instructor { InstructorId = 1, InstructorName = "Nguyen Van A", Department = "Software Engineering", HireDate = new DateTime(2020,1,15) },
            new Instructor { InstructorId = 2, InstructorName = "Tran Thi B", Department = "Information Assurance", HireDate = new DateTime(2021,6,20) },
            new Instructor { InstructorId = 3, InstructorName = "Le Van C", Department = "Software Engineering", HireDate = new DateTime(2022,9,1) }
        };

        public static List<Course> Courses = new List<Course> {
            new Course { CourseId = 101, Title = "Web Application Development", Credits = 3, InstructorId = 1 },
            new Course { CourseId = 102, Title = "Introduction to Software Engineering", Credits = 3, InstructorId = 1 },
            new Course { CourseId = 103, Title = "Information Security", Credits = 3, InstructorId = 2 },
            new Course { CourseId = 104, Title = "Database Systems", Credits = 4, InstructorId = 3 }
        };
    }
}
