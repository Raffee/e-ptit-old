using e_ptit_2.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace e_ptit_2.Controllers
{
    public class MathGamesController : Controller
    {
        // GET: MathGames
        private readonly EPtitDataClassesDataContext _db = new EPtitDataClassesDataContext();
        readonly Helpers.CommonFunctions _cFunc = new Helpers.CommonFunctions();

        public ActionResult Index()
        {
            var gameList = new List<GameViewModel>();
            var groups = _db.GameGroups.Where(x => (x.fkEditionId <= _cFunc.GetPublishedEdition()) 
                && (x.fkGameMenuTypeId == (int) Helpers.PtitEnums.GameMenuType.Math)).
                OrderByDescending(x => x.SortOrder ?? 0).ToList();

            foreach (var group in groups)
            {
                var game = new GameViewModel
                {
                    Title = @group.GroupHeader,
                    Description = @group.ListingDescription,
                    Id = @group.pkGameGroupID,
                    LargeIcon = Helpers.Constants.GameIconsMath.IconNames[@group.fkGameTypeId.Value],
                    Color = Helpers.Constants.GameTypeColors.ColorNames[@group.fkGameTypeId.Value],
                    GameTypeId = @group.fkGameTypeId.Value,
                    GameMenuTypeId = @group.fkGameMenuTypeId.Value
                };
                game.GameType = ((Helpers.PtitEnums.GameType) game.GameTypeId).ToString();
                game.GameMenuType = ((Helpers.PtitEnums.GameMenuType) game.GameMenuTypeId).ToString();
                game.URL = $"/MathGames/Play/{group.pkGameGroupID}?TID={group.fkGameTypeId.Value}";

                gameList.Add(game);
            }

            return View(gameList);
        }

        public ActionResult Play(int Id, int TID)
        {
            if (TID == (int) Helpers.PtitEnums.GameType.Crossword)
            {
                return PlayCrosswordGame(Id);
            }
            else if (TID == (int) Helpers.PtitEnums.GameType.Pyramid)
            {
                return PlayPyramidGame(Id);
            }
            else if (TID == (int) Helpers.PtitEnums.GameType.WriteAnswer)
            {
                return PlayWriteAnswerGame(Id);
            }
            else if (TID == (int) Helpers.PtitEnums.GameType.Matching)
            {
                return PlayMatchingGame(Id);
            }
            return View();
        }

        private ActionResult PlayCrosswordGame(int Id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            dlo.LoadWith<CrosswordGame>(c => c.CrosswordGameWords);
            dlo.LoadWith<CrosswordGame>(c => c.CrosswordGameSpecialCells);
            _db.LoadOptions = dlo;

            GameGroup grp = _db.GameGroups.Where(gg => gg.pkGameGroupID == Id).FirstOrDefault();

            if(grp != null)
            {
                GroupToGameInter inter = grp.GroupToGameInters.FirstOrDefault();

                if(inter != null)
                {
                    CrosswordGame game = _db.CrosswordGames.Where(x => x.CrosswordID == inter.fkGameId).FirstOrDefault();

                    if (game != null)
                    {
                        var ww = game.CrosswordGameWords.ToList();
                        var spCells = game.CrosswordGameSpecialCells.ToList();
                        foreach (CrosswordGameWord word in ww)
                        {
                            //word.fkCrosswordId = null;
                            word.CrosswordGame = null;
                            if (!String.IsNullOrEmpty(word.hint))
                                word.hint.Replace(" ", "&nbsp; ");
                        }
                        foreach (CrosswordGameSpecialCell cell in spCells)
                        {
                            //cell.fkCrosswordGameId = null;
                            cell.CrosswordGame = null;
                        }

                        CrosswordViewModel vm = new CrosswordViewModel();
                        vm.Crossword = game;
                        vm.Rows = game.Rows ?? 0;
                        vm.Cols = game.Cols ?? 0;
                        vm.isSeparateHints = game.SeparateHints;
                        vm.ShowKeyboard = game.showKeyboard;
                        vm.words = Newtonsoft.Json.JsonConvert.SerializeObject(ww);
                        vm.otherCells = Newtonsoft.Json.JsonConvert.SerializeObject(spCells);
                        vm.HintsIn2Columns = game.HintsIn2Columns;

                        return View(viewName: "~/Views/Games/Crossword.cshtml", model: vm);
                    }
                }
            }

            return View();
        }

        private ActionResult PlayPyramidGame(int Id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            dlo.LoadWith<PyramidGame>(c => c.PyramidEntries);
            _db.LoadOptions = dlo;

            GameGroup grp = _db.GameGroups.Where(gg => gg.pkGameGroupID == Id).FirstOrDefault();

            if (grp != null)
            {
                GroupToGameInter inter = grp.GroupToGameInters.FirstOrDefault();

                if (inter != null)
                {
                    PyramidGame game = _db.PyramidGames.Where(x => x.pkPyramidID == inter.fkGameId).FirstOrDefault();

                    if (game != null)
                    {
                        var ww = game.PyramidEntries.ToList();

                        foreach(var entry in ww)
                        {
                            entry.PyramidGame = null;
                        }

                        PyramidViewModel vm = new PyramidViewModel();
                        vm.Pyramid = game;
                        vm.PyramidEntries = ww;
                        vm.Rows = game.Height ?? 0;
                        vm.words = Newtonsoft.Json.JsonConvert.SerializeObject(ww);
                        vm.padding = (game.Width ?? 0 - 2.5) * game.CellWidth ?? 50;

                        return View(viewName: "~/Views/Games/Pyramid.cshtml", model: vm);
                    }
                }
            }

            return View();
        }

        private ActionResult PlayWriteAnswerGame(int id)
        {
            var dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            _db.LoadOptions = dlo;

            var grp = _db.GameGroups.FirstOrDefault(gg => gg.pkGameGroupID == id);

            if (grp == null) return View();
            var inters = grp.GroupToGameInters.ToList();

            if (inters.Count <= 0) return View();

            var vm = new WriteAnswerViewModel();
            var games = inters.Select(inter => _db.WriteAnswerGames.FirstOrDefault(x => x.pkWriteAnswerGameId == inter.fkGameId)).ToList();

            vm.ShowKeyboard = false;
            vm.Games = games;

            vm.Title = grp.GroupHeader;
            vm.Description = grp.GroupDescription;
            vm.ItemsPerRow = grp.ItemsPerRow ?? 1;

            if (!string.IsNullOrEmpty(grp.BackgroungImage))
                vm.Style += " background-image:url('../../Content/images/games/" + grp.BackgroungImage + "');";
            if (!string.IsNullOrEmpty(grp.BackgroundColor))
                vm.Style += " background-color: " + grp.BackgroundColor + ";";
            if (!string.IsNullOrEmpty(grp.BackgroundStyle))
                vm.Style += grp.BackgroundStyle;

            return View(viewName: "~/Views/Games/WriteAnswer.cshtml", model: vm);
        }

        private ActionResult PlayMatchingGame(int Id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            dlo.LoadWith<MatchingGame>(c => c.MatchingGameQuestions);
            dlo.LoadWith<MatchingGame>(c => c.MatchingGameAnswers);
            _db.LoadOptions = dlo;

            GameGroup grp = _db.GameGroups.Where(gg => gg.pkGameGroupID == Id).FirstOrDefault();

            if (grp != null)
            {
                GroupToGameInter inter = grp.GroupToGameInters.FirstOrDefault();

                if (inter != null)
                {
                    MatchingGame game = _db.MatchingGames.Where(x => x.pkMatchingGameID == inter.fkGameId).FirstOrDefault();

                    if (game != null)
                    {
                        var vm = new MatchingViewModel();
                        vm.Game = game;

                        if (game.Direction == 1)
                            return View(viewName: "~/Views/Games/Matching.cshtml", model: vm);
                        if (game.Direction == 2)
                            return View(viewName: "~/Views/Games/MatchingHorizontal.cshtml", model: vm);
                    }
                }
            }

            return View();
        }
    }
}