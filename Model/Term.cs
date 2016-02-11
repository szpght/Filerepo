using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRepo.Model
{
    public class Term
    {
        [Key]
        public int Id { get; set; }
        public int TermNumber { get; set; }
        public List<Subject> Subjects { get; set; }
    }
}
