using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRepo.Model;

namespace FileRepo
{
    public class RepoContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Term> Terms { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename = db.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Term>().HasMany(x => x.Subjects);
            //modelBuilder.Entity<Subject>().HasOne(x => x.Term);
        }
    }
}
