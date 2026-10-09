using NextGen.Models;
using System.Configuration;
using System.Data.SqlClient;    
using System.Web.Mvc;
using System;
using System.IO;
using System.Web;

namespace NextGen.Controllers
{
    public class TeacherController : Controller
    {
        // =========================
        // DATABASE CONNECTION
        // =========================

        string cs =
        ConfigurationManager
        .ConnectionStrings["MyConnection"]
        .ConnectionString;

        // =========================
        // HOME PAGE
        // =========================

        public ActionResult Index()
        {
            return View();
        }

        // =========================
        // REGISTRATION PAGE
        // =========================

        public ActionResult TeacherRegistration()
        {
            return View();
        }

        // =========================
        // TEACHER REGISTRATION
        // =========================

        [HttpPost]
        public ActionResult TeacherRegistration(Teacher t)
        {
            // PASSWORD CHECK

            if (t.Password != t.ConfirmPassword)
            {
                ViewBag.msg =
                "Password and Confirm Password do not match";

                ViewBag.msgClass = "error";

                return View();
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // CHECK EMAIL EXISTS

                string checkQuery =
                "SELECT COUNT(*) FROM Teacher WHERE TeacherEmail=@TeacherEmail";

                SqlCommand checkCmd =
                new SqlCommand(checkQuery, con);

                checkCmd.Parameters.AddWithValue(
                    "@TeacherEmail",
                    t.TeacherEmail
                );

                int count = (int)checkCmd.ExecuteScalar();

                // EMAIL ALREADY REGISTERED

                if (count > 0)
                {
                    ViewBag.msg =
                    "You are already registered with this email";

                    ViewBag.msgClass = "error";

                    return View();
                }

                // INSERT QUERY

                string query = @"
                INSERT INTO Teacher
                (
                    TeacherName,
                    CourseName,
                    Experience,
                    TeacherEmail,
                    Password,
                    ConfirmPassword,
                    CourseId
                )

                VALUES
                (
                    @TeacherName,
                    @CourseName,
                    @Experience,
                    @TeacherEmail,
                    @Password,
                    @ConfirmPassword,
                    @CourseId
                )";

                SqlCommand cmd =
                new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@TeacherName",
                    t.TeacherName
                );

                cmd.Parameters.AddWithValue(
                    "@CourseName",
                    t.CourseName
                );

                cmd.Parameters.AddWithValue(
                    "@Experience",
                    t.Experience
                );

                cmd.Parameters.AddWithValue(
                    "@TeacherEmail",
                    t.TeacherEmail
                );

                cmd.Parameters.AddWithValue(
                    "@Password",
                    t.Password
                );

                cmd.Parameters.AddWithValue(
                    "@ConfirmPassword",
                    t.ConfirmPassword
                );

                cmd.Parameters.AddWithValue(
                    "@CourseId",
                    t.CourseId
                );

                cmd.ExecuteNonQuery();
            }

            ViewBag.msg =
            "Teacher Registered Successfully";

            ViewBag.msgClass = "success";

            return View();
        }

        // =========================
        // LOGIN PAGE
        // =========================

        public ActionResult TeacherLogin()
        {
            return View();
        }

        // =========================
        // LOGIN POST
        // =========================

        [HttpPost]
        public ActionResult TeacherLogin(Teacher t)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                SELECT * FROM Teacher

                WHERE TeacherEmail=@TeacherEmail
                AND Password=@Password";

                SqlCommand cmd =
                new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@TeacherEmail",
                    t.TeacherEmail
                );

                cmd.Parameters.AddWithValue(
                    "@Password",
                    t.Password
                );

                con.Open();

                SqlDataReader dr =
                cmd.ExecuteReader();

                // LOGIN SUCCESS

                if (dr.Read())
                {
                    Session["TeacherId"] =
                    dr["Id"].ToString();

                    Session["TeacherName"] =
                    dr["TeacherName"].ToString();

                    Session["TeacherEmail"] =
                    dr["TeacherEmail"].ToString();

                    Session["CourseName"] =
                    dr["CourseName"].ToString();

                    return RedirectToAction(
                        "TeacherDashboard"
                    );
                }
                else
                {
                    ViewBag.msg =
                    "Invalid email or password";

                    ViewBag.msgClass = "error";

                    return View();
                }
            }
        }

        // =========================
        // DASHBOARD
        // =========================

        public ActionResult TeacherDashboard()
        {
            // LOGIN CHECK

            if (Session["TeacherEmail"] == null)
            {
                return RedirectToAction(
                    "TeacherLogin"
                );
            }

            return View();
        }

        // =========================
        // LOGOUT
        // =========================

        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction(
                "TeacherLogin"
            );
        }

        // =========================
        // RESET PASSWORD
        // =========================

        [HttpPost]
        public ActionResult ResetTeacherPassword
        (
            string TeacherEmail,
            string NewPassword,
            string ConfirmPassword
        )
        {
            // PASSWORD CHECK

            if (NewPassword != ConfirmPassword)
            {
                TempData["msg"] =
                "Password and Confirm Password do not match.";

                return RedirectToAction(
                    "TeacherLogin"
                );
            }

            using (SqlConnection con =
                new SqlConnection(cs))
            {
                // UPDATE PASSWORD

                string query = @"
                UPDATE Teacher

                SET
                Password=@p,
                ConfirmPassword=@p

                WHERE TeacherEmail=@e";

                SqlCommand cmd =
                new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@p",
                    NewPassword
                );

                cmd.Parameters.AddWithValue(
                    "@e",
                    TeacherEmail
                );

                con.Open();

                int row =
                cmd.ExecuteNonQuery();

                // SUCCESS

                if (row > 0)
                {
                    TempData["msg"] =
                    "Password reset successfully.";
                }
                else
                {
                    TempData["msg"] =
                    "Email not found.";
                }
            }

            return RedirectToAction(
                "TeacherLogin"
            );
        }
        // =========================
        // ADD LIVE CLASS
        // =========================

        [HttpPost]
        public ActionResult AddLiveClass
        (
            int TeacherId,
            string CourseName,
            string ClassTitle,
            DateTime ClassDate,
            TimeSpan StartTime,
            TimeSpan EndTime,
            string ClassLink
        )
        {
            using (SqlConnection con =
                new SqlConnection(cs))
            {
                string query = @"
        INSERT INTO LiveClasses
        (
            TeacherId,
            CourseName,
            ClassTitle,
            ClassDate,
            StartTime,
            EndTime,
            ClassLink
        )

        VALUES
        (
            @TeacherId,
            @CourseName,
            @ClassTitle,
            @ClassDate,
            @StartTime,
            @EndTime,
            @ClassLink
        )";

                SqlCommand cmd =
                new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@TeacherId",
                    TeacherId
                );

                cmd.Parameters.AddWithValue(
                    "@CourseName",
                    CourseName
                );

                cmd.Parameters.AddWithValue(
                    "@ClassTitle",
                    ClassTitle
                );

                cmd.Parameters.AddWithValue(
                    "@ClassDate",
                    ClassDate
                );

                cmd.Parameters.AddWithValue(
                    "@StartTime",
                    StartTime
                );

                cmd.Parameters.AddWithValue(
                    "@EndTime",
                    EndTime
                );

                cmd.Parameters.AddWithValue(
                    "@ClassLink",
                    ClassLink
                );

                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["msg"] =
            "Live class added successfully.";

            return RedirectToAction(
                "TeacherDashboard"
            );
        }


        // =========================
        // ADD ASSIGNMENT
        // =========================

        [HttpPost]
        public ActionResult AddAssignment
        (
            int TeacherId,
            string CourseName,
            string Title,
            string Description,
            DateTime Deadline
        )
        {
            using (SqlConnection con =
                new SqlConnection(cs))
            {
                string query = @"
        INSERT INTO Assignments
        (
            TeacherId,
            CourseName,
            Title,
            Description,
            Deadline
        )

        VALUES
        (
            @TeacherId,
            @CourseName,
            @Title,
            @Description,
            @Deadline
        )";

                SqlCommand cmd =
                new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@TeacherId",
                    TeacherId
                );

                cmd.Parameters.AddWithValue(
                    "@CourseName",
                    CourseName
                );

                cmd.Parameters.AddWithValue(
                    "@Title",
                    Title
                );

                cmd.Parameters.AddWithValue(
                    "@Description",
                    Description
                );

                cmd.Parameters.AddWithValue(
                    "@Deadline",
                    Deadline
                );

                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["msg"] =
            "Assignment added successfully.";

            return RedirectToAction(
                "TeacherDashboard"
            );
        }


        // =========================
        // ADD NOTES
        // =========================

        [HttpPost]
        public ActionResult AddNote
        (
            int TeacherId,
            string CourseName,
            string Title,
            HttpPostedFileBase NoteFile
        )
        {
            if (NoteFile != null)
            {
                string fileName =
                Path.GetFileName(
                    NoteFile.FileName
                );

                string filePath =
                "~/Content/Notes/" + fileName;

                string savePath =
                Server.MapPath(filePath);

                NoteFile.SaveAs(savePath);

                using (SqlConnection con =
                    new SqlConnection(cs))
                {
                    string query = @"
            INSERT INTO StudyNotes
            (
                TeacherId,
                CourseName,
                Title,
                FilePath
            )

            VALUES
            (
                @TeacherId,
                @CourseName,
                @Title,
                @FilePath
            )";

                    SqlCommand cmd =
                    new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue(
                        "@TeacherId",
                        TeacherId
                    );

                    cmd.Parameters.AddWithValue(
                        "@CourseName",
                        CourseName
                    );

                    cmd.Parameters.AddWithValue(
                        "@Title",
                        Title
                    );

                    cmd.Parameters.AddWithValue(
                        "@FilePath",
                        filePath
                    );

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["msg"] =
            "Notes uploaded successfully.";

            return RedirectToAction(
                "TeacherDashboard"
            );
        }
    }
}