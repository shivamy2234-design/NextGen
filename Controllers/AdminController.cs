using NextGen.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;



namespace NextGen.Controllers
{
    public class AdminController : Controller
    {
        // ADMIN LOGIN PAGE
        public ActionResult AdminLogin()
        {
            return View();
        }

        // ADMIN LOGIN POST
        [HttpPost]
        public ActionResult AdminLogin(string UserName, string Password)
        {
            string cs = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT * FROM Admin WHERE UserName=@u AND Password=@p";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@u", UserName);
                cmd.Parameters.AddWithValue("@p", Password);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    Session["AdminName"] = dr["AdminName"].ToString();
                    Session["AdminUserName"] = dr["UserName"].ToString();

                    return RedirectToAction("AdminDashboard");
                }
                else
                {
                    ViewBag.msg = "Invalid Username or Password";
                    return View();
                }
            }
        }

        // ADMIN DASHBOARD
        public ActionResult AdminDashboard()
        {
            if (Session["AdminUserName"] == null)
            {
                return RedirectToAction("AdminLogin");
            }

            string cs = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                ViewBag.TotalStudents = Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM Students", con).ExecuteScalar());
                ViewBag.TotalTeachers = Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM Teacher", con).ExecuteScalar());
                ViewBag.TotalCourses = Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM Courses", con).ExecuteScalar());
                ViewBag.TotalRevenue = Convert.ToDecimal(new SqlCommand("SELECT ISNULL(SUM(CoursePrice),0) FROM PurchasedCourses", con).ExecuteScalar());

                // STUDENTS LIST
                List<Student> students = new List<Student>();
                SqlCommand studentListCmd = new SqlCommand("SELECT * FROM Students ORDER BY Id DESC", con);
                SqlDataReader dr = studentListCmd.ExecuteReader();

                while (dr.Read())
                {
                    students.Add(new Student()
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        StudentName = dr["StudentName"].ToString(),
                        StudentEmail = dr["StudentEmail"].ToString()
                    });
                }

                dr.Close();
                ViewBag.StudentList = students;

                // TEACHERS LIST
                List<Teacher> teachers = new List<Teacher>();
                SqlCommand teacherCmd = new SqlCommand("SELECT * FROM Teacher ORDER BY Id DESC", con);
                SqlDataReader teacherDr = teacherCmd.ExecuteReader();

                while (teacherDr.Read())
                {
                    teachers.Add(new Teacher()
                    {
                        Id = Convert.ToInt32(teacherDr["Id"]),
                        TeacherName = teacherDr["TeacherName"].ToString(),
                        TeacherEmail = teacherDr["TeacherEmail"].ToString(),
                        CourseName = teacherDr["CourseName"].ToString(),
                        Experience = teacherDr["Experience"].ToString()
                    });
                }

                teacherDr.Close();
                ViewBag.TeacherList = teachers;

                // PURCHASES LIST
                List<PurchasedCourse> purchases = new List<PurchasedCourse>();
                SqlCommand purchaseCmd = new SqlCommand("SELECT * FROM PurchasedCourses ORDER BY PurchaseDate DESC", con);
                SqlDataReader purchaseDr = purchaseCmd.ExecuteReader();

                while (purchaseDr.Read())
                {
                    purchases.Add(new PurchasedCourse()
                    {
                        Id = Convert.ToInt32(purchaseDr["Id"]),
                        StudentName = purchaseDr["StudentName"].ToString(),
                        StudentEmail = purchaseDr["StudentEmail"].ToString(),
                        CourseName = purchaseDr["CourseName"].ToString(),
                        CoursePrice = Convert.ToDecimal(purchaseDr["CoursePrice"]),
                        PaymentId = purchaseDr["PaymentId"].ToString(),
                        Status = purchaseDr["Status"].ToString(),
                        PurchaseDate = Convert.ToDateTime(purchaseDr["PurchaseDate"])
                    });
                }

                purchaseDr.Close();
                ViewBag.PurchaseList = purchases;
            }

            return View();
        }

        // LOGOUT
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("AdminLogin");
        }
        public ActionResult ResetAdminPassword()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult ResetAdminPassword(string UserName, string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword)
            {
                TempData["msg"] = "Password and Confirm Password do not match.";
                return RedirectToAction("AdminLogin");
            }

            string cs = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "UPDATE Admin SET Password=@p WHERE UserName=@u";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@u", UserName);
                cmd.Parameters.AddWithValue("@p", NewPassword);

                con.Open();

                int row = cmd.ExecuteNonQuery();

                if (row > 0)
                {
                    TempData["msg"] = "Password reset successfully. Please login.";
                }
                else
                {
                    TempData["msg"] = "Username not found.";
                }
            }

            return RedirectToAction("AdminLogin");
        }
        public ActionResult ManageCourses()
        {
            if (Session["AdminUserName"] == null)
            {
                return RedirectToAction("AdminLogin");
            }

            List<Course> courses = new List<Course>();

            using (SqlConnection con = new SqlConnection(
                ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString))
            {
                string query = "SELECT * FROM Courses ORDER BY Id DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Course c = new Course();

                    c.Id = Convert.ToInt32(dr["Id"]);
                    c.CourseName = dr["CourseName"].ToString();
                    c.CoursePrice = Convert.ToDecimal(dr["CoursePrice"]);
                    c.CourseImage = dr["CourseImage"].ToString();
                    c.StartTime = dr["StartTime"].ToString();
                    c.EndTime = dr["EndTime"].ToString();
                    c.Duration = dr["Duration"].ToString();

                    courses.Add(c);
                }
            }

            return View(courses);
        }

        [HttpPost]
        public ActionResult AddCourse
        (
            string CourseName,
            decimal CoursePrice,
            string StartTime,
            string EndTime,
            string Duration,
            HttpPostedFileBase CourseImage
        )
        {
            if (Session["AdminUserName"] == null)
            {
                return RedirectToAction("AdminLogin");
            }

            string imagePath = "";

            if (CourseImage != null && CourseImage.ContentLength > 0)
            {
                string fileName = Path.GetFileName(CourseImage.FileName);
                imagePath = "~/Content/CourseImages/" + fileName;

                string savePath = Server.MapPath(imagePath);
                CourseImage.SaveAs(savePath);
            }

            using (SqlConnection con = new SqlConnection(
                ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString))
            {
                string query = @"
        INSERT INTO Courses
        (CourseName, CoursePrice, CourseImage, StartTime, EndTime, Duration)
        VALUES
        (@CourseName, @CoursePrice, @CourseImage, @StartTime, @EndTime, @Duration)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@CourseName", CourseName);
                cmd.Parameters.AddWithValue("@CoursePrice", CoursePrice);
                cmd.Parameters.AddWithValue("@CourseImage", imagePath);
                cmd.Parameters.AddWithValue("@StartTime", TimeSpan.Parse(StartTime));
                cmd.Parameters.AddWithValue("@EndTime", TimeSpan.Parse(EndTime));
                cmd.Parameters.AddWithValue("@Duration", Duration);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["CourseMsg"] = "Course added successfully.";
            return RedirectToAction("ManageCourses");
        }

        public ActionResult DeleteCourse(int id)
        {
            if (Session["AdminUserName"] == null)
            {
                return RedirectToAction("AdminLogin");
            }

            using (SqlConnection con = new SqlConnection(
                ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString))
            {
                string query = "DELETE FROM Courses WHERE Id=@Id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["CourseMsg"] = "Course deleted successfully.";
            return RedirectToAction("ManageCourses");
        }
        public ActionResult DeleteStudent(int id)
        {
            string cs =
                ConfigurationManager
                .ConnectionStrings["MyConnection"]
                .ConnectionString;

            using (SqlConnection con =
                new SqlConnection(cs))
            {
                string query =
                    "DELETE FROM Students WHERE Id=@Id";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }


            return RedirectToAction(
                "AdminDashboard"
            );
        }
        public ActionResult DeleteTeacher(int id)
        {
            string cs =
                ConfigurationManager
                .ConnectionStrings["MyConnection"]
                .ConnectionString;

            using (SqlConnection con =
                new SqlConnection(cs))
            {
                string query =
                    "DELETE FROM Teacher WHERE Id=@Id";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@Id",
                    id
                );

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction(
                "AdminDashboard"
            );
        }

    }
}