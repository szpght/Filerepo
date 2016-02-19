using Nancy;
using System;
using System.IO;
using FileRepo.Model;
using Nancy.ModelBinding;

namespace FileRepo
{
    public class FileSaver
    {
        private readonly IRootPathProvider pathProvider;
        private readonly RepoContext db;
        private readonly string directory;

        public FileSaver(IRootPathProvider pathProvider, RepoContext db)
        {
            this.pathProvider = pathProvider;
            this.db = db;

            directory = GetDirectoryPath();
            EnsureDirectoryExists();
        }

        public void SaveFiles(int subjectId, NancyModule module)
        {
            foreach(var file in module.Context.Request.Files)
            {
                SaveFile(file, subjectId, module);
            }
            db.SaveChanges();
        }

        private void SaveFile(HttpFile file, int subjectId, NancyModule module)
        {
            var fileName = Guid.NewGuid().ToString();
            var filePath = Path.Combine(directory, fileName);
            WriteFileContent(file, filePath);
            var itemFromRequest = module.Bind<Item>();
            var item = new Item
            {
                UserId = (module.Context.CurrentUser as User).Id,
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

        private void WriteFileContent(HttpFile file, string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.Value.CopyTo(fileStream);
            }
        }

        private void EnsureDirectoryExists()
        {
            Directory.CreateDirectory(directory);
        }

        private string GetDirectoryPath()
        {
            return Path.Combine(pathProvider.GetRootPath(), Config.FileUploadDirectory);
        }
    }
}
