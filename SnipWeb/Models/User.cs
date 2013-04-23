using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Snip.Models
{
    public class User
    {
        private List<Snippet> createdSnippets = new List<Snippet>();
        public List<Snippet> CreatedSnippets
        {
            get { return createdSnippets; }
            set { createdSnippets = value; }
        }

        private List<Snippet> favoriteSnippets = new List<Snippet>();
        public List<Snippet> FavoriteSnippets
        {
            get { return favoriteSnippets; }
            set { favoriteSnippets = value; }
        }
    }
}