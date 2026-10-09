using NextGen.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace NextGen.Controllers
{
    public class UserController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

        public ActionResult Index()
        {
            List<Feedback> feedbackList = new List<Feedback>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT TOP 2 * FROM Feedbacks
                 ORDER BY CreatedAt DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Feedback fb = new Feedback();

                    fb.Id = Convert.ToInt32(dr["Id"]);
                    fb.StudentName = dr["StudentName"].ToString();
                    fb.StudentEmail = dr["StudentEmail"].ToString();
                    fb.Rating = Convert.ToInt32(dr["Rating"]);
                    fb.Message = dr["Message"].ToString();
                    fb.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    fb.IsTop = Convert.ToBoolean(dr["IsTop"]);
                    fb.CreatedAt = Convert.ToDateTime(dr["CreatedAt"]);

                    feedbackList.Add(fb);
                }
            }

            return View(feedbackList);
        }

        [HttpPost]
        public ActionResult Index(Enquiry en)
        {
            string res = en.SendMail();
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Services()
        {
            return View();
        }

        public ActionResult CourseDetails(int id)
        {
            if (Session["StudentEmail"] == null)
            {
                TempData["msg"] = "Please login first to purchase any batch.";

                return RedirectToAction(
                    "StudentLogin",
                    new
                    {
                        returnUrl = Url.Action("CourseDetails", "User", new { id = id })
                    }
                );
            }

            Course course = null;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT * FROM Courses WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    course = new Course();

                    course.Id = Convert.ToInt32(dr["Id"]);
                    course.CourseName = dr["CourseName"].ToString();
                    course.CoursePrice = Convert.ToDecimal(dr["CoursePrice"]);
                    course.CourseImage = dr["CourseImage"].ToString();
                    course.StartTime = dr["StartTime"].ToString();
                    course.EndTime = dr["EndTime"].ToString();
                    course.Duration = dr["Duration"].ToString();
                }
            }

            if (course == null)
            {
                return RedirectToAction("Batches");
            }

            return View(course);
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(Contact model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    string query = @"INSERT INTO Contact
                                    (FirstName, LastName, Email, Type, Message)
                                    VALUES
                                    (@FirstName, @LastName, @Email, @Type, @Message)";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Message", model.Message);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["Success"] = "Message sent successfully.";
                return RedirectToAction("Contact");
            }

            TempData["Error"] = "Please fill all fields correctly.";
            return View(model);
        }

        public ActionResult StudentRegistration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StudentRegistration(Student s)
        {
            if (s.StudentPassword != s.StudentCpassword)
            {
                ViewBag.msg = "Password and Confirm Password do not match";
                ViewBag.msgClass = "error";
                return View();
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                string checkQuery = "SELECT COUNT(*) FROM Students WHERE StudentEmail = @StudentEmail";

                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@StudentEmail", s.StudentEmail);

                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    ViewBag.msg = "You are already registered with this email";
                    ViewBag.msgClass = "error";
                    return View();
                }

                string query = @"INSERT INTO Students
                                (StudentName, StudentEmail, StudentPassword, StudentCpassword)
                                VALUES
                                (@StudentName, @StudentEmail, @StudentPassword, @StudentCpassword)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@StudentName", s.StudentName);
                cmd.Parameters.AddWithValue("@StudentEmail", s.StudentEmail);
                cmd.Parameters.AddWithValue("@StudentPassword", s.StudentPassword);
                cmd.Parameters.AddWithValue("@StudentCpassword", s.StudentCpassword);

                cmd.ExecuteNonQuery();
            }

            ViewBag.msg = "Student Registered Successfully";
            ViewBag.msgClass = "success";

            return View();
        }

        public ActionResult StudentLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult StudentLogin(Student s, string returnUrl)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT * FROM Students
                                WHERE StudentEmail = @StudentEmail
                                AND StudentPassword = @StudentPassword";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@StudentEmail", s.StudentEmail);
                cmd.Parameters.AddWithValue("@StudentPassword", s.StudentPassword);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    Session["StudentId"] = dr["Id"].ToString();
                    Session["StudentName"] = dr["StudentName"].ToString();
                    Session["StudentEmail"] = dr["StudentEmail"].ToString();

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("StudentDashboard", "User");
                }
                else
                {
                    ViewBag.msg = "Invalid email or password";
                    return View();
                }
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "User");
        }

        public ActionResult Batches()
        {
            List<Course> courseList = new List<Course>();

            using (SqlConnection con = new SqlConnection(cs))
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

                    courseList.Add(c);
                }
            }

            return View(courseList);
        }

        public ActionResult StudentDashboard()
        {
            if (Session["StudentEmail"] == null)
            {
                return RedirectToAction("StudentLogin");
            }

            List<PurchasedCourse> purchasedCourses = new List<PurchasedCourse>();

            int studentId = Convert.ToInt32(Session["StudentId"]);

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
        SELECT 
            pc.Id,
            pc.StudentId,
            pc.StudentName,
            pc.StudentEmail,
            pc.CourseId,
            pc.CourseName,
            pc.CoursePrice,
            pc.PurchaseDate,
            pc.PaymentId,
            pc.Status,
            c.CourseImage,
            c.Duration,
            c.StartTime,
            c.EndTime
        FROM PurchasedCourses pc
        INNER JOIN Courses c
            ON pc.CourseId = c.Id
        WHERE pc.StudentId = @StudentId
        ORDER BY pc.PurchaseDate DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StudentId", studentId);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    PurchasedCourse pc = new PurchasedCourse();

                    pc.Id = Convert.ToInt32(dr["Id"]);
                    pc.StudentId = Convert.ToInt32(dr["StudentId"]);
                    pc.StudentName = dr["StudentName"].ToString();
                    pc.StudentEmail = dr["StudentEmail"].ToString();
                    pc.CourseId = Convert.ToInt32(dr["CourseId"]);
                    pc.CourseName = dr["CourseName"].ToString();
                    pc.CoursePrice = Convert.ToDecimal(dr["CoursePrice"]);
                    pc.PurchaseDate = Convert.ToDateTime(dr["PurchaseDate"]);
                    pc.PaymentId = dr["PaymentId"].ToString();
                    pc.Status = dr["Status"].ToString();
                    pc.CourseImage = dr["CourseImage"].ToString();
                    pc.Duration = dr["Duration"].ToString();
                    pc.StartTime = dr["StartTime"].ToString();
                    pc.EndTime = dr["EndTime"].ToString();

                    purchasedCourses.Add(pc);
                }
            }

            return View(purchasedCourses);
        }

        public ActionResult ResetStudentPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetStudentPassword(string StudentEmail, string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword)
            {
                TempData["msg"] = "Password and Confirm Password do not match.";
                return RedirectToAction("StudentLogin");
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE Students
                                SET StudentPassword = @Password,
                                    StudentCpassword = @Password
                                WHERE StudentEmail = @Email";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Password", NewPassword);
                cmd.Parameters.AddWithValue("@Email", StudentEmail);

                con.Open();

                int row = cmd.ExecuteNonQuery();

                if (row > 0)
                {
                    TempData["msg"] = "Password reset successfully.";
                }
                else
                {
                    TempData["msg"] = "Email not found.";
                }
            }

            return RedirectToAction("StudentLogin");
        }

        [HttpPost]
        public ActionResult SubmitFeedback(Feedback feedback)
        {
            if (feedback.Rating < 1 || feedback.Rating > 5)
            {
                TempData["FeedbackMsg"] = "Please select a valid rating.";
                return RedirectToAction("Index");
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO Feedbacks
                                (StudentName, StudentEmail, Rating, Message, IsApproved, IsTop, CreatedAt)
                                VALUES
                                (@StudentName, @StudentEmail, @Rating, @Message, @IsApproved, @IsTop, @CreatedAt)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@StudentName", feedback.StudentName);
                cmd.Parameters.AddWithValue("@StudentEmail", string.IsNullOrEmpty(feedback.StudentEmail) ? "" : feedback.StudentEmail);
                cmd.Parameters.AddWithValue("@Rating", feedback.Rating);
                cmd.Parameters.AddWithValue("@Message", feedback.Message);
                cmd.Parameters.AddWithValue("@IsApproved", true);
                cmd.Parameters.AddWithValue("@IsTop", true);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["FeedbackMsg"] = "Thank you! Your feedback has been submitted.";
            return RedirectToAction("Index");
        }
        public ActionResult AllFeedbacks()
        {
            List<Feedback> feedbackList = new List<Feedback>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT * FROM Feedbacks
                         ORDER BY CreatedAt DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Feedback fb = new Feedback();

                    fb.Id = Convert.ToInt32(dr["Id"]);
                    fb.StudentName = dr["StudentName"].ToString();
                    fb.StudentEmail = dr["StudentEmail"].ToString();
                    fb.Rating = Convert.ToInt32(dr["Rating"]);
                    fb.Message = dr["Message"].ToString();
                    fb.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    fb.IsTop = Convert.ToBoolean(dr["IsTop"]);
                    fb.CreatedAt = Convert.ToDateTime(dr["CreatedAt"]);

                    feedbackList.Add(fb);
                }
            }

            return View(feedbackList);
        }
        [HttpPost]
        public JsonResult SavePurchase
(
    int courseId,
    string paymentId
)
        {
            if (Session["StudentId"] == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Please login first."
                });
            }

            int studentId =
            Convert.ToInt32(Session["StudentId"]);

            using (SqlConnection con =
                new SqlConnection(cs))
            {
                con.Open();

                string checkQuery = @"
        SELECT COUNT(*)
        FROM PurchasedCourses
        WHERE StudentId=@StudentId
        AND CourseId=@CourseId";

                SqlCommand checkCmd =
                new SqlCommand(checkQuery, con);

                checkCmd.Parameters.AddWithValue(
                    "@StudentId",
                    studentId
                );

                checkCmd.Parameters.AddWithValue(
                    "@CourseId",
                    courseId
                );

                int exists =
                (int)checkCmd.ExecuteScalar();

                if (exists > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message =
                        "You already purchased this course."
                    });
                }

                string query = @"
        INSERT INTO PurchasedCourses
        (
            StudentId,
            StudentName,
            StudentEmail,
            CourseId,
            CourseName,
            CoursePrice,
            PaymentId,
            Status
        )

        VALUES
        (
            @StudentId,
            @StudentName,
            @StudentEmail,
            @CourseId,
            @CourseName,
            @CoursePrice,
            @PaymentId,
            @Status
        )";

                SqlCommand cmd =
                new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@StudentId",
                    studentId
                );

                cmd.Parameters.AddWithValue(
                    "@StudentName",
                    Session["StudentName"]
                );

                cmd.Parameters.AddWithValue(
                    "@StudentEmail",
                    Session["StudentEmail"]
                );

                cmd.Parameters.AddWithValue(
                    "@CourseId",
                    courseId
                );

                cmd.Parameters.AddWithValue(
                    "@PaymentId",
                    paymentId
                );

                cmd.Parameters.AddWithValue(
                    "@Status",
                    "Paid"
                );

                string courseQuery =
                "SELECT * FROM Courses WHERE Id=@Id";

                SqlCommand courseCmd =
                new SqlCommand(courseQuery, con);

                courseCmd.Parameters.AddWithValue(
                    "@Id",
                    courseId
                );

                SqlDataReader dr =
                courseCmd.ExecuteReader();

                if (dr.Read())
                {
                    cmd.Parameters.AddWithValue(
                        "@CourseName",
                        dr["CourseName"]
                    );

                    cmd.Parameters.AddWithValue(
                        "@CoursePrice",
                        dr["CoursePrice"]
                    );
                }

                dr.Close();

                cmd.ExecuteNonQuery();

                return Json(new
                {
                    success = true
                });
            }
        }
        public ActionResult OurTeam()
        {
            return View();
        }
    }
}