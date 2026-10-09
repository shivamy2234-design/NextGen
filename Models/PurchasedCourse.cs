using System;

namespace NextGen.Models
{
    public class PurchasedCourse
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentEmail { get; set; }

        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public decimal CoursePrice { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string PaymentId { get; set; }

        public string Status { get; set; }

        public string CourseImage { get; set; }

        public string Duration { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }
    }
}