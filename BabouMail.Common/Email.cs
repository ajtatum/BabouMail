using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using BabouExtensions;

namespace BabouMail.Common
{
    public class Email
    {
        /// <summary>
        /// The from email address where you can specify the from name and address.
        /// </summary>
        public MailAddress FromMailAddress { get; set; }

        /// <summary>
        /// The from email address.
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// The reply-to email address where you can specify the from name and address.
        /// </summary>
        public MailAddress ReplyMailAddress { get; set; }

        /// <summary>
        /// The reply-to email address.
        /// </summary>
        public string ReplyAddress { get; set; }

        /// <summary>
        /// The to email address where you can specify the from name and address.
        /// </summary>
        public List<MailAddress> ToMailAddresses { get; set; }

        /// <summary>
        /// The to email address, separated by a comma.
        /// </summary>
        public string ToAddress { get; set; }

        /// <summary>
        /// Email addresses to be carbon copied where you can specify the from name and address.
        /// </summary>
        public List<MailAddress> CcAddresses { get; set; }

        /// <summary>
        /// Email addresses to be carbon copied, separated by a comma.
        /// </summary>
        public string CcAddress { get; set; }

        /// <summary>
        /// Email addresses to be blind carbon copied where you can specify the from name and address.
        /// </summary>
        public List<MailAddress> BccAddresses { get; set; }

        /// <summary>
        /// Email addresses to be blind carbon copied, separated by a comma.
        /// </summary>
        public string BccAddress { get; set; }

        /// <summary>
        /// The subject of the message.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The body of the message.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Indicates whether or not the email is HTML.
        /// </summary>
        public bool IsHtml { get; set; }

        /// <summary>
        /// The name of the attachment.
        /// </summary>
        public string AttachmentName { get; set; }

        /// <summary>
        /// The attachment data.
        /// </summary>
        public string AttachmentData { get; set; }

        /// <summary>
        /// The content type of the attachment.
        /// </summary>
        public string AttachmentContentType { get; set; }

        /// <summary>
        /// Tests the validity of the object.
        /// </summary>
        /// <returns>True if valid.</returns>
        public bool IsValid()
        {
            if(FromAddress.IsNullOrEmpty() && FromMailAddress == null)
                throw new ArgumentException("FromAddress or FromMailAddress must contain a value.");

            if (ToAddress.IsNullOrEmpty() && (ToMailAddresses == null || !ToMailAddresses.Any()))
                throw new ArgumentException("ToAddress or ToMailAddress must contain a value.");

            Subject.ThrowIfNullOrEmpty(nameof(Subject));
            Body.ThrowIfNullOrEmpty(nameof(Body));
            return true;
        }
    }
}
