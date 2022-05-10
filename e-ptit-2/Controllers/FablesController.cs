using e_ptit_2.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace e_ptit_2.Controllers
{
    public class FablesController : Controller
    {
        private EPtitDataClassesDataContext db = new EPtitDataClassesDataContext();
        Helpers.CommonFunctions CFunc = new Helpers.CommonFunctions();

        // GET: Proverbs
        public ActionResult Index()
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<Proverb>(c => c.ProverbContents);
            db.LoadOptions = dlo;
            List<Proverb> ProverbObj = db.Proverbs.Where(x => x.fkEditionId <= CFunc.GetPublishedEdition()).OrderByDescending(x => x.fkEditionId).ToList();

            return View(ProverbObj);
        }

        public ActionResult Read(int Id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<Proverb>(c => c.ProverbContents);
            db.LoadOptions = dlo;
            Proverb ProverbObj = db.Proverbs.Where(x => x.pkProverbId == Id).FirstOrDefault() as Proverb;

            ProverbViewModel vm = new ProverbViewModel(ProverbObj);
            vm.RelatedProverbs = db.Proverbs.Where(x => x.fkEditionId <= CFunc.GetPublishedEdition() && x.pkProverbId != Id)
                .OrderByDescending(p => p.pkProverbId).Take(4).ToList();

            ViewBag.Title = ProverbObj.title;

            return View(vm);
        }
    }
}