using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Snip.Models;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Snip.Data
{
    public class SnipContext : DbContext
    {
        public DbSet<Snippet> Snippets { get; set; }

        public SnipContext()
            : base("SnipDatabase")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Snippet>();
            entity.HasKey(s => s.Id);
            entity.Ignore(s => s.ExpirationInMinutes);
        }
    }
}