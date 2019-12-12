using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BabouMail.Common.Defaults;
using BabouMail.Common.Interfaces;
using BabouMail.Common.Models;

namespace BabouMail.Common
{
    public class BabouEmail : IBabouEmail
    {
        public EmailData EmailData { get; set; }
        public IBabouSender Sender { get; set; }

        public static IBabouSender DefaultSender = new SaveToDiskSender("/");

        /// <summary>
        /// Creates a new email instance with default settings.
        /// </summary>
        public BabouEmail() : this(DefaultSender) { }

        /// <summary>
        /// Creates a new email instance with overrides for the rendering and sending engines.
        /// </summary>
        /// <param name="sender">The email sending implementation</param>
        public BabouEmail(IBabouSender sender)
            : this(sender, null, null)
        {
        }

        /// <summary>
        ///  Creates a new Email instance with default settings, from a specific mailing address.
        /// </summary>
        /// <param name="emailAddress">Email address to send from</param>
        /// <param name="name">Name to send from</param>
        public BabouEmail(string emailAddress, string name = "")
            : this(DefaultSender, emailAddress, name) { }

        /// <summary>
        ///  Creates a new Email instance using the given engines and mailing address.
        /// </summary>
        /// <param name="sender">The email sending implementation</param>
        /// <param name="emailAddress">Email address to send from</param>
        /// <param name="name">Name to send from</param>
        public BabouEmail(IBabouSender sender, string emailAddress, string name = "")
        {
            EmailData = new EmailData()
            {
                FromAddress = new Address() { EmailAddress = emailAddress, Name = name }
            };
            Sender = sender;
        }

        /// <summary>
        /// Set the send from email address
        /// </summary>
        /// <param name="emailAddress">Email address of sender</param>
        /// <param name="name">Name of sender</param>
        /// <returns>Instance of the Email class</returns>
        public IBabouEmail From(string emailAddress, string name = null)
        {
            EmailData.FromAddress = new Address(emailAddress, name ?? "");
            return this;
        }

        /// <summary>
        /// Adds a recipient to the email, Splits name and address on ';'
        /// </summary>
        /// <param name="emailAddress">Email address of recipient</param>
        /// <param name="name">Name of recipient</param>
        /// <returns>Instance of the Email class</returns>
        public IBabouEmail To(string emailAddress, string name)
        {
            if (emailAddress.Contains(";"))
            {
                //email address has semi-colon, try split
                var nameSplit = name.Split(';');
                var addressSplit = emailAddress.Split(';');
                for (var i = 0; i < addressSplit.Length; i++)
                {
                    var currentName = string.Empty;
                    if ((nameSplit.Length - 1) >= i)
                    {
                        currentName = nameSplit[i];
                    }
                    EmailData.ToAddresses.Add(new Address(addressSplit[i].Trim(), currentName.Trim()));
                }
            }
            else
            {
                EmailData.ToAddresses.Add(new Address(emailAddress.Trim(), name?.Trim()));
            }
            return this;
        }

        /// <summary>
        /// Adds a recipient to the email
        /// </summary>
        /// <param name="emailAddress">Email address of recipient (allows multiple splitting on ';')</param>
        /// <returns></returns>
        public IBabouEmail To(string emailAddress)
        {
            if (emailAddress.Contains(";"))
            {
                foreach (var address in emailAddress.Split(';'))
                {
                    EmailData.ToAddresses.Add(new Address(address));
                }
            }
            else
            {
                EmailData.ToAddresses.Add(new Address(emailAddress));
            }

            return this;
        }

        /// <summary>
        /// Adds all recipients in list to email
        /// </summary>
        /// <param name="mailAddresses">List of recipients</param>
        /// <returns>Instance of the Email class</returns>
        public IBabouEmail To(IList<Address> mailAddresses)
        {
            foreach (var address in mailAddresses)
            {
                EmailData.ToAddresses.Add(address);
            }
            return this;
        }

        /// <summary>
        /// Adds a Carbon Copy to the email
        /// </summary>
        /// <param name="emailAddress">Email address to cc</param>
        /// <param name="name">Name to cc</param>
        /// <returns>Instance of the Email class</returns>
        public IBabouEmail CC(string emailAddress, string name = "")
        {
            EmailData.CcAddresses.Add(new Address(emailAddress, name));
            return this;
        }

        /// <summary>
        /// Adds all Carbon Copy in list to an email
        /// </summary>
        /// <param name="mailAddresses">List of recipients to CC</param>
        /// <returns>Instance of the Email class</returns>
        public IBabouEmail CC(IList<Address> mailAddresses)
        {
            foreach (var address in mailAddresses)
            {
                EmailData.CcAddresses.Add(address);
            }
            return this;
        }

        /// <summary>
        /// Adds a blind carbon copy to the email
        /// </summary>
        /// <param name="emailAddress">Email address of bcc</param>
        /// <param name="name">Name of bcc</param>
        /// <returns>Instance of the Email class</returns>
        public IBabouEmail BCC(string emailAddress, string name = "")
        {
            EmailData.BccAddresses.Add(new Address(emailAddress, name));
            return this;
        }

        /// <summary>
        /// Adds all blind carbon copy in list to an email
        /// </summary>
        /// <param name="mailAddresses">List of recipients to BCC</param>
        /// <returns>Instance of the Email class</returns>
        public IBabouEmail BCC(IList<Address> mailAddresses)
        {
            foreach (var address in mailAddresses)
            {
                EmailData.BccAddresses.Add(address);
            }
            return this;
        }

        /// <summary>
        /// Sets the ReplyTo address on the email
        /// </summary>
        /// <param name="address">The ReplyTo Address</param>
        /// <returns></returns>
        public IBabouEmail ReplyTo(string address)
        {
            EmailData.ReplyToAddresses.Add(new Address(address));

            return this;
        }

        /// <summary>
        /// Sets the ReplyTo address on the email
        /// </summary>
        /// <param name="address">The ReplyTo Address</param>
        /// <param name="name">The Display Name of the ReplyTo</param>
        /// <returns></returns>
        public IBabouEmail ReplyTo(string address, string name)
        {
            EmailData.ReplyToAddresses.Add(new Address(address, name));

            return this;
        }

        /// <summary>
        /// Sets the subject of the email
        /// </summary>
        /// <param name="subject">email subject</param>
        /// <returns>Instance of the Email class</returns>
        public IBabouEmail Subject(string subject)
        {
            EmailData.Subject = subject;
            return this;
        }

        /// <summary>
        /// Adds a Body to the Email
        /// </summary>
        /// <param name="body">The content of the body</param>
        /// <param name="isHtml">True if Body is HTML, false for plain text (default)</param>
        public IBabouEmail Body(string body, bool isHtml = false)
        {
            EmailData.IsHtml = isHtml;
            EmailData.Body = body;
            return this;
        }

        /// <summary>
        /// Adds a Plaintext alternative Body to the Email. Used in conjunction with an HTML email,
        /// this allows for email readers without html capability, and also helps avoid spam filters.
        /// </summary>
        /// <param name="body">The content of the body</param>
        public IBabouEmail PlaintextAlternativeBody(string body)
        {
            EmailData.PlaintextAlternativeBody = body;
            return this;
        }

        /// <summary>
        /// Marks the email as High Priority
        /// </summary>
        public IBabouEmail HighPriority()
        {
            EmailData.Priority = Priority.High;
            return this;
        }

        /// <summary>
        /// Marks the email as Low Priority
        /// </summary>
        public IBabouEmail LowPriority()
        {
            EmailData.Priority = Priority.Low;
            return this;
        }

        /// <summary>
        /// Adds an Attachment to the Email
        /// </summary>
        /// <param name="attachment">The Attachment to add</param>
        /// <returns>Instance of the Email class</returns>
        public IBabouEmail Attach(Attachment attachment)
        {
            if (!EmailData.Attachments.Contains(attachment))
            {
                EmailData.Attachments.Add(attachment);
            }

            return this;
        }

        /// <summary>
        /// Adds Multiple Attachments to the Email
        /// </summary>
        /// <param name="attachments">The List of Attachments to add</param>
        /// <returns>Instance of the Email class</returns>
        public IBabouEmail Attach(IList<Attachment> attachments)
        {
            foreach (var attachment in attachments.Where(attachment => !EmailData.Attachments.Contains(attachment)))
            {
                EmailData.Attachments.Add(attachment);
            }
            return this;
        }

        public IBabouEmail AttachFromFilename(string filename, string contentType = null, string attachmentName = null)
        {
            var stream = File.OpenRead(filename);
            Attach(new Attachment()
            {
                Data = stream,
                Filename = attachmentName ?? filename,
                ContentType = contentType
            });

            return this;
        }

        /// <summary>
        /// Adds tag to the Email. This is currently only supported by the Mailgun provider. <see href="https://documentation.mailgun.com/en/latest/user_manual.html#tagging"/>
        /// </summary>
        /// <param name="tag">Tag name, max 128 characters, ASCII only</param>
        /// <returns>Instance of the Email class</returns>
        public IBabouEmail Tag(string tag)
        {
            EmailData.Tags.Add(tag);

            return this;
        }

        public IBabouEmail Header(string header, string body)
        {
            EmailData.Headers.Add(header, body);

            return this;
        }

        /// <summary>
        /// Sends email synchronously
        /// </summary>
        /// <returns>Instance of the Email class</returns>
        public virtual SendResponse Send(CancellationToken? token = null)
        {
            return Sender.Send(this, token);
        }

        public virtual async Task<SendResponse> SendAsync(CancellationToken? token = null)
        {
            return await Sender.SendAsync(this, token);
        }

        public bool IsValid()
        {
            throw new System.NotImplementedException();
        }

        private static string GetCultureFileName(string fileName, CultureInfo culture)
        {
            var extension = Path.GetExtension(fileName);
            var cultureExtension = string.Format("{0}{1}", culture.Name, extension);

            var cultureFile = Path.ChangeExtension(fileName, cultureExtension);
            if (File.Exists(cultureFile))
                return cultureFile;
            else
                return fileName;
        }
    }
}
