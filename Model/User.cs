using Nancy.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRepo.Model
{
    public class User : IUserIdentity
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string UserName { get; set; } // facebook id
        public string Name { get; set; }
        public bool Admin { get; set; }
        public bool? Allowed { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? VerificationDate { get; set; }
        public List<Item> Items { get; set; }

        public User(string facebookId, string name)
        {
            Guid = Guid.NewGuid();
            UserName = facebookId;
            Name = name;
            Admin = false;
            Allowed = null;
            RegistrationDate = DateTime.Now;
            VerificationDate = null;
        }

        public User() : base() { }

        [NotMapped]
        public string FbProfileURL
        {
            get
            {
                return string.Format("https://facebook.com/{0}", UserName);
            }
        }

        [NotMapped]
        public string FbPictureURL
        {
            get
            {
                return string.Format("https://graph.facebook.com/{0}/picture", UserName);
            }
        }

        [NotMapped]
        public IEnumerable<string> Claims
        {
            get
            {
                if (Allowed.HasValue && Allowed.Value) yield return "user";
                if (Admin) yield return "admin";
            }
        }
    }
}
