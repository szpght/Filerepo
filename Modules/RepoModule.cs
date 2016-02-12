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
using Nancy.ModelBinding;

namespace FileRepo.Modules
{
    public class RepoModule : NancyModule
    {
        private readonly RepoContext db;

        public RepoModule(RepoContext db) : base("/repo")
        {
            this.db = db;

            //this.RequiresClaims("user");
            Get["/"] = parameters =>
            {
                ViewBag.Title = "test";
                var model = db.Terms
                    .Include(x => x.Subjects)
                    .OrderByDescending(x => x.TermNumber);
                return View["repo", model];
            };

            Get["/subject/{subject:int}"] = parameters =>
            {
                int subject = parameters.subject;
                var model = db.Items
                    .Include(x => x.User)
                    .Where(x => x.SubjectId == subject);
                return View["subject", model];
            };

            Get["/file/{id:int}"] = parameters =>
            {
                var model = GetItemFromId(parameters.id);
                return View["FileDetails", model];
            };

            Get["/file/{id:int}/edit"] = parameters =>
            {
                var model = GetItemFromId(parameters.id);
                return View["FileEdit", model];
            };
            
            Post["/file/{id:int}/edit"] = parameters =>
            {
                Item file = GetItemFromId(parameters.id);
                Item newFile = this.Bind();
                file.Description = newFile.Description;
                file.Notes = newFile.Notes;
                db.Items.Update(file);
                db.SaveChanges();
                string path = string.Format("/repo/file/{0}", parameters.id);
                return Response.AsRedirect(path);
            };

            Get["/file/{id:int}/delete"] = parameters =>
            {
                var model = GetItemFromId(parameters.id);
                return View["DeleteFile", model];
            };

            Post["/file/{id:int}/delete"] = parameters =>
            {
                Item file = GetItemFromId(parameters.id);
                db.Remove(file);
                db.SaveChanges();
                string path = string.Format("/repo/subject/{0}", file.SubjectId);
                return Response.AsRedirect(path);
            };
        }

        public Item GetItemFromId(int id)
        {
            var file = db.Items
                .Include(x => x.User)
                .Include(x => x.Subject)
                .Single(x => x.Id == id);
            return file;
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
