using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabouExtensions;
using BabouMail.Common;
using RestSharp;
using RestSharp.Authenticators;

namespace BabouMail.MailGun
{
    public class MailGun
    {
        private readonly string _apiKey;
        private readonly string _domainName;
        private readonly RestClient _restClient;

        public MailGun(string domainName, string apiKey, MailGunRegion mailGunRegion = MailGunRegion.USA)
        {
            apiKey.ThrowIfNullOrEmpty(nameof(apiKey));
            domainName.ThrowIfNullOrEmpty(nameof(domainName));

            _apiKey = apiKey;
            _domainName = domainName;

            string url = string.Empty;
            switch (mailGunRegion)
            {
                case MailGunRegion.USA:
                    url = $"https://api.mailgun.net/v3";
                    break;
                case MailGunRegion.EU:
                    url = $"https://api.eu.mailgun.net/v3";
                    break;
                default:
                    throw new ArgumentException($"'{mailGunRegion}' is not a valid value for {nameof(mailGunRegion)}");
            }

            _restClient = new RestClient
            {
                BaseUrl = new Uri(url),
                Authenticator = new HttpBasicAuthenticator("api", apiKey)
            };
        }

        public async Task<IRestResponse> SendMailAsync(Email email)
        {
            var request = new RestRequest();
            request.AddParameter("domain", _domainName, ParameterType.UrlSegment);
            request.Resource = _domainName + "/messages";
            request.AddParameter("from", email.FromMailAddress != null ? $"{email.FromMailAddress.DisplayName} <{email.FromMailAddress.Address}>" : $"{email.ToAddress}");

            if (email.ToMailAddresses != null && email.ToMailAddresses.Any())
            {
                foreach (var toAddress in email.ToMailAddresses)
                {
                    request.AddParameter("to", string.Join(",", $"{toAddress.DisplayName} <{toAddress.Address}>"));
                }
            }
            else
            {
                request.AddParameter("to", $"{email.ToAddress}");
            }

            if (!string.IsNullOrEmpty(email.ReplyAddress) || email.ReplyMailAddress != null)
                request.AddParameter("h:Reply-To", email.ReplyMailAddress != null ? $"{email.ReplyMailAddress.DisplayName} <{email.ReplyMailAddress.Address}>" : $"{email.ReplyAddress}");

            if (email.CcAddresses != null && email.CcAddresses.Any())
            {
                foreach (var ccAddress in email.CcAddresses)
                {
                    request.AddParameter("cc", string.Join(",", $"{ccAddress.DisplayName} <{ccAddress.Address}>"));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(email.CcAddress))
                    request.AddParameter("cc", email.CcAddress);
            }

            if (email.BccAddresses != null && email.BccAddresses.Any())
            {
                foreach (var bccAddress in email.BccAddresses)
                {
                    request.AddParameter("bcc", string.Join(",", $"{bccAddress.DisplayName} <{bccAddress.Address}>"));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(email.BccAddress))
                    request.AddParameter("bcc", email.BccAddress);
            }

            request.AddParameter("subject", email.Subject);

            request.AddParameter(email.IsHtml ? "html" : "text", email.Body);

            if (!string.IsNullOrEmpty(email.AttachmentData))
                request.AddFileBytes(
                    "attachment",
                    Encoding.UTF8.GetBytes(email.AttachmentData),
                    email.AttachmentName,
                    email.AttachmentContentType);

            request.Method = Method.POST;
            return await _restClient.ExecuteTaskAsync(request);
        }

        
    }
}
