using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EVoting.Server.Services.EMailService
{
    public class EMailService
    {


        public static async Task<bool> SendEmail(string Email, string Subject, string Message)
        {
            try
            {

                // Credentials
                var credentials = new NetworkCredential("username", "password");
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress("office@evoting.com"),
                    Subject = Subject,
                    Body = Message
                };
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(Email));
                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials
                };

                await client.SendMailAsync(mail);
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
    }
}
