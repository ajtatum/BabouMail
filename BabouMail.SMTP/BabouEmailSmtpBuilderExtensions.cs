using BabouMail.Common.Interfaces;
using BabouMail.Smtp;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net;
using System.Net.Mail;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BabouEmailSmtpBuilderExtensions
    {
        public static BabouEmailServicesBuilder AddSmtpSender(this BabouEmailServicesBuilder builder, SmtpClient smtpClient)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<IBabouSender>(x => new SmtpSender(smtpClient)));
            return builder;
        }

        public static BabouEmailServicesBuilder AddSmtpSender(this BabouEmailServicesBuilder builder, string host, int port) => AddSmtpSender(builder, new SmtpClient(host, port));

        public static BabouEmailServicesBuilder AddSmtpSender(this BabouEmailServicesBuilder builder, string host, int port, string username, string password) => AddSmtpSender(builder,
             new SmtpClient(host, port) { EnableSsl = true, Credentials = new NetworkCredential (username, password) });
        
        public static BabouEmailServicesBuilder AddSmtpSender(this BabouEmailServicesBuilder builder, Func<SmtpClient> clientFactory)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<IBabouSender>(x => new SmtpSender(clientFactory)));
            return builder;
        }
    }
}
