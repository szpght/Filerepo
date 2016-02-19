using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileRepo.Auth;
using FileRepo.Model;
using Nancy;
using Microsoft.Data.Entity;
using FileRepo.ViewModels;
using Nancy.ModelBinding;
using Nancy.Security;

namespace FileRepo.Modules
{
     public class SubjectModule : NancyModule
    {
        private readonly RepoContext db;
        private readonly FileSaver fileSaver;

        public SubjectModule(RepoContext db, FileSaver fileSaver) : base("/repo/subject")
        {
            this.db = db;
            this.fileSaver = fileSaver;

            this.RequiresClaims("user");

            Get["/{subject:int}"] = parameters =>
            {
                int subjectId = parameters.subject;
                var subject = db.Subjects
                    .Single(x => x.Id == subjectId);
                var items = db.Items
                    .Include(x => x.User)
                    .Where(x => x.SubjectId == subjectId)
                    .ToList();
                var viewModel = new SubjectViewModel
                {
                    Subject = subject,
                    Items = items
                };
                return View["subject", viewModel];
            };

            Get["/{id:int}/upload"] = parameters =>
            {
                int id = parameters.id;
                var subject = db.Subjects.Single(x => x.Id == id);
                return View["UploadFile", subject];
            };

            Post["/{id:int}/upload"] = parameters =>
            {
                int subjectId = parameters.id;
                fileSaver.SaveFiles(subjectId, this);
                string redirectPath = string.Format("/repo/subject/{0}", subjectId);
                return Response.AsRedirect(redirectPath);
            };
        }
    }
}
