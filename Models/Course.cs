using System;

namespace NextGen.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public decimal CoursePrice { get; set; }
        public string CourseImage { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Duration { get; set; }
    }
}