using Microsoft.Extensions.DependencyInjection;
using System;
using BabouMail.Common;
using BabouMail.Common.Interfaces;

namespace FluentEmail.Core
{
    public class BabouEmailFactory : IBabouEmailFactory
    {
        private IServiceProvider services;

        public BabouEmailFactory(IServiceProvider services) => this.services = services;

        public IBabouEmail Create() => services.GetService<IBabouEmail>();
    }
}
