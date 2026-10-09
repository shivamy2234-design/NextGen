using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextGen.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        public string TeacherName { get; set; }

        public string CourseName { get; set; }

        public string TeacherEmail { get; set; }
        public string Experience { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public int CourseId { get; set; }
    }
}