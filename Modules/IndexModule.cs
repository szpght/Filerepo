using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Authentication;
using Nancy.Security;
using System.Linq;

namespace FileRepo.Modules
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            var model = new { test1 = "test1", test2 = "test2" };
            Get["/"] = parameters =>
            {
                if (Context.CurrentUser == null)
                    return View["index", model];
                else
                    return Response.AsRedirect("/repo");
            };

            /*Get["/repo", ctx => ] = _ =>
            {
                
                this.RequiresAuthentication();
                if (Context.CurrentUser == null)
                {
                    return "user null";
                }
                else if (Context.CurrentUser.Claims.Any())
                    return Response.AsRedirect("/repo/")
                return "nie ma uprawnien";
            };*/
        }
    }
}