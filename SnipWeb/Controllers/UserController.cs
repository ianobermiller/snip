using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using Snip.Data;
using Snip.Models;

namespace Snip.Controllers
{
    public partial class UserController : BaseController
    {
        //
        // GET: /User/
        public virtual ActionResult Index()
        {
            var user = new User();
            var userName = GetUserName();
            using (var db = new SnipContext())
            {
                user.CreatedSnippets = 
                    (from s in
                        (from snippet in db.Snippets
                        where snippet.Creator.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                        orderby snippet.Modified descending
                         select new { 
                             Id = snippet.Id, 
                             Title = snippet.Title,
                             Created = snippet.Created,
                             Creator = snippet.Creator, 
                             Modified = snippet.Modified, 
                             ViewCount = snippet.ViewCount,
                             Expiration = snippet.Expiration }).AsEnumerable()
                     select new Snippet() { 
                         Id = s.Id, 
                         Title = s.Title,
                         Created = s.Created,
                         Creator = s.Creator,
                         Modified = s.Modified, 
                         ViewCount = s.ViewCount,
                         Expiration = s.Expiration}).ToList();
            }
            return View(user);
        }

        public virtual ActionResult Archive(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Snippet));
            using (var db = new SnipContext())
            {
                var userName = GetUserName();
                using (MemoryStream stream = new MemoryStream())
                {
                    using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create))
                    {
                        foreach (var snippet in db.Snippets.Where(s => s.Creator.Equals(userName, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            using (var entryStream = archive.CreateEntry(snippet.Id + "_" + snippet.GetDisplayName().GetFileNameSafeString().Trim() + ".xml").Open())
                            {
                                serializer.Serialize(entryStream, snippet);
                            }
                        }
                    }
                    HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                    return new FileContentResult(stream.ToArray(), "application/zip");
                }
            }
        }
    }
}