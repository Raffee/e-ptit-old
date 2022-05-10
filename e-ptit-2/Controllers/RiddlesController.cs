using e_ptit_2.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace e_ptit_2.Controllers
{
    public class RiddlesController : Controller
    {
        private EPtitDataClassesDataContext db = new EPtitDataClassesDataContext();
        Helpers.CommonFunctions CFunc = new Helpers.CommonFunctions();

        // GET: Proverbs
        public ActionResult Index()
        {
            DataLoadOptions dlo = new DataLoadOptions();
            List<Riddle> RiddleObj = db.Riddles.Where(x => x.fkEditionId <= CFunc.GetPublishedEdition()).OrderByDescending(x => x.fkEditionId).ToList();

            return View(RiddleObj);
        }
    }
}