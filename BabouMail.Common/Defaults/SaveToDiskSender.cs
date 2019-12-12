using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BabouMail.Common.Interfaces;
using BabouMail.Common.Models;

namespace BabouMail.Common.Defaults
{
    public class SaveToDiskSender : IBabouSender
    {
        private readonly string _directory;

        public SaveToDiskSender(string directory)
        {
            _directory = directory;
        }

        public SendResponse Send(IBabouEmail email, CancellationToken? token = null)
        {
            return SendAsync(email, token).GetAwaiter().GetResult();
        }

        public async Task<SendResponse> SendAsync(IBabouEmail email, CancellationToken? token = null)
        {
            var response = new SendResponse();
            await SaveEmailToDisk(email);
            return response;
        }

        private async Task<bool> SaveEmailToDisk(IBabouEmail email)
        {
            var random = new Random();
            var filename = $"{_directory.TrimEnd('\\')}\\{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{random.Next(1000)}";

            using (var sw = new StreamWriter(File.OpenWrite(filename)))
            {
                sw.WriteLine($"From: {email.EmailData.FromAddress.Name} <{email.EmailData.FromAddress.EmailAddress}>");
                sw.WriteLine($"To: {string.Join(",", email.EmailData.ToAddresses.Select(x => $"{x.Name} <{x.EmailAddress}>"))}");
                sw.WriteLine($"Cc: {string.Join(",", email.EmailData.CcAddresses.Select(x => $"{x.Name} <{x.EmailAddress}>"))}");
                sw.WriteLine($"Bcc: {string.Join(",", email.EmailData.BccAddresses.Select(x => $"{x.Name} <{x.EmailAddress}>"))}");
                sw.WriteLine($"ReplyTo: {string.Join(",", email.EmailData.ReplyToAddresses.Select(x => $"{x.Name} <{x.EmailAddress}>"))}");
                sw.WriteLine($"Subject: {email.EmailData.Subject}");
                foreach (var dataHeader in email.EmailData.Headers)
                {
                    sw.WriteLine($"{dataHeader.Key}:{dataHeader.Value}");
                }
                sw.WriteLine();
                await sw.WriteAsync(email.EmailData.Body);
            }

            return true;
        }
    }
}
