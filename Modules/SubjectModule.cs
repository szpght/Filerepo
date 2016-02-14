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
        private readonly IRootPathProvider pathProvider;
        private readonly UserMapper userMapper;

        public SubjectModule(RepoContext db, IRootPathProvider pathProvider, UserMapper userMapper) : base("/repo/subject")
        {
            this.db = db;
            this.pathProvider = pathProvider;
            this.userMapper = userMapper;

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
                SaveFiles(subjectId, Request.Files);
                string path = string.Format("/repo/subject/{0}", subjectId);
                return Response.AsRedirect(path);
            };
        }

        private void SaveFiles(int subjectId, IEnumerable<HttpFile> files)
        {
            var uploadDir = Path.Combine(pathProvider.GetRootPath(), Config.FileUploadDirectory);
            Directory.CreateDirectory(uploadDir);
            foreach (var file in files)
            {
                var fileName = Guid.NewGuid().ToString();
                var filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.Value.CopyTo(fileStream);
                }
                var itemFromRequest = this.Bind<Item>();
                var item = new Item
                {
                    User = userMapper.GetUserFromFbId(Context.CurrentUser.UserName),
                    DateAdded = DateTime.Now,
                    Description = itemFromRequest.Description,
                    Notes = itemFromRequest.Notes,
                    Name = file.Name,
                    StoredName = fileName,
                    SubjectId = subjectId,
                    Size = file.Value.Length
                };
                db.Items.Add(item);
            }
            db.SaveChanges();
        }
    }
}
