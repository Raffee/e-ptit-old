using e_ptit_2.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace e_ptit_2.Controllers
{
    public class HomeController : Controller
    {
        private EPtitDataClassesDataContext db = new EPtitDataClassesDataContext();
        Helpers.CommonFunctions CFunc = new Helpers.CommonFunctions();

        public ActionResult Index()
        {
            HomePageDTO dto = new HomePageDTO();
            dto.Stories = GetCurrentEditionStories();
            dto.LanguageGames = GetCurrentLanguageGames();
            dto.Proverbs = GetCurrentEditionProverbs();
            dto.OtherGames = GetLatestOtherGames(4);
            return View(dto);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Download()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactViewModel contact)
        {
            if (ModelState.IsValid)
            {
                //SendMail(name, email, text, country);
            }
            return View();
        }

        public List<Story> GetCurrentEditionStories()
        {
            var storyObj = db.Stories.Where(x => x.fkEditionId == CFunc.GetPublishedEdition()).ToList();

            return storyObj;
        }

        public List<Proverb> GetCurrentEditionProverbs()
        {
            var proverbObj = db.Proverbs.Where(x => x.fkEditionId == CFunc.GetPublishedEdition()).ToList();

            return proverbObj;
        }

        public List<GameViewModel> GetCurrentLanguageGames()
        {
            var gameList = new List<GameViewModel>();
            var groups = db.GameGroups.Where(x => x.fkEditionId <= CFunc.GetPublishedEdition() && x.fkGameMenuTypeId == (int) Helpers.PtitEnums.GameMenuType.Language 
            && x.ShowOnHomePage && x.ShowOnHomePage)
                .OrderByDescending(gm => gm.SortOrder).Take(4).ToList();

            foreach(var group in groups)
            {
                var game = new GameViewModel
                {
                    Title = @group.GroupHeader,
                    Description = @group.ListingDescription,
                    Id = @group.pkGameGroupID,
                    LargeIcon = Helpers.Constants.GameIconsLarge.IconNames[@group.fkGameTypeId ?? 1],
                    GameTypeId = @group.fkGameTypeId ?? 1,
                    GameMenuTypeId = @group.fkGameMenuTypeId ?? 1
                };
                game.GameType = ((Helpers.PtitEnums.GameType) game.GameTypeId).ToString();
                game.GameMenuType = ((Helpers.PtitEnums.GameMenuType) game.GameMenuTypeId).ToString();

                gameList.Add(game);
            }

            return gameList;
        }

        public List<GameViewModel> GetLatestOtherGames(int count)
        {
            var gameList = new List<GameViewModel>();
            var groups = db.GameGroups.Where(x => x.fkEditionId <= CFunc.GetPublishedEdition() && x.fkGameMenuTypeId != (int) Helpers.PtitEnums.GameMenuType.Language 
            && x.ShowOnHomePage)
                .OrderByDescending(gm => gm.pkGameGroupID).Take(count).ToList();

            foreach (var group in groups)
            {
                var game = new GameViewModel
                {
                    Title = @group.GroupHeader,
                    Description = @group.ListingDescription,
                    Id = @group.pkGameGroupID,
                    LargeIcon = Helpers.Constants.GameIconsLarge.IconNames[@group.fkGameTypeId ?? 1],
                    GameTypeId = @group.fkGameTypeId ?? 1,
                    GameMenuTypeId = @group.fkGameMenuTypeId ?? 1
                };
                game.GameType = ((Helpers.PtitEnums.GameType) game.GameTypeId).ToString();
                game.GameMenuType = ((Helpers.PtitEnums.GameMenuType) game.GameMenuTypeId).ToString();

                gameList.Add(game);
            }

            return gameList;
        }

        protected void SendMail(string name, string email, string text, string country)
        {
            var mail = new MailMessage();
            var client = new SmtpClient("mail.smtp2go.com", 2525) //Port 8025, 587 and 25 can also be used.
            {
                Credentials = new System.Net.NetworkCredential("raffee.parseghian@gmail.com", "p@ssw0rdPISSO"),
                EnableSsl = true
            };
            mail.From = new MailAddress("info@e-ptit.com");
            mail.To.Add("Apr-vega@cyberia.net.lb");
            mail.CC.Add("raffee.parseghian@gmail.com");
            mail.CC.Add("ptit.vega@gmail.com");
            mail.Subject = "Contact Us - Message";

            var body = "Name: " + name + "\n";
            body += "Email: " + email + "\n";
            body += "Country: " + country + "\n";
            body += "Message: \n" + text + "\n";
            mail.Body = body;
            client.Send(mail);
        }
    }
}