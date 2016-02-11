using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRepo.Model;
using Nancy;
using Nancy.Security;
using Microsoft.Data.Entity;
using Nancy.Bootstrapper;

namespace FileRepo.Modules
{
    public class RepoModule : NancyModule
    {
        public RepoModule() : base("/repo")
        {
            //this.RequiresClaims("user");
            Get["/"] = parameters =>
            {
                ViewBag.Title = "test";
                var db = new RepoContext();
                var model = db.Terms
                    .Include(x => x.Subjects)
                    .OrderByDescending(x => x.TermNumber);
                return View["repo", model];
            };

            Get["/subject/{subject:int}"] = parameters =>
            {
                var db = new RepoContext();
                int subject = parameters.subject;
                var model = db.Items
                    .Include(x => x.User)
                    .Where(x => x.SubjectId == subject);
                return View["subject", model];
            };

            Get["/file/{file:int}"] = parameters =>
            {
                var db = new RepoContext();
                int file = parameters.file;
                var model = db.Items
                    .Include(x => x.User)
                    .Include(x => x.Subject)
                    .Single(x => x.Id == file);
                return View["FileDetails", model];
            };

            Get["/file/{file:int}/edit"] = parameters =>
            {
                var db = new RepoContext();
                int file = parameters.file;
                var model = db.Items
                    .Single(x => x.Id == file);
                return View["FileEdit", model];
            };
        }

        public static bool UserAllowedToEdit(NancyContext context, Item file)
        {
            return true;
            if (context.CurrentUser.UserName == file.User.UserName)
                return true;
            return context.CurrentUser.Claims.Contains("admin"); // TODO czy taki zapis to dobra praktyka?
        }
    }
}
