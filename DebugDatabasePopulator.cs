using System;
using System.Linq;
using FileRepo.Model;

namespace FileRepo
{
    public class DebugDatabasePopulator
    {
        private void Populate(RepoContext db)
        {
            var term = new Term
            {
                TermNumber = 1
            };
            db.Terms.Add(term);

            var subject = new Subject
            {
                Name = "Programowanie w C",
                Term = term
            };
            db.Subjects.Add(subject);

            var user = new User
            {
                Admin = true,
                Allowed = true,
                Guid = Guid.NewGuid(),
                Name = "Paweł Pietrasz",
                RegistrationDate = DateTime.Now,
                UserName = "559071237590207",
                VerificationDate = DateTime.Now,
            };
            db.Users.Add(user);

            db.SaveChanges();
        }

        public void EnsurePopulated()
        {
            var db = new RepoContext();
            if (!db.Terms.Any())
                Populate(db);
            db.Dispose();
        }
    }
}
