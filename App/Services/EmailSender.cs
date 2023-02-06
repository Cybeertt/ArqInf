using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ArqInf.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender()
        {

        }

        /// <summary>
        /// Função de envio de email assincrono com o corpo do email,o tema e a mensagem
        /// </summary>
        /// <return> Task Message do email na totalidade </return>
       
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string fromMail = "ArqInfEng@gmail.com";
            string fromPassword = "gdgavleqvcpccwfz";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = subject;
            message.To.Add(new MailAddress(email));
            message.Body = "<html><body> " + htmlMessage + " </body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };
            smtpClient.Send(message);
        }

    }
}
