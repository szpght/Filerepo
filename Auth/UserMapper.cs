using Nancy.Authentication.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Security;
using FileRepo.Model;
using SimpleAuthentication.Core;

namespace FileRepo.Auth
{
    class UserMapper : IUserMapper
    {
        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            using (var db = new RepoContext())
            {
                var user = from u in db.Users
                           where u.Guid == identifier
                           select u;
                if (user.Any())
                    return user.Single();
                return null;
            }
        }

        public User GetUserFromFbId(string fbId)
        {
            using (var db = new RepoContext())
            {
                var user = from u in db.Users
                           where u.UserName == fbId
                           select u;
                if (user.Any())
                    return user.Single();
                return null;
            }
        }

        public User RegisterUser(UserInformation info)
        {
            using (var db = new RepoContext())
            {
                var user = new User(info.Id, info.Name);
                db.Users.Add(user);
                db.SaveChanges();
                return user;
            }
        }
    }
}
