using System;
using System.Threading.Tasks;
using System.Net.Mail;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using kroniiapi.DTO.Email;

namespace kroniiapi.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;
        private readonly IWebHostEnvironment _env;

        public EmailService(EmailConfig emailConfig, IWebHostEnvironment env)
        {
            this._emailConfig = emailConfig;
            this._env = env;
        }

        /// <summary>
        /// Send an auto email to selected mail address
        /// </summary>
        /// <param name="emailContent">Class include receiver information</param>
        /// <returns></returns>
        public async Task SendEmailAsync(EmailContent emailContent)
        {
            try
            {
                // Mail service config
                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                // Ready mail template
                string filePath = _env.WebRootPath + "/beefree/beefree.html";
                StreamReader str = new StreamReader(filePath);
                string mailTemplate = await str.ReadToEndAsync();
                str.Close();

                mailTemplate = mailTemplate.Replace("[@mailcontent@]", emailContent.Body.Trim());

                // Mail content config
                mail.From = new MailAddress(_emailConfig.MailAddress,"Kronii");
                mail.To.Add(emailContent.ToEmail);
                mail.Subject = emailContent.Subject;
                mail.IsBodyHtml = true;
                mail.Body = mailTemplate;

                // Port & Login to Mail account
                smtpClient.Port = _emailConfig.MailPort;
                smtpClient.Credentials = new System.Net.NetworkCredential(_emailConfig.MailAddress, _emailConfig.MailPassword);
                smtpClient.EnableSsl = true;

                // Send email
                await smtpClient.SendMailAsync(mail);
            }
            catch
            {
                throw new Exception("Email send failed");
            }
        }
    }
}