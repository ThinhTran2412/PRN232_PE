USE master;
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'PE_PRN_26SP_P8')
BEGIN
    ALTER DATABASE PE_PRN_26SP_P8 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PE_PRN_26SP_P8;
END
GO

CREATE DATABASE PE_PRN_26SP_P8;
GO

USE PE_PRN_26SP_P8;
GO

-- Create tables
CREATE TABLE Instructors (
    InstructorID INT IDENTITY(1,1) PRIMARY KEY,
    InstructorName NVARCHAR(100) NOT NULL,
    Department NVARCHAR(100) NULL,
    HireDate DATE NULL
);

CREATE TABLE Courses (
    CourseID INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Credits INT NULL,
    InstructorID INT NULL FOREIGN KEY REFERENCES Instructors(InstructorID)
);

CREATE TABLE Students (
    StudentID INT IDENTITY(1,1) PRIMARY KEY,
    StudentName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NULL,
    EnrollmentDate DATE NULL
);

CREATE TABLE Enrollments (
    EnrollmentID INT IDENTITY(1,1) PRIMARY KEY,
    StudentID INT NULL FOREIGN KEY REFERENCES Students(StudentID),
    CourseID INT NULL FOREIGN KEY REFERENCES Courses(CourseID),
    Semester NVARCHAR(50) NULL,
    Grade FLOAT NULL
);

-- Insert mock data
-- Instructors
INSERT INTO Instructors (InstructorName, Department, HireDate) VALUES 
(N'Nguyen Van A', N'Software Engineering', '2020-01-15'),
(N'Tran Thi B', N'Information Assurance', '2021-06-20'),
(N'Le Van C', N'Software Engineering', '2022-09-01');

-- Courses
INSERT INTO Courses (Title, Credits, InstructorID) VALUES
(N'Web Application Development', 3, 1),
(N'Introduction to Software Engineering', 3, 1),
(N'Information Security', 3, 2),
(N'Database Systems', 4, 3);

-- Students
INSERT INTO Students (StudentName, Email, EnrollmentDate) VALUES
(N'Tran Van X', 'xtv@fpt.edu.vn', '2023-09-05'),
(N'Nguyen Thi Y', 'ynt@fpt.edu.vn', '2023-09-05'),
(N'Le Van Z', 'zlv@fpt.edu.vn', '2024-03-10');

-- Enrollments
INSERT INTO Enrollments (StudentID, CourseID, Semester, Grade) VALUES
(1, 1, 'Fall-2024', 8.5),
(1, 2, 'Fall-2024', 7.0),
(2, 1, 'Fall-2024', 9.0),
(2, 3, 'Fall-2024', NULL), -- Not graded yet
(3, 4, 'Fall-2024', 6.5);
