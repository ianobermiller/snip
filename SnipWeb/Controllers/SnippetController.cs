using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Snip.Data;
using Snip.Models;
using System.Web.Routing;

namespace Snip.Controllers
{
    public partial class SnippetController : BaseController
    {
        public virtual ActionResult Add()
        {
            return View(new Snippet());
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Add(Snippet snippet)
        {
            int statusCode = 200;
            string id = snippet.Id.OrIfEmpty(RandomUtils.GetString());
            var now = DateTime.UtcNow;

            snippet.Id = id;
            snippet.Created = now;
            snippet.Modified = now;
            snippet.LastView = now;
            snippet.Creator = GetUserName();

            if (snippet.ExpirationInMinutes > 0)
            {
                snippet.Expiration = now.AddMinutes(snippet.ExpirationInMinutes);
            }

            using (var db = new SnipContext())
            {
                if (db.Snippets.Any(s => s.Id == snippet.Id))
                {
                    statusCode = 409;
                }
                else
                {
                    db.Snippets.Add(snippet);
                    db.SaveChanges();
                }
            }

            if (Request["ajax"] != null)
            {
                Response.StatusCode = statusCode;
                return Json(new
                {
                    id = id,
                    url = Url.Action(Mvc.Snippet.Display(id), "http") // Make sure it puts the absolute url
                });
            }
            else
            {
                return RedirectToAction(Mvc.Snippet.Display(id));
            }
        }

        public virtual ActionResult Delete(string id)
        {
            int statusCode = 200;
            using (var db = new SnipContext())
            {
                Snippet snippet = db.Snippets.ById(id);

                if (snippet == null)
                {
                    statusCode = 404;
                }
                else if (!snippet.Creator.EqualsI(GetUserName()))
                {
                    statusCode = 403;
                }
                else
                {
                    db.Snippets.Remove(snippet);
                    db.SaveChanges();
                }
            }

            if (Request["ajax"] != null)
            {
                return new HttpStatusCodeResult(statusCode);
            }
            else
            {
                return RedirectToAction(Mvc.User.Index());
            }
        }

        public virtual ActionResult Display(string id)
        {
            Snippet snippet = null;

            using (var db = new SnipContext())
            {
                snippet = db.Snippets.ById(id);
                if (snippet != null)
                {
                    db.Database.ExecuteSqlCommand("UPDATE Snippets SET ViewCount = ViewCount + 1, LastView = GETDATE() WHERE (Id = {0})", snippet.Id);
                }
            }

            return View(snippet);
        }

        public virtual ActionResult Edit(string id)
        {
            Snippet snippet = null;

            using (var db = new SnipContext())
            {
                snippet = db.Snippets.ByIdAndCreator(id, GetUserName());
            }

            return View(snippet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Edit(Snippet modifiedSnippet)
        {
            if (ModelState.IsValid)
            {
                using (var db = new SnipContext())
                {
                    Snippet snippet = db.Snippets.ByIdAndCreator(modifiedSnippet.Id, GetUserName());
                    if (snippet == null)
                    {
                        ModelState.AddModelError(string.Empty, "The specified snippet id could not be found or you do not have permission to edit it.");
                    }
                    else
                    {
                        DateTime now = DateTime.UtcNow;
                        snippet.Modified = now;
                        snippet.Content = modifiedSnippet.Content;
                        snippet.Title = modifiedSnippet.Title;
                        snippet.Language = modifiedSnippet.Language;

                        if (modifiedSnippet.ExpirationInMinutes > 0)
                        {
                            snippet.Expiration = now.AddMinutes(modifiedSnippet.ExpirationInMinutes);
                        }
                        else if (modifiedSnippet.ExpirationInMinutes < 0)
                        {
                            snippet.Expiration = null;
                        }

                        db.SaveChanges();
                        return RedirectToAction(Mvc.Snippet.Display(snippet.Id));
                    }
                }
            }

            return View(modifiedSnippet);
        }

        [OutputCache(NoStore=true, Duration=0)]
        public virtual JsonResult IsAvailable(string id)
        {
            return Json(IsSnippetIdAvailable(id) ? (object)true : (object)"Id not available.", JsonRequestBehavior.AllowGet);
        }

        public bool IsSnippetIdAvailable(string id)
        {
            if (new[] { "delete", "edit", "isavailable", "mysnips", "about" }.Contains(id.ToLower()))
            {
                return false;
            }

            using (var db = new SnipContext())
            {
                if (db.Snippets.Any(s => s.Id == id))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
