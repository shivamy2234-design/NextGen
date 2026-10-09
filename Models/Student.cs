using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextGen.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string StudentName { get; set; }

        public string StudentEmail { get; set; }

        public string StudentPassword { get; set; }

        public string StudentCpassword { get; set; }
    }
}