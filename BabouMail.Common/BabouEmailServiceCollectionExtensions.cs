using System;
using BabouMail.Common;
using BabouMail.Common.Interfaces;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BabouEmailServiceCollectionExtensions
    {
        public static BabouEmailServicesBuilder AddBabouEmail(this IServiceCollection services, string defaultFromEmail, string defaultFromName = "")
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var builder = new BabouEmailServicesBuilder(services);
            services.TryAdd(ServiceDescriptor.Transient<IBabouEmail>(x => 
                new BabouEmail(x.GetService<IBabouSender>(), defaultFromEmail, defaultFromName)
            ));

            services.TryAddTransient<IBabouEmailFactory, BabouEmailFactory>();

            return builder;
        }
    }

    public class BabouEmailServicesBuilder
    {
        public IServiceCollection Services { get; private set; }

        internal BabouEmailServicesBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
