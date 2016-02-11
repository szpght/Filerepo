using System;
using System.Linq;
using Nancy.Hosting.Self;
using Microsoft.Data.Entity;
using FileRepo.Model;

namespace FileRepo
{
    class Program
    {
        static void Main(string[] args)
        {
            /*var db = new RepoContext();
            var term = new Term
            {
                TermNumber = 1
            };
            var subject = new Subject
            {
                Name = "Programowanie w C",
                Term = term
            };
            var file = new Item
            {
                DateAdded = DateTime.Now,
                Description = "to jest taki dość przydługawy opisik pliczku wrzuconego przez studencika na serwerek",
                Name = "plik.jpg",
                Subject = subject,
                User = db.Users.First()
            };
            db.Terms.Add(term);
            db.Subjects.Add(subject);
            db.Items.Add(file);
            db.SaveChanges();*/

            /*var db = new RepoContext();
            var plik = db.Items.First();
            plik.Notes = "dupa";
            db.Update(plik);
            db.SaveChanges();*/

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
