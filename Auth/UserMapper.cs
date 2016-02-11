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
    public class UserMapper : IUserMapper
    {
        private RepoContext db;

        public UserMapper(RepoContext db)
        {
            this.db = db;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            var user = from u in db.Users
                        where u.Guid == identifier
                        select u;
            if (user.Any())
                return user.Single();
            return null;
        }

        public User GetUserFromFbId(string fbId)
        {
            var user = from u in db.Users
                        where u.UserName == fbId
                        select u;
            if (user.Any())
                return user.Single();
            return null;
        }

        public User RegisterUser(UserInformation info)
        {
            var user = new User(info.Id, info.Name);
            db.Users.Add(user);
            db.SaveChanges();
            return user;
        }
    }
}
