using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextGen.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        public string StudentName { get; set; }

        public string StudentEmail { get; set; }

        public int Rating { get; set; }

        public string Message { get; set; }

        public bool IsApproved { get; set; }

        public bool IsTop { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}