using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.Mvc;
using e_ptit_2.Models;

namespace e_ptit_2.Controllers
{
    public class StoriesController : Controller
    {
        private EPtitDataClassesDataContext db = new EPtitDataClassesDataContext();
        Helpers.CommonFunctions CFunc = new Helpers.CommonFunctions();
        // GET: Stories
        public ActionResult Index()
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<Story>(c => c.StoryContents);
            db.LoadOptions = dlo;
            List<Story> storyObj = db.Stories.Where(x => x.fkEditionId <= CFunc.GetPublishedEdition()).OrderByDescending(x => x.fkEditionId).ToList();

            return View(storyObj);
        }

        public ActionResult Read(int Id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<Story>(c => c.StoryContents);
            db.LoadOptions = dlo;
            Story storyObj = db.Stories.Where(x => x.pkStoryId == Id).FirstOrDefault() as Story;

            StoryViewModel vm = new StoryViewModel(storyObj);
            vm.RelatedStories = db.Stories.Where(x => x.fkEditionId <= CFunc.GetPublishedEdition() && x.pkStoryId != Id)
                .OrderByDescending(s => s.pkStoryId).Take(8).ToList();

            ViewBag.Title = storyObj.title;

            return View(vm);
        }
    }
}