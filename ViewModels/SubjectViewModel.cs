using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRepo.Model;

namespace FileRepo.ViewModels
{
    public class SubjectViewModel
    {
        public Subject Subject { get; set; }
        public List<Item> Items { get; set; }
    }
}
