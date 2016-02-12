﻿using System;
using Nancy.Hosting.Self;

namespace FileRepo
{
    class Program
    {
        static void Main(string[] args)
        {
            var populator = new DebugDatabasePopulator();
            populator.EnsurePopulated();

            var uri = new Uri("http://localhost:3579");

            var config = new HostConfiguration();
            config.UrlReservations.CreateAutomatically = true;

            using (var host = new NancyHost(config, uri))
            {
                host.Start();

                Console.WriteLine("Your application is running on " + uri);
                Console.WriteLine("Press any [Enter] to close the host.");
                Console.ReadLine();
            }
        }
    }
}
