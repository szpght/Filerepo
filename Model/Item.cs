using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRepo.Model
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public Subject Subject { get; set; }
        public int SubjectId { get; set; }
        public DateTime DateAdded { get; set; }
        public string StoredName { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public long Size { get; set; }

        private static string[] suffixes = { "B", "KiB", "MiB", "GiB" };
        [NotMapped]
        public string SizeHumanReadable
        {
            get
            {
                if (Size == 0)
                    return "0 B";
                int suffixIndex = (int)Math.Log(Size, 1024);
                double normalizedSize = Size/Math.Pow(1024, suffixIndex);
                return string.Format("{0:0.#} {1}", normalizedSize, suffixes[suffixIndex]);
            }
        }
    }
}
