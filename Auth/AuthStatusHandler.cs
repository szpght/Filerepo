using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.ErrorHandling;

namespace FileRepo.Auth
{
    public class AuthStatusHandler : IStatusCodeHandler
    {
        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            if (statusCode == HttpStatusCode.Forbidden)
                handleForbidden(context);
            else if (statusCode == HttpStatusCode.Unauthorized)
                handleUnauthorized(context);
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            if (statusCode == HttpStatusCode.Forbidden || statusCode == HttpStatusCode.Unauthorized)
                return true;
            return false;
        }

        private void handleUnauthorized(NancyContext context)
        {
            
        }

        private void handleForbidden(NancyContext context)
        {
            
        }
    }
}
