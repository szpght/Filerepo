using Nancy;
using Nancy.TinyIoc;
using Nancy.SimpleAuthentication;
using SimpleAuthentication.Core;
using SimpleAuthentication.Core.Providers;
using Nancy.Bootstrapper;
using Nancy.Authentication.Forms;
using Nancy.Security;
using System;
using FileRepo.Auth;
using Nancy.Diagnostics;

namespace FileRepo
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper
                
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            var facebookProvider = new FacebookProvider(new ProviderParams { PublicApiKey = Config.FacebookAppId, SecretApiKey = Config.FacebookAppSecret });
            var authenticationProviderFactory = new AuthenticationProviderFactory();
            authenticationProviderFactory.AddProvider(facebookProvider);
            container.Register<IAuthenticationCallbackProvider>(new AuthenticationCallbackProvider());
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            // simple authentication configuration
            var formsAuthConfiguration =
                new FormsAuthenticationConfiguration()
                {
                    RedirectUrl = Config.LoginRedirectUrl,
                    UserMapper = container.Resolve<IUserMapper>(),
                };
            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = Config.DashboardPassword }; }
        }
    }
}