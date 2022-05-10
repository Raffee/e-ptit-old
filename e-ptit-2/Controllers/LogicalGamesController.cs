using e_ptit_2.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using e_ptit_2.Helpers;

namespace e_ptit_2.Controllers
{
    public class LogicalGamesController : Controller
    {
        // GET: LanguageGames
        private readonly EPtitDataClassesDataContext _db = new EPtitDataClassesDataContext();
        private readonly CommonFunctions _cFunc = new CommonFunctions();
        // GET: Stories
        public ActionResult Index()
        {
            var gameList = new List<GameViewModel>();
            var groups = _db.GameGroups.Where(x => (x.fkEditionId <= _cFunc.GetPublishedEdition())
                && (x.fkGameMenuTypeId == (int) PtitEnums.GameMenuType.Logical)).
                OrderByDescending(x => x.SortOrder ?? 0).ToList();

            foreach (var group in groups)
            {
                if (@group.fkGameMenuTypeId == null) continue;
                if (@group.fkGameTypeId == null) continue;
                var game = new GameViewModel
                {
                    Title = @group.GroupHeader,
                    Description = @group.ListingDescription,
                    Id = @group.pkGameGroupID,
                    LargeIcon = Constants.GameIconsLogic.IconNames[@group.fkGameTypeId.Value],
                    Color = Constants.GameTypeColors.ColorNames[@group.fkGameTypeId.Value],
                    GameTypeId = @group.fkGameTypeId.Value,
                    GameMenuTypeId = @group.fkGameMenuTypeId.Value
                };
                game.GameType = ((PtitEnums.GameType) game.GameTypeId).ToString();
                game.GameMenuType = ((PtitEnums.GameMenuType) game.GameMenuTypeId).ToString();
                game.URL = $"/LogicalGames/Play/{@group.pkGameGroupID}?TID={@group.fkGameTypeId.Value}";

                gameList.Add(game);
            }

            return View(gameList);
        }

        public ActionResult Play(int id, int tid)
        {
            switch (tid)
            {
                case (int)PtitEnums.GameType.Sudoku4:
                    return PlaySudoku4Game(id);
                case (int) PtitEnums.GameType.Sudoku6:
                    return PlaySudoku6Game(id);
                case (int) PtitEnums.GameType.SelectOne:
                case (int)PtitEnums.GameType.SelectOneImages:
                    return PlaySelectOneGame(id, tid);
                case (int) PtitEnums.GameType.WriteAnswer:
                    return PlayWriteAnswerGame(id);
                case (int) PtitEnums.GameType.MagicSquares:
                    return PlayMagicSquaresGame(id);
                case (int)PtitEnums.GameType.Matching:
                    return PlayMatchingGame(id);
            }
            return View();
        }

        private ActionResult PlaySudoku4Game(int id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            dlo.LoadWith<CrosswordGame>(c => c.CrosswordGameWords);
            dlo.LoadWith<CrosswordGame>(c => c.CrosswordGameSpecialCells);
            _db.LoadOptions = dlo;

            GameGroup grp = _db.GameGroups.Where(gg => gg.pkGameGroupID == id).FirstOrDefault();

            if (grp != null)
            {
                GroupToGameInter inter = grp.GroupToGameInters.FirstOrDefault();

                if (inter != null)
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

                        return View(viewName: "~/Views/Games/Sudoku4.cshtml", model: vm);
                    }
                }
            }

            return View();
        }

        private ActionResult PlaySudoku6Game(int id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            dlo.LoadWith<CrosswordGame>(c => c.CrosswordGameWords);
            dlo.LoadWith<CrosswordGame>(c => c.CrosswordGameSpecialCells);
            _db.LoadOptions = dlo;

            GameGroup grp = _db.GameGroups.Where(gg => gg.pkGameGroupID == id).FirstOrDefault();

            if (grp != null)
            {
                GroupToGameInter inter = grp.GroupToGameInters.FirstOrDefault();

                if (inter != null)
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

                        return View(viewName: "~/Views/Games/Sudoku6.cshtml", model: vm);
                    }
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

            if (!(inters?.Count > 0)) return View();

            var vm = new SelectOneViewModel();
            var games = inters.Select(inter => _db.SelectOneGames.FirstOrDefault(x => x.pkSelectOneGameID == inter.fkGameId)).ToList();
            vm.Games = games;

            vm.Title = grp.GroupHeader;
            vm.Description = grp.GroupDescription;
            vm.Style = grp.BackgroundStyle;
            vm.ShowAnswerButton = tid != (int)PtitEnums.GameType.SelectOneImages;

            if (!string.IsNullOrEmpty(grp.BackgroungImage))
                vm.Style += "background-image:url('../../Content/images/games/" + grp.BackgroungImage + "');";
            if (!string.IsNullOrEmpty(grp.BackgroundColor))
                vm.Style += "background-color: " + grp.BackgroundColor + ";";

            return View(viewName: "~/Views/Games/SelectOne.cshtml", model: vm);
        }

        private ActionResult PlayWriteAnswerGame(int Id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            _db.LoadOptions = dlo;

            GameGroup grp = _db.GameGroups.Where(gg => gg.pkGameGroupID == Id).FirstOrDefault();

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
                    vm.ShowKeyboard = false;
                    vm.Games = games;
                    vm.Title = grp.GroupHeader;
                    vm.Description = grp.GroupDescription;
                    vm.ItemsPerRow = grp.ItemsPerRow ?? 1;

                    if (!String.IsNullOrEmpty(grp.BackgroungImage))
                        vm.Style = "background-image:url('../images/games/" + grp.BackgroungImage + "');";
                    if (!String.IsNullOrEmpty(grp.BackgroundColor))
                        vm.Style = "background-color: " + grp.BackgroundColor + ";";

                    return View(viewName: "~/Views/Games/WriteAnswer.cshtml", model: vm);
                }
            }

            return View();
        }

        private ActionResult PlayMagicSquaresGame(int Id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            dlo.LoadWith<CrosswordGame>(c => c.CrosswordGameWords);
            dlo.LoadWith<CrosswordGame>(c => c.CrosswordGameSpecialCells);
            _db.LoadOptions = dlo;

            GameGroup grp = _db.GameGroups.Where(gg => gg.pkGameGroupID == Id).FirstOrDefault();

            if (grp != null)
            {
                GroupToGameInter inter = grp.GroupToGameInters.FirstOrDefault();

                if (inter != null)
                {
                    CrosswordGame game = _db.CrosswordGames.Where(x => x.CrosswordID == inter.fkGameId).FirstOrDefault();

                    if (game != null)
                    {
                        var ww = game.CrosswordGameWords.ToList();
                        var spCells = game.CrosswordGameSpecialCells.ToList();
                        foreach (CrosswordGameWord word in ww)
                        {
                            word.CrosswordGame = null;
                            if (!string.IsNullOrEmpty(word.hint))
                                word.hint.Replace(" ", "&nbsp; ");
                        }
                        foreach (CrosswordGameSpecialCell cell in spCells)
                        {
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

                        return View(viewName: "~/Views/Games/MagicSquare.cshtml", model: vm);
                    }
                }
            }

            return View();
        }

        private ActionResult PlayMatchingGame(int Id)
        {
            var dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            dlo.LoadWith<MatchingGame>(c => c.MatchingGameQuestions);
            dlo.LoadWith<MatchingGame>(c => c.MatchingGameAnswers);
            _db.LoadOptions = dlo;

            var grp = _db.GameGroups.FirstOrDefault(gg => gg.pkGameGroupID == Id);

            if (grp == null) return View();
            var inter = grp.GroupToGameInters.FirstOrDefault();

            if (inter == null) return View();
            var game = _db.MatchingGames.FirstOrDefault(x => x.pkMatchingGameID == inter.fkGameId);

            if (game == null) return View();
            var vm = new MatchingViewModel
            {
                Game = game,
                SoundFile = "successful"
            };

            switch (game.Direction)
            {
                case 1:
                    return View(viewName: "~/Views/Games/Matching.cshtml", model: vm);
                case 2:
                    return View(viewName: "~/Views/Games/MatchingHorizontal.cshtml", model: vm);
            }

            return View();
        }
    }
}