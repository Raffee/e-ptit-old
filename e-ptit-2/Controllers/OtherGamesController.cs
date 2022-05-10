using e_ptit_2.Helpers;
using e_ptit_2.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace e_ptit_2.Controllers
{
    public class OtherGamesController : Controller
    {
        // GET: MathGames
        private readonly EPtitDataClassesDataContext _db = new EPtitDataClassesDataContext();
        private readonly CommonFunctions _cFunc = new CommonFunctions();

        public ActionResult Index()
        {
            var gameList = new List<GameViewModel>();
            var groups = _db.GameGroups.Where(x => (x.fkEditionId <= _cFunc.GetPublishedEdition()) 
                && (x.fkGameMenuTypeId == (int) PtitEnums.GameMenuType.Other)).
                OrderByDescending(x => x.SortOrder ?? 0).ToList();

            foreach (var group in groups)
            {
                var game = new GameViewModel
                {
                    Title = @group.GroupHeader,
                    Description = @group.ListingDescription,
                    Id = @group.pkGameGroupID,
                    LargeIcon = Helpers.Constants.GameIconsJamants.IconNames[@group.fkGameTypeId.Value],
                    Color = Helpers.Constants.GameTypeColors.ColorNames[@group.fkGameTypeId.Value],
                    GameTypeId = @group.fkGameTypeId.Value,
                    GameMenuTypeId = @group.fkGameMenuTypeId.Value
                };
                game.GameType = ((Helpers.PtitEnums.GameType) game.GameTypeId).ToString();
                game.GameMenuType = ((Helpers.PtitEnums.GameMenuType) game.GameMenuTypeId).ToString();
                game.URL = $"/OtherGames/Play/{group.pkGameGroupID}?TID={group.fkGameTypeId.Value}";

                gameList.Add(game);
            }

            return View(gameList);
        }

        public ActionResult Play(int id, int tid)
        {
            if (tid == (int) PtitEnums.GameType.FindDifference)
            {
                return PlayFindDifferenceGame(id);
            }
            else if (tid == (int) PtitEnums.GameType.Maze)
            {
                return PlayMazeGame(id);
            }
            else if (tid == (int) PtitEnums.GameType.WriteAnswer)
            {
                return PlayWriteAnswerGame(id);
            }
            else if (tid == (int) PtitEnums.GameType.SelectOne || tid == (int) Helpers.PtitEnums.GameType.SelectOneImages)
            {
                return PlaySelectOneGame(id, tid);
            }
            return View();
        }

        private ActionResult PlayFindDifferenceGame(int id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            _db.LoadOptions = dlo;

            GameGroup grp = _db.GameGroups.Where(gg => gg.pkGameGroupID == id).FirstOrDefault();

            if(grp != null)
            {
                GroupToGameInter inter = grp.GroupToGameInters.FirstOrDefault();

                if(inter != null)
                {
                    FindDifferenceGame game = _db.FindDifferenceGames.Where(x => x.pkFindDifferenceGameID == inter.fkGameId).FirstOrDefault();

                    if (game != null)
                    {
                        return View(viewName: "~/Views/Games/FindDifference.cshtml", model: game);
                    }
                }
            }

            return View();
        }

        private ActionResult PlayMazeGame(int id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            _db.LoadOptions = dlo;

            GameGroup grp = _db.GameGroups.Where(gg => gg.pkGameGroupID == id).FirstOrDefault();

            if (grp != null)
            {
                GroupToGameInter inter = grp.GroupToGameInters.FirstOrDefault();

                if (inter != null)
                {
                    Maze game = _db.Mazes.Where(x => x.pkMazeID == inter.fkGameId).FirstOrDefault();

                    if (game != null)
                    {
                        ViewBag.BackgroundColor = grp.BackgroundColor ?? "white";
                        return View(viewName: "~/Views/Games/Maze.cshtml", model: game);
                    }
                }
            }

            return View();
        }

        private ActionResult PlayWriteAnswerGame(int id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            _db.LoadOptions = dlo;

            GameGroup grp = _db.GameGroups.Where(gg => gg.pkGameGroupID == id).FirstOrDefault();

            if (grp != null)
            {
                var inters = grp.GroupToGameInters.ToList();

                if (inters != null && inters.Count > 0)
                {
                    WriteAnswerViewModel vm = new WriteAnswerViewModel();
                    List<WriteAnswerGame> games = new List<WriteAnswerGame>();
                    foreach (GroupToGameInter inter in inters)
                    {
                        WriteAnswerGame game = _db.WriteAnswerGames.Where(x => x.pkWriteAnswerGameId == inter.fkGameId).FirstOrDefault();
                        games.Add(game);
                    }
                    vm.ShowKeyboard = true;
                    vm.Games = games;

                    vm.Title = grp.GroupHeader;
                    vm.Description = grp.GroupDescription;

                    if (!String.IsNullOrEmpty(grp.BackgroungImage))
                        vm.Style = "background-image:url('../images/games/" + grp.BackgroungImage + "');";
                    if (!String.IsNullOrEmpty(grp.BackgroundColor))
                        vm.Style = "background-color: " + grp.BackgroundColor + ";";

                    return View(viewName: "~/Views/Games/WriteAnswer.cshtml", model: vm);
                }
            }

            return View();
        }

        private ActionResult PlaySelectOneGame(int id, int tid)
        {
            var dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            dlo.LoadWith<SelectOneGame>(c => c.SelectOneGameAnswers);
            _db.LoadOptions = dlo;

            var grp = _db.GameGroups.FirstOrDefault(gg => gg.pkGameGroupID == id);

            var inters = grp?.GroupToGameInters.ToList();

            if (inters == null || inters.Count <= 0) return View();
            var vm = new SelectOneViewModel();
            var games = inters.Select(inter => _db.SelectOneGames.FirstOrDefault(x => x.pkSelectOneGameID == inter.fkGameId)).ToList();
            vm.Games = games;
            vm.ShowAnswerButton = tid != (int) PtitEnums.GameType.SelectOneImages;

            vm.Title = grp.GroupHeader;
            vm.Description = grp.GroupDescription;

            if (!string.IsNullOrEmpty(grp.BackgroungImage))
                vm.Style = "background-image:url('../images/games/" + grp.BackgroungImage + "');";
            if (!string.IsNullOrEmpty(grp.BackgroundColor))
                vm.Style = "background-color: " + grp.BackgroundColor + ";";

            return View(viewName: "~/Views/Games/SelectOne.cshtml", model: vm);
        }

    }
}