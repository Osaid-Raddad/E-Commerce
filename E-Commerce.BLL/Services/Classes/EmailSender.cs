using E_Commerce.BLL.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services.Classes
{
    public class EmailSender : IEmailSender
    {

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("osaidislam1@gmail.com", "baui lcim gftr xaem")
            };

            return client.SendMailAsync(
                new MailMessage(from: "osaidislam1@gmail.com",
                                to: email,
                                subject,
                                htmlMessage
                                )
                               { IsBodyHtml = true });

        }
    }
}
