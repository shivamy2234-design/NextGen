using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace NextGen.Models
{
    public class Enquiry
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public int Phone { get; set; }
        [Required]
        public string Message { get; set; }
        

        public string SendMail()
        {
            SmtpClient smtp = new SmtpClient("smtp.gmail.com",587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("shivamy2234@gmail.com", "hghd fggz uyha mgdm");
            MailMessage msg = new MailMessage("shivamy2234@gmail.com",Email,"Confirmation of sending email","your email has been sent successfully");
            smtp.Send(msg);
            return "Mail sent successfully";
        }
    }

   

}