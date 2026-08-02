# PRN232 – Practical Exam – Paper No: 8 (Luyện Tập Tư Duy)

**Duration:** 90 minutes — **Total:** 10 points

---

## INSTRUCTIONS

**Please read the instructions carefully before doing the questions.**

- You can use materials in your computer, notebook and text book.
- You are **NOT allowed** to use any device to share data with others.

Beside the above conditions, students must follow the following requirements:

1. **The work must complete by using Visual Studio 2022++**
2. **The Framework must be .NET 8.0**
3. **THIS PART IS VERY IMPORTANT, PLEASE READ IT CAREFULLY AND FOLLOW THE INSTRUCTIONS.**
   - You are given a database script (.sql file) in Zip file. Execute the script before doing questions.
   - **You must use the given solution.**
   - **You are not allowed to add any more libraries via NuGet Package Manager into given solution.**
   - **Submission Guideline**:Submit your work for each question separately. For each question, please:
     - Publish your project using the command:

       ```
       dotnet publish -c Release -o ./[QuestionNumber_StudentAccount]
       ```

       Example:

       ```
       dotnet publish -c Release -o ./Q1_trungnthe123432
       ```
     - Submit the root folder of the project into the PEA_Client application.
     - If the root folder of the project is too large, you may delete the following subfolders to reduce its size before submitting: `/bin`, `/obj`

**Just one of above requirements is violated, your work will be considered as invalid.**

---

# Question 1: (5 points)

You are provided with a database as following diagram:

### Database Schema (database.sql — database name: PE_PRN_26SP_P8)

| Table | Column | Data Type / Notes |
| --- | --- | --- |
| Instructors | InstructorID | INT – PK, auto-increment |
|  | InstructorName | NVARCHAR(100) NOT NULL |
|  | Department | NVARCHAR(100) |
|  | HireDate | DATE |
| Courses | CourseID | INT – PK, auto-increment |
|  | Title | NVARCHAR(200) NOT NULL |
|  | Credits | INT |
|  | InstructorID | INT – FK → Instructors |
| Students | StudentID | INT – PK, auto-increment |
|  | StudentName | NVARCHAR(100) NOT NULL |
|  | Email | NVARCHAR(100) |
|  | EnrollmentDate | DATE |
| Enrollments | EnrollmentID | INT – PK, auto-increment |
|  | StudentID | INT – FK → Students |
|  | CourseID | INT – FK → Courses |
|  | Semester | NVARCHAR(50) – e.g., Fall-2024 |
|  | Grade | FLOAT – NULL = the student has not been graded yet |

Your task is to develop a **.NET 8 Web API** project (use the given solution — project **Q1**, which already contains the Entity Framework Core models scaffolded from the database) that implements the following endpoints.

**Notes:**

- The database connection string must be stored in `appsettings.json` with the key `"MyCnn"`. **Zero marks** if it is stored anywhere else.
- All JSON property names in responses are in **camelCase** (default behavior).
- Examples below assume the API runs at `http://localhost:5000`.

## 1. GET /api/courses/catalog

Retrieve a list of courses, showing details and calculating GPA of students enrolled.

- Return a list of courses with the following information: **CourseId**, **Title**, **Credits**, **InstructorName**, **AverageGrade**, **TotalEnrolled** (count of all enrollments in this course), and **GradeSum** (sum of all grades for this course, ignoring NULL. If no grades, return 0).
- **AverageGrade** is calculated as the average of all **Grade** values in the **Enrollments** table for that course. Records with a NULL Grade should be **excluded** from the calculation. If a course has no graded enrollments, `averageGrade` should be `0`.
- **Filtering:** Use query parameter `minAverageGrade` (if provided, only show courses with average grade >= minAverageGrade).
- **Sorting:** Sort descending by `averageGrade`, then ascending by `title`.
- Support response formats in both **JSON** and **XML** using media formatters.
- Example:

```json
GET http://localhost:5000/api/courses/catalog?minAverageGrade=7.5
HTTP 200
[
  {
    "courseId": 101,
    "title": "Web Application Development",
    "credits": 3,
    "instructorName": "Nguyen Van A",
    "averageGrade": 8.25,
    "totalEnrolled": 2,
    "gradeSum": 17.5
  }
]
```

## 2. GET /api/student-enrollments

Retrieve a list of student enrollment statistics with detailed filtering and pagination features.

- **Filtering:** Use query parameters:
  - `semester`: Filter by exact semester (if provided, e.g., "Fall-2024").
  - `studentName`: Search for students whose name contains this string (if provided, case-insensitive).
- **Detailed pagination:** Use query parameters:
  - `page`: Current page (default is 1).
  - `pageSize`: Number of items per page (default is 10).
- Return the list of items for the requested page, along with pagination metadata: **TotalRecords**, **TotalPages**, **CurrentPage**, and **PageSize**.
- Each item in the list must contain: **StudentId**, **StudentName**, **Email**, **TotalEnrolledCourses** (count of courses the student enrolled in the specified semester), and **AverageSemesterGrade** (average grade of the student in the specified semester, ignoring NULL).
- **Error handling:** If page or pageSize is non-positive, return a **400 Bad Request** error with the body `Invalid pagination parameters.`

```json
GET http://localhost:5000/api/student-enrollments?semester=Fall-2024&page=1&pageSize=5
HTTP 200
{
  "data": [
    {
      "studentId": 1,
      "studentName": "Tran Van A",
      "email": "atv@fpt.edu.vn",
      "totalEnrolledCourses": 3,
      "averageSemesterGrade": 8.5
    }
  ],
  "totalRecords": 1,
  "totalPages": 1,
  "currentPage": 1,
  "pageSize": 5
}
```

## 3. POST /api/enrollments

Enroll a student into a course.

- Accept **StudentID**, **CourseID**, and **Semester** from the request body:

```json
{
  "studentId": 1,
  "courseId": 101,
  "semester": "Fall-2024"
}
```

- **Logic:**
  - If the **StudentID** does not exist, or the **CourseID** does not exist, return a **404 Not Found** error with the message "Student or Course not found".
  - **Business Rule (Trùng lặp):** A student cannot enroll in the same course in the same semester more than once. If an enrollment already exists for the same `studentId`, `courseId`, and `semester`, return a **400 Bad Request** error with the message "Student is already enrolled in this course for this semester".
  - **Business Rule (Số lượng tín chỉ):** A student cannot enroll in more than 15 total credits of courses in a single semester. If the new enrollment would cause the student's total credits in this semester to exceed 15, return a **400 Bad Request** error with the message "Enrolling in this course exceeds the limit of 15 credits per semester".
  - If valid, add the new enrollment with **Grade = NULL**. Return the created enrollment details with HTTP Status **201 Created**.

## 4. DELETE /api/enrollments/{enrollmentId}

Drop an enrollment (Remove the record from the **Enrollments** table).

- Delete the enrollment with the specified **EnrollmentId** from the URL and return **204 No Content** if success.
- **Error handling:**
  - If the enrollment record already has a grade (**Grade** is not NULL), return a **400 Bad Request** error with the message "Cannot drop an enrollment that has already been graded".
  - If the **EnrollmentId** does not exist, return a **404 Not Found** error with the message "No enrollment found with provided EnrollmentId".

---

# Question 2: (5 points)

In this question, you are asked to write an MVC/Razor Pages application. The application fetch data by calling pre-existing RESTful API hosted at **GivenAPIBaseUrl**. The API are provided in a separate project named **GivenAPIs**, which students must run locally to start the API server.

## 1. Important Notes

- Students **MUST** use **HttpClient** to make calls to the API.
- The value of **GivenAPIBaseUrl** must be written in `appsettings.json` as:

```json
{ "GivenAPIBaseUrl": "http://localhost:5100" }
```

- **"GivenAPIBaseUrl"** is a provided key, and students are not allowed to modify it.
- Students get the **GivenAPIBaseUrl** value from **appsettings.json**, combine it with the **endpoint** to call the API.
- When concatenating the base URL with the endpoint, students must explicitly use **string concatenation** (e.g., `"baseUrl" + "/endpoint"`).
- All input and output elements in the HTML source **must** have an **'id'** attribute to ensure accessibility and traceability.

## 2. Provided APIs

- **GET /api/instructors/search?name={name}&department={department}**: Returns a list of instructors filtered by their Instructor Name and Department. In case {name}, {department} is missing, return all **instructors**.
- **GET /api/instructors/{instructorId}**: Returns detailed information about a specific instructor including their taught courses.

## 3. Requirements

### 3.1. Instructor List Page (see list.html)

**URL: /Instructor**

- **Search Form:**
  - An input field for Instructor Name (id: `ip_instructorName`).
  - An input field for Department (id: `ip_department`).
  - A Search button (id: `bt_search`).
- **Table Display**: Display data in a tabular format with columns:
  - **Instructor Name**.
  - **Department**.
  - **Hire Date**.
  - **Total Courses** (Number of courses they teach).
- Each row in the table should have a **"View Courses"** link redirecting to the detail page with the URL `/Instructor/{InstructorId}`.

**ID Requirements:**

- Each `<td>` tag in the table: `td_{columnName}_{instructorId}`. Example: `td_instructorName_1`, `td_department_1`.
- Each "View Courses" link must be placed in an `<a>` tag with id `a_{instructorId}`. Example: `a_1`

### 3.2. Filter logic

- When a user enters criteria and clicks the "Search" button, the list should only show instructors matching both the Name and Department.
- If both fields are empty, list all instructors.

### 3.3. Instructor Detail Page (see detail.html)

**URL: /Instructor/{InstructorId}**

- **Display basic information** of the instructor: **InstructorID**, **InstructorName**, **Department**.
- **Display a list of all courses** taught by this instructor in a table with columns: **CourseID**, **Title**, **Credits**.

**ID Requirement:**

- Basic info fields in `<span>` tags: `span_{instructorId}`, `span_{instructorName}`, `span_{department}`. Example: `span_1`, `span_Software Engineering`
- Each `<td>` tag in the course table: `td_{columnName}_{courseId}`. Example: `td_courseID_1`, `td_title_1`

## 4. Summary of HTML Elements ID

The HTML id requirements are summarized in the table below:

| Page | Element | Tag | Id |
| --- | --- | --- | --- |
| /Instructor | Input Instructor Name | `<input>` | `ip_instructorName` |
|  | Input Department | `<input>` | `ip_department` |
|  | Button Search | `<input>` | `bt_search` |
|  | Each cell in the table | `<td>` | `td_{columnName}_{instructorId}` |
|  | Each View Products link | `<a>` | `a_{instructorId}` |
| /Instructor/{id} | Field InstructorID | `<span>` | `span_{instructorId}` |
|  | Field InstructorName | `<span>` | `span_{instructorName}` |
|  | Field Department | `<span>` | `span_{department}` |
|  | Each cell in course table | `<td>` | `td_{columnName}_{courseId}` |

**Note: Ensure all ID requirements are strictly followed** so that the examiner can automatically verify your work.

## 5. Summary of Required URLs

| Function | URL |
| --- | --- |
| Search and list instructors | /Instructor |
| Details of the instructor | /Instructor/{InstructorId} |

\--- END OF PAPER ---