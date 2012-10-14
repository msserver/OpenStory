﻿using System;
using System.Threading;
using OpenStory.Server;
using OpenStory.Server.Auth.Data;
using OpenStory.Server.Data;
using OpenStory.Server.Fluent;
using OpenStory.Server.Modules.Logging;

namespace OpenStory.Services.Auth
{
    internal static class Program
    {
        private static void Main()
        {
            Console.Title = "OpenStory - Authentication Service";

            Initialize();

            string error;
            var service = Bootstrap.Service(() => new AuthService(), out error);
            if (error != null)
            {
                Console.WriteLine(error);
                Console.ReadLine();
                return;
            }

            using (service)
            {
                OS.Log().Info("Service registered.");
                Thread.Sleep(Timeout.Infinite);
            }
        }

        private static void Initialize()
        {
            OS.Initialize()
                .Logger(new ConsoleLogger())
                .Managers<DataManager>()
                    .DefaultManager(new AuthDataManager()).Done();
        }
    }
}
