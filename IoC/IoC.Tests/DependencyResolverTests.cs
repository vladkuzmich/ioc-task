﻿using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using IoC.Data.Contracts;
using IoC.Business.Contracts;
using IoC.API;

namespace IoC.Tests
{
    public class DependencyResolverTests
    {
        private IHost _webHost;

        [SetUp]
        public void Setup()
        {
            _webHost = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).Build();
        }

        [TestCase(typeof(IBaseRepository<>))]
        [TestCase(typeof(ITestService))]
        public void GetService_WhenResolveDependencies_InstancesShouldNotBeNull(Type typeUnderTest)
        {
            using (var scope = _webHost.Services.CreateScope())
            {
                foreach (var type in Assembly.GetAssembly(typeUnderTest)
                    .GetTypes().Where(x => x.IsInterface && !x.IsGenericType))
                {
                    try
                    {
                        var instance = scope.ServiceProvider.GetService(type);
                        Assert.NotNull(instance);
                    }
                    catch (Exception e)
                    {
                        Assert.Fail(e.Message);
                    }
                }
            }
        }
    }
}