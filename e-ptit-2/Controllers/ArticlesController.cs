using e_ptit_2.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace e_ptit_2.Controllers
{
    public class ArticlesController : Controller
    {
        private EPtitDataClassesDataContext db = new EPtitDataClassesDataContext();
        Helpers.CommonFunctions CFunc = new Helpers.CommonFunctions();

        // GET: Proverbs
        public ActionResult Index(int id)
        {
            List<Article> ArticlesObj = db.Articles.Where(x => x.fkEditionId <= CFunc.GetPublishedEdition() && x.Type == id).OrderByDescending(x => x.fkEditionId).ToList();

            if (id == 1)
                ViewBag.Title = "ԽՄԲԱԳՐԱԿԱՆ";
            else
                ViewBag.Title = "ԱՌՈՂՋԱՊԱՀԱԿԱՆ";

            return View(ArticlesObj);
        }
    }
}