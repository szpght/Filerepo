using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAuthentication;
using Nancy.SimpleAuthentication;
using Nancy;
using Nancy.Authentication.Forms;

namespace FileRepo.Auth
{
    public class AuthenticationCallbackProvider : IAuthenticationCallbackProvider
    {
        public dynamic Process(NancyModule nancyModule, AuthenticateCallbackData model)
        {
            var info = model.AuthenticatedClient.UserInformation;
            var userMapper = new UserMapper();
            var user = userMapper.GetUserFromFbId(info.Id);
            if (user == null)
                user = userMapper.RegisterUser(info);
            return nancyModule.LoginAndRedirect(user.Guid);
        }

        // TODO
        public dynamic OnRedirectToAuthenticationProviderError(NancyModule nancyModule, string errorMessage)
        {
            return string.Format("<h1>ERROR</h1><pre>{0}</pre>", errorMessage);
        }
    }
}
