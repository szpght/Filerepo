using System.Linq;
using Nancy;
using Microsoft.Data.Entity;
using Nancy.Security;

namespace FileRepo.Modules
{
    public class RepoModule : NancyModule
    {
        public RepoModule(RepoContext db) : base("/repo")
        {
            this.RequiresClaims("user");
            Get["/"] = parameters =>
            {
                ViewBag.Title = "test";
                var model = db.Terms
                    .Include(x => x.Subjects)
                    .OrderByDescending(x => x.TermNumber);
                return View["repo", model];
            };
        }
    }
}
