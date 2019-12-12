using BabouMail.Common.Interfaces;
using BabouMail.MailGun;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BabouEmailMailGunBuilderExtensions
    {
        public static BabouEmailServicesBuilder AddMailGunSender(this BabouEmailServicesBuilder builder, string domainName, string apiKey, MailGunRegion mailGunRegion = MailGunRegion.USA)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<IBabouSender>(x => new MailGunSender(domainName, apiKey, mailGunRegion)));
            return builder;
        }
    }
}
