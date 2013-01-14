using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Uqbar.Services.Framework.Email
{
    public class EmailClient
    {
        public EmailClient()
        {
        }

        public void SendEmail(string to, string subject, string body)
        {
            SendEmail("gpulga@gmail.com", to, subject, body);
        }

        public void SendEmail(string from, string to, string subject, string body)
        {
            MailMessage mailMsg = new MailMessage();
            mailMsg.To.Add(to);
            // From
            MailAddress mailAddress = new MailAddress(from);
            mailMsg.From = mailAddress;

            // Subject and Body
            mailMsg.Subject = subject;
            mailMsg.Body = body;

            SmtpClient smtpClient = new SmtpClient();

            smtpClient.Send(mailMsg);
        }
    }
}
