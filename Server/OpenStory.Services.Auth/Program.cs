﻿using System;
using System.Threading;
using Ninject;
using OpenStory.Server;
using OpenStory.Server.Auth;
using OpenStory.Server.Fluent;
using OpenStory.Server.Modules.Logging;

namespace OpenStory.Services.Auth
{
    internal static class Program
    {
        private static void Main()
        {
            Console.Title = @"OpenStory - Authentication Service";

            var kernel = Initialize();

            string error;
            var service = Bootstrap.Service<AuthService>(kernel, out error);
            if (error != null)
            {
                Console.Title = @"OpenStory - Authentication Service - Error";
                
                Console.WriteLine(error);
                Console.ReadLine();
                return;
            }

            using (service)
            {
                Console.Title = @"OpenStory - Authentication Service - Running";
                
                Thread.Sleep(Timeout.Infinite);
            }
        }

        private static IKernel Initialize()
        {
            var kernel = new StandardKernel();
            kernel.Bind<ILogger>().To<ConsoleLogger>().InSingletonScope();
            kernel.Bind<AuthService>().ToSelf();

            OS.Initialize(kernel);

            return kernel;
        }
    }
}
