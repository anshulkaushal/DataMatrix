using Microsoft.Data.Sqlite;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;

namespace DataMatrix.Services
{
    public interface ISqliteFormService
    {
        void SaveFormData(
            string fullName,
            string email,
            string subject,
            string message
        );

        void SendEmail(string mailBody);
        void SendThankYouEmail(string userEmail, string userName, string templates);
    }

    public class SqliteFormService : ISqliteFormService
    {
        private readonly string _connectionString;
        private readonly IPublishedContentQuery _contentQuery;

        public SqliteFormService(
        IConfiguration config,
        IPublishedContentQuery contentQuery,
        IUmbracoContextAccessor umbracoContextAccessor)
        {
            _connectionString = config.GetConnectionString("umbracoDbDSN");
            _contentQuery = contentQuery;
        }

        public void SaveFormData(string fullName, string email, string subject, string message)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO ContactUsForm 
                (FullName, Email, Subject, Message, SubmittedAt)
                VALUES 
                ($fullName, $email, $subject, $message, $submittedAt)
            ";

            command.Parameters.AddWithValue("$fullName", fullName);
            command.Parameters.AddWithValue("$email", email);
            command.Parameters.AddWithValue("$subject", subject);
            command.Parameters.AddWithValue("$message", message);
            command.Parameters.AddWithValue("$submittedAt", DateTime.UtcNow);

            command.ExecuteNonQuery();
        }

        private IPublishedContent? GetEmailSettings()
        {
            return _contentQuery
        .ContentAtRoot()
        .FirstOrDefault(x => x.ContentType.Alias == "smtpEmailCredentials");
        }


        public void SendEmail(string mailBody)
        {
            var settings = GetEmailSettings();
            if (settings == null) return;

            // SMTP settings from Umbraco
            var smtpHost = settings.Value<string>("smtpHost");      // smtpout.secureserver.net
            var smtpPort = settings.Value<int>("smtpPort");        // 587
            var smtpUser = settings.Value<string>("smtpUsername"); // info@datamatrixiq.com
            var smtpPass = settings.Value<string>("smtpPassword"); // EMAIL PASSWORD

            var senderEmail = settings.Value<string>("senderEmail");
            var adminEmail = settings.Value<string>("adminEmail");
            var fromName = settings.Value<string>("fromName");
            var subject = settings.Value<string>("emailSubject");

            // Build email
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, senderEmail));
            message.To.Add(MailboxAddress.Parse(adminEmail)); // Admin email
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = mailBody
            };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            client.Connect(
                smtpHost,
                smtpPort,
                SecureSocketOptions.SslOnConnect
            );

            client.Authenticate(smtpUser, smtpPass);
            client.Send(message);
            client.Disconnect(true);
        }

        public void SendThankYouEmail(string userEmail, string userName, string template)
        {
            var settings = GetEmailSettings();
            if (settings == null) return;

            var smtpHost = settings.Value<string>("smtpHost");
            var smtpPort = settings.Value<int>("smtpPort");
            var smtpUser = settings.Value<string>("smtpUsername");
            var smtpPass = settings.Value<string>("smtpPassword");

            var senderEmail = settings.Value<string>("senderEmail");
            var fromName = settings.Value<string>("fromName");

            // Build thank-you email
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, senderEmail));
            message.To.Add(MailboxAddress.Parse(userEmail));
            message.Subject = "Thank you for contacting DataMatrix";

            message.Body = new TextPart("html")
            {
                Text = template
            };

            using var client = new MailKit.Net.Smtp.SmtpClient();

            //  GoDaddy-safe settings
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Connect(smtpHost, smtpPort, SecureSocketOptions.SslOnConnect);
            client.Authenticate(smtpUser, smtpPass);
            client.Send(message);
            client.Disconnect(true);
        }

    }
}

