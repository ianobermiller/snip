using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Snip.Models;
using System.Data.Entity;

namespace Snip.Data
{
    public static class DbSetExtensions
    {
        public static Snippet ById(this DbSet<Snippet> dbSet, string id)
        {
            return dbSet.Where(s => s.Id == id).FirstOrDefault();
        }

        public static Snippet ByIdAndCreator(this DbSet<Snippet> dbSet, string id, string creator)
        {
            return dbSet.Where(s => s.Id == id && s.Creator.Equals(creator, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }
    }
}