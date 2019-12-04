using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BabouExtensions;
using BabouMail.Common;

namespace BabouMail.Smtp
{
    public class SmtpClient
    {
        private string SmtpServer { get; set; }
        private int SmtpPort { get; set; }
        private string SmtpUsername { get; set; }
        private string SmtpPassword { get; set; }
        private bool SmtpSsl { get; set; }

        /// <summary>
        /// Instantiates the object to use an SMTP server.
        /// </summary>
        /// <param name="smtpServer">SMTP server hostname or IP address.</param>
        /// <param name="smtpPort">SMTP server port number.</param>
        /// <param name="smtpUsername">Username for the SMTP server.</param>
        /// <param name="smtpPassword">Password for the SMTP server.</param>
        /// <param name="smtpSsl">Enable or disable SSL.</param>
        public SmtpClient(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, bool smtpSsl)
        {
            smtpServer.ThrowIfNullOrEmpty(nameof(smtpServer));
            smtpPort.ThrowIfNull(nameof(smtpPort));
            smtpUsername.ThrowIfNullOrEmpty(nameof(smtpUsername));
            smtpPassword.ThrowIfNullOrEmpty(nameof(smtpPassword));
            smtpSsl.ThrowIfNull(nameof(smtpSsl));

            if (smtpPort <= 0)
                throw new ArgumentOutOfRangeException(nameof(smtpPort));

            SmtpServer = smtpServer;
            SmtpUsername = smtpUsername;
            SmtpPassword = smtpPassword;
            SmtpPort = smtpPort;
            SmtpSsl = smtpSsl;
        }

        /// <summary>
        /// Sends the email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> SendMailAsync(Email email)
        {
            email.ThrowIfNull(nameof(email));

            if (!email.IsValid())
            {
                throw new ArgumentException("Email object is not valid..");
            }

            using (var msg = new MailMessage())
            {
                msg.From = email.FromMailAddress ?? new MailAddress(email.FromAddress);

                if (email.ToMailAddresses != null && email.ToMailAddresses.Any())
                {
                    email.ToMailAddresses.ForEach(toAddress =>
                    {
                        msg.To.Add(toAddress);
                    });
                }
                else
                {
                    msg.To.Add(email.ToAddress);
                }

                msg.Subject = email.Subject;

                if (email.CcAddresses != null && email.CcAddresses.Any())
                {
                    email.CcAddresses.ForEach(cc =>
                    {
                        msg.CC.Add(cc);
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(email.CcAddress))
                        msg.CC.Add(email.CcAddress);
                }

                if (email.BccAddresses != null && email.BccAddresses.Any())
                {
                    email.BccAddresses.ForEach(bcc =>
                    {
                        msg.Bcc.Add(bcc);
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(email.BccAddress))
                        msg.Bcc.Add(email.BccAddress);
                }

                if (!string.IsNullOrEmpty(email.ReplyAddress) || email.ReplyMailAddress != null)
                    msg.ReplyToList.Add(email.ReplyMailAddress ?? new MailAddress(email.ReplyAddress));

                msg.IsBodyHtml = email.IsHtml;
                msg.Body = email.Body;

                if (!string.IsNullOrEmpty(email.AttachmentData))
                {
                    var bytes = Encoding.UTF8.GetBytes(email.AttachmentData);
                    var attachmentStream = new MemoryStream(bytes);
                    msg.Attachments.Add(new Attachment(attachmentStream, email.AttachmentName, email.AttachmentContentType));
                    attachmentStream.Dispose();
                }

                using (var smtp = new System.Net.Mail.SmtpClient(SmtpServer, SmtpPort))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = SmtpSsl;
                    smtp.Credentials = new NetworkCredential(SmtpUsername, SmtpPassword, null);

                    await smtp.SendMailAsync(msg);
                }
            }

            return true;
        }
    }
}
