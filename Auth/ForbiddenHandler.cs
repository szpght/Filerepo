using Nancy;
using Nancy.ErrorHandling;
using Nancy.ViewEngines;

namespace FileRepo.Auth
{
    public class ForbiddenHandler : IStatusCodeHandler
    {
        private IViewRenderer viewRenderer;

        public ForbiddenHandler(IViewRenderer viewRenderer)
        {
            this.viewRenderer = viewRenderer;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            context.Response = viewRenderer.RenderView(context, "Forbidden")
                .WithStatusCode(HttpStatusCode.Forbidden);
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.Forbidden;
        }
    }
}
