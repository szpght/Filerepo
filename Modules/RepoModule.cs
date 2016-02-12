using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRepo.Auth;
using FileRepo.Model;
using FileRepo.ViewModels;
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
        private readonly IRootPathProvider pathProvider;
        private readonly UserMapper userMapper;

        public RepoModule(RepoContext db, IRootPathProvider pathProvider, UserMapper userMapper) : base("/repo")
        {
            this.db = db;
            this.pathProvider = pathProvider;
            this.userMapper = userMapper;

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

            Get["/subject/{id:int}/upload"] = parameters =>
            {
                int id = parameters.id;
                var subject = db.Subjects.Single(x => x.Id == id);
                return View["UploadFile", subject];
            };

            Post["/subject/{id:int}/upload"] = parameters =>
            {
                int subjectId = parameters.id;
                SaveFile(subjectId, Request.Files);
                string path = string.Format("/repo/subject/{0}", subjectId);
                return Response.AsRedirect(path);
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

            Get["/file/{id:int}/download/{junk?}"] = parameters =>
            {
                int id = parameters.id;
                Item file = db.Items.Single(x => x.Id == id);
                return Response.AsFile(Path.Combine(Config.FileUploadDirectory, file.StoredName));
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

        private void SaveFile(int subjectId, IEnumerable<HttpFile> files)
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
                db.SaveChanges();
            }
        }

        public static bool UserAllowedToEdit(NancyContext context, Item file)
        {
            return true; // TODO 
            if (context.CurrentUser.UserName == file.User.UserName)
                return true;
            return context.CurrentUser.Claims.Contains("admin"); // TODO good practice?
        }
    }
}
