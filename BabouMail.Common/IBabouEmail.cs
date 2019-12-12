using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BabouMail.Common.Interfaces;
using BabouMail.Common.Models;

namespace BabouMail.Common
{
    public interface IBabouEmail
    {
		EmailData EmailData { get; set; }
        IBabouSender Sender { get; set; }

        /// <summary>
		/// Adds a recipient to the email, Splits name and address on ';'
		/// </summary>
		/// <param name="emailAddress">Email address of recipient</param>
		/// <param name="name">Name of recipient</param>
		/// <returns>Instance of the Email class</returns>
        IBabouEmail To(string emailAddress, string name = null);

		/// <summary>
		/// Set the send from email address
		/// </summary>
		/// <param name="emailAddress">Email address of sender</param>
		/// <param name="name">Name of sender</param>
		/// <returns>Instance of the Email class</returns>
		IBabouEmail SetFrom(string emailAddress, string name = null);

		/// <summary>
		/// Adds a recipient to the email
		/// </summary>
		/// <param name="emailAddress">Email address of recipient (allows multiple splitting on ';')</param>
		/// <returns></returns>
		IBabouEmail To(string emailAddress);

		/// <summary>
		/// Adds all recipients in list to email
		/// </summary>
		/// <param name="mailAddresses">List of recipients</param>
		/// <returns>Instance of the Email class</returns>
		IBabouEmail To(IList<Address> mailAddresses);

		/// <summary>
		/// Adds a Carbon Copy to the email
		/// </summary>
		/// <param name="emailAddress">Email address to cc</param>
		/// <param name="name">Name to cc</param>
		/// <returns>Instance of the Email class</returns>
		IBabouEmail CC(string emailAddress, string name = "");

		/// <summary>
		/// Adds all Carbon Copy in list to an email
		/// </summary>
		/// <param name="mailAddresses">List of recipients to CC</param>
		/// <returns>Instance of the Email class</returns>
		IBabouEmail CC(IList<Address> mailAddresses);

		/// <summary>
		/// Adds a blind carbon copy to the email
		/// </summary>
		/// <param name="emailAddress">Email address of bcc</param>
		/// <param name="name">Name of bcc</param>
		/// <returns>Instance of the Email class</returns>
		IBabouEmail BCC(string emailAddress, string name = "");

		/// <summary>
		/// Adds all blind carbon copy in list to an email
		/// </summary>
		/// <param name="mailAddresses">List of recipients to BCC</param>
		/// <returns>Instance of the Email class</returns>
		IBabouEmail BCC(IList<Address> mailAddresses);

		/// <summary>
		/// Sets the ReplyTo address on the email
		/// </summary>
		/// <param name="address">The ReplyTo Address</param>
		/// <returns></returns>
		IBabouEmail ReplyTo(string address);

		/// <summary>
		/// Sets the ReplyTo address on the email
		/// </summary>
		/// <param name="address">The ReplyTo Address</param>
		/// <param name="name">The Display Name of the ReplyTo</param>
		/// <returns></returns>
		IBabouEmail ReplyTo(string address, string name);

		/// <summary>
		/// Sets the subject of the email
		/// </summary>
		/// <param name="subject">email subject</param>
		/// <returns>Instance of the Email class</returns>
		IBabouEmail Subject(string subject);

		/// <summary>
		/// Adds a Body to the Email
		/// </summary>
		/// <param name="body">The content of the body</param>
		/// <param name="isHtml">Defaults to true</param>
		IBabouEmail Body(string body, bool isHtml = true);

		/// <summary>
		/// Marks the email as High Priority
		/// </summary>
		IBabouEmail HighPriority();

		/// <summary>
		/// Marks the email as Low Priority
		/// </summary>
		IBabouEmail LowPriority();

        /// <summary>
		/// Adds an Attachment to the Email
		/// </summary>
		/// <param name="attachment">The Attachment to add</param>
		/// <returns>Instance of the Email class</returns>
		IBabouEmail Attach(Attachment attachment);

		/// <summary>
		/// Adds Multiple Attachments to the Email
		/// </summary>
		/// <param name="attachments">The List of Attachments to add</param>
		/// <returns>Instance of the Email class</returns>
		IBabouEmail Attach(IList<Attachment> attachments);

		IBabouEmail AttachFromFilename(string filename, string contentType = null, string attachmentName = null);

		/// <summary>
		/// Adds a Plaintext alternative Body to the Email. Used in conjunction with an HTML email,
		/// this allows for email readers without html capability, and also helps avoid spam filters.
		/// </summary>
		/// <param name="body">The content of the body</param>
		IBabouEmail PlaintextAlternativeBody(string body);

		/// <summary>
		/// Adds tag to the Email. This is currently only supported by the Mailgun provider. <see href="https://documentation.mailgun.com/en/latest/user_manual.html#tagging"/>
		/// </summary>
		/// <param name="tag">Tag name, max 128 characters, ASCII only</param>
		/// <returns>Instance of the Email class</returns>
		IBabouEmail Tag(string tag);

		/// <summary>
		/// Adds header to the Email.
		/// </summary>
		/// <param name="header">Header name, only printable ASCII allowed.</param>
		/// <param name="body">value of the header</param>
		/// <returns>Instance of the Email class</returns>
		IBabouEmail Header(string header, string body);

        /// <summary>
        /// Sends email synchronously
        /// </summary>
        /// <returns>Instance of the Email class</returns>
        SendResponse Send(CancellationToken? token = null);

        /// <summary>
        /// Sends email asynchronously
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<SendResponse> SendAsync(CancellationToken? token = null);

        bool IsValid();
    }
}
