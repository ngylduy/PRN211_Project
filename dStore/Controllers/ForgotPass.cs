using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;


namespace dStore.Controllers
{

    public class ForgotPassController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index([FromForm] string email)
        {
            try
            {
                var context = new FStoreContext();
                Member mem = context.Members.Where(x => x.Email.Equals(email)).FirstOrDefault();
                if (mem != null)
                {
                    var password = randomPass();
                    // Set up email message
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("plusthuthuatnet@gmail.com");
                    message.To.Add(new MailAddress(mem.Email));
                    message.Subject = "Password Reset";
                    message.Body = "Dear " + mem.Fullname + ",\nYou requested to reset your password.\nHere is your new password: " + password;
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("plusthuthuatnet@gmail.com", "pxemsggrmjvwhoqj");
                    smtpClient.EnableSsl = true;
                    // Send email
                    smtpClient.Send(message);
                    mem.Password = password;
                    context.SaveChanges();
                    ViewBag.Message = "Your password has been recovered successfully, check your email and sign in again.";
                }
                else
                {
                    ViewBag.Message = "Email is not valid";
                    return View();
                }
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        private string randomPass()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int passwordLength = random.Next(5, 10);
            string password = new string(Enumerable.Repeat(chars, passwordLength)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }

    }
}
