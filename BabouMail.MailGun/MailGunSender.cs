using System;
using System.Threading;
using System.Threading.Tasks;
using BabouExtensions;
using BabouMail.Common.Interfaces;
using BabouMail.Common.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Extensions;

namespace BabouMail.MailGun
{
    public class MailGunSender : IBabouSender
    {
        private readonly string _domainName;
        private readonly RestClient _restClient;

        /// <summary>
        /// Initiate the MailGun Sender
        /// </summary>
        /// <param name="domainName">Domain Name registered with MailGun</param>
        /// <param name="apiKey">ApiKey from MailGun</param>
        /// <param name="mailGunRegion">The MailGun Region. Defaults to USA.</param>
        /// <exception cref="ArgumentException"></exception>
        public MailGunSender(string domainName, string apiKey, MailGunRegion mailGunRegion = MailGunRegion.USA)
        {
            apiKey.ThrowIfNullOrEmpty(nameof(apiKey));
            domainName.ThrowIfNullOrEmpty(nameof(domainName));

            _domainName = domainName;

            var url = string.Empty;
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

        public SendResponse Send(IBabouEmail email, CancellationToken? token = null)
        {
            return SendAsync(email, token).GetAwaiter().GetResult();
        }

        public async Task<SendResponse> SendAsync(IBabouEmail email, CancellationToken? token = null)
        {
            var request = new RestRequest();
            request.AddParameter("domain", _domainName, ParameterType.UrlSegment);
            request.Resource = _domainName + "/messages";

            request.AddParameter("from", $"{email.EmailData.FromAddress.Name} <{email.EmailData.FromAddress.EmailAddress}>");

            email.EmailData.ToAddresses.ForEach(x => {
                request.AddParameter("to", $"{x.Name} <{x.EmailAddress}>");
            });

            email.EmailData.CcAddresses.ForEach(x => {
                request.AddParameter("cc", $"{x.Name} <{x.EmailAddress}>");
            });

            email.EmailData.BccAddresses.ForEach(x => {
                request.AddParameter("bcc", $"{x.Name} <{x.EmailAddress}>");
            });

            email.EmailData.ReplyToAddresses.ForEach(x => {
                request.AddParameter("h:Reply-To", $"{x.Name} <{x.EmailAddress}>");
            });

            request.AddParameter("subject", email.EmailData.Subject);

            request.AddParameter(email.EmailData.IsHtml ? "html" : "text", email.EmailData.Body);

            if (!string.IsNullOrEmpty(email.EmailData.PlaintextAlternativeBody))
            {
                request.AddParameter("text", email.EmailData.PlaintextAlternativeBody);
            }

            email.EmailData.Tags.ForEach(x =>
            {
                request.AddParameter("o:tag", x);
            });

            foreach (var emailHeader in email.EmailData.Headers)
            {
                var key = emailHeader.Key;
                if (!key.StartsWith("h:"))
                {
                    key = "h:" + emailHeader.Key;
                }

                request.AddParameter(key, emailHeader.Value);
            }

            email.EmailData.Attachments.ForEach(x =>
            {
                var param = x.IsInline ? "inline" : "attachment";
                request.AddFileBytes(param, x.Data.ReadAsBytes(), x.Filename, x.ContentType);
            });

            request.Method = Method.POST;
            var restResponse = await _restClient.ExecuteTaskAsync(request);
            var mailGunResponse =  JsonConvert.DeserializeObject<MailGunResponse>(restResponse.Content);

            var result = new SendResponse { MessageId = mailGunResponse.Id };
            
            if (restResponse.IsSuccessful) 
                return result;

            result.ErrorMessages.Add(restResponse.ErrorMessage);
            return result;

        }
    }
}
