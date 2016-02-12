using System.IO;
using System.Linq;
using FileRepo.Model;
using Nancy;
using Nancy.ModelBinding;
using Microsoft.Data.Entity;

namespace FileRepo.Modules
{
    public class FileModule : NancyModule
    {
        private RepoContext db;

        public FileModule(RepoContext db) : base("/repo/file")
        {
            this.db = db;

            Get["/{id:int}"] = parameters =>
            {
                var model = GetItemFromId(parameters.id);
                return View["FileDetails", model];
            };

            Get["/{id:int}/edit"] = parameters =>
            {
                var model = GetItemFromId(parameters.id);
                return View["FileEdit", model];
            };

            Post["/{id:int}/edit"] = parameters =>
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

            Get["/{id:int}/delete"] = parameters =>
            {
                var model = GetItemFromId(parameters.id);
                return View["DeleteFile", model];
            };

            Post["/{id:int}/delete"] = parameters =>
            {
                Item file = GetItemFromId(parameters.id);
                db.Remove(file);
                db.SaveChanges();
                string path = string.Format("/repo/subject/{0}", file.SubjectId);
                return Response.AsRedirect(path);
            };

            Get["/{id:int}/download/{junk?}"] = parameters =>
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

        public static bool UserAllowedToEdit(NancyContext context, Item file)
        {
            if (context.CurrentUser.UserName == file.User.UserName)
                return true;
            return context.CurrentUser.Claims.Contains("admin"); // TODO good practice?
        }
    }
}