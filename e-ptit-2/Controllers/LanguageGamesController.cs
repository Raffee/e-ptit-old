using e_ptit_2.Models;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.String;

namespace e_ptit_2.Controllers
{
    public class LanguageGamesController : Controller
    {
        // GET: LanguageGames
        private readonly EPtitDataClassesDataContext _db = new EPtitDataClassesDataContext();
        private readonly Helpers.CommonFunctions _cFunc = new Helpers.CommonFunctions();
        // GET: Stories
        public ActionResult Index()
        {
            var gameList = new List<GameViewModel>();
            var groups = _db.GameGroups.Where(x => (x.fkEditionId <= _cFunc.GetPublishedEdition()) 
                && (x.fkGameMenuTypeId == (int) Helpers.PtitEnums.GameMenuType.Language)).
                OrderByDescending(x => x.SortOrder).ToList();

            foreach (var group in groups)
            {
                var gameTypeId = @group.fkGameTypeId ?? 1;
                var game = new GameViewModel
                {
                    Title = @group.GroupHeader,
                    Description = @group.ListingDescription,
                    Id = @group.pkGameGroupID,
                    LargeIcon = Helpers.Constants.GameIconsLarge.IconNames[gameTypeId],
                    Color = Helpers.Constants.GameTypeColors.ColorNames[gameTypeId],
                    GameTypeId = gameTypeId,
                    GameMenuTypeId = @group.fkGameMenuTypeId ?? 1
                };
                game.GameType = ((Helpers.PtitEnums.GameType) game.GameTypeId).ToString();
                game.GameMenuType = ((Helpers.PtitEnums.GameMenuType) game.GameMenuTypeId).ToString();
                game.URL = $"/LanguageGames/Play/{group.pkGameGroupID}?TID={gameTypeId}";

                gameList.Add(game);
            }

            return View(gameList);
        }

        public ActionResult Play(int id, int tid)
        {
            switch (tid)
            {
                case (int) Helpers.PtitEnums.GameType.Crossword:
                    return PlayCrosswordGame(id);
                case (int) Helpers.PtitEnums.GameType.WordSearch:
                    return PlayHiddenWordGame(id);
                case (int) Helpers.PtitEnums.GameType.Matching:
                    return PlayMatchingGame(id);
                case (int) Helpers.PtitEnums.GameType.WriteAnswer:
                    return PlayWriteAnswerGame(id);
                case (int) Helpers.PtitEnums.GameType.WordLetterGame:
                    return PlayWordLetterGame(id);
                case (int)Helpers.PtitEnums.GameType.ConnectDots:
                    return PlayConnectDotsGame(id);
            }
            return View();
        }

        private ActionResult PlayConnectDotsGame(int id)
        {
            var dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            dlo.LoadWith<MatchingGame>(c => c.MatchingGameQuestions);
            dlo.LoadWith<MatchingGame>(c => c.MatchingGameAnswers);
            _db.LoadOptions = dlo;

            var grp = _db.GameGroups.FirstOrDefault(gg => gg.pkGameGroupID == id);

            var inter = grp?.GroupToGameInters.FirstOrDefault();

            if (inter == null) return View("~/Views/Games/ConnectDots.cshtml");
            var game = _db.MatchingGames.FirstOrDefault(x => x.pkMatchingGameID == inter.fkGameId);

            if (game == null) return View();
            var vm = new MatchingViewModel
            {
                Game = game,
                SoundFile = "successful"
            };
            return View("~/Views/Games/ConnectDots.cshtml", vm);
        }

        private ActionResult PlayCrosswordGame(int id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            dlo.LoadWith<CrosswordGame>(c => c.CrosswordGameWords);
            dlo.LoadWith<CrosswordGame>(c => c.CrosswordGameSpecialCells);
            _db.LoadOptions = dlo;

            GameGroup grp = _db.GameGroups.Where(gg => gg.pkGameGroupID == id).FirstOrDefault();

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
                            if (!IsNullOrEmpty(word.hint))
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
                        vm.SoundFile = "successful";

                        return View(viewName: "~/Views/Games/Crossword.cshtml", model: vm);
                    }
                }
            }

            return View();
        }

        private ActionResult PlayHiddenWordGame(int id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            dlo.LoadWith<HiddenWordGame>(c => c.HiddenWordGameWords);
            _db.LoadOptions = dlo;

            GameGroup grp = _db.GameGroups.Where(gg => gg.pkGameGroupID == id).FirstOrDefault();

            if (grp != null)
            {
                GroupToGameInter inter = grp.GroupToGameInters.FirstOrDefault();

                if (inter != null)
                {
                    HiddenWordGame game = _db.HiddenWordGames.Where(x => x.pkHiddenWordGameID == inter.fkGameId).FirstOrDefault();

                    if (game != null)
                    {
                        string wordList = "";
                        var ww = game.HiddenWordGameWords.ToList();
                        foreach (HiddenWordGameWord word in ww)
                        {
                            word.HiddenWordGame = null;
                            wordList += word.word + ",";
                        }
                        if (wordList.Length > 0)
                            wordList.Remove(wordList.Length - 1);

                        HiddenWordViewModel vm = new HiddenWordViewModel();
                        vm.HiddenWordGame = game;
                        vm.Rows = game.Rows ?? 0;
                        vm.Cols = game.Columns ?? 0;
                        vm.correctAnswer = game.Answer;
                        vm.words = wordList;
                        vm.wordObjsList = Newtonsoft.Json.JsonConvert.SerializeObject(ww);
                        vm.targetLetters = Newtonsoft.Json.JsonConvert.SerializeObject(game.Answer.ToCharArray());
                        vm.SoundFile = "successful";

                        return View(viewName: "~/Views/Games/HiddenWord.cshtml", model: vm);
                    }
                }
            }

            return View();
        }

        private ActionResult PlayMatchingGame(int id)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            dlo.LoadWith<MatchingGame>(c => c.MatchingGameQuestions);
            dlo.LoadWith<MatchingGame>(c => c.MatchingGameAnswers);
            _db.LoadOptions = dlo;

            GameGroup grp = _db.GameGroups.Where(gg => gg.pkGameGroupID == id).FirstOrDefault();

            if (grp != null)
            {
                GroupToGameInter inter = grp.GroupToGameInters.FirstOrDefault();

                if (inter != null)
                {
                    MatchingGame game = _db.MatchingGames.Where(x => x.pkMatchingGameID == inter.fkGameId).FirstOrDefault();

                    if (game != null)
                    {
                        MatchingViewModel vm = new MatchingViewModel();
                        vm.Game = game;
                        vm.SoundFile = "successful";

                        if(game.Direction == 1)
                            return View(viewName: "~/Views/Games/Matching.cshtml", model: vm);
                        if (game.Direction == 2)
                            return View(viewName: "~/Views/Games/MatchingHorizontal.cshtml", model: vm);
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

            var grp = _db.GameGroups.FirstOrDefault(gg => gg.pkGameGroupID == id);

            if (grp != null)
            {
                var inters = grp.GroupToGameInters.ToList();

                if (inters.Count > 0)
                {
                    var vm = new WriteAnswerViewModel();
                    var games = inters.Select(inter => _db.WriteAnswerGames.FirstOrDefault(x => x.pkWriteAnswerGameId == inter.fkGameId)).ToList();
                    vm.ShowKeyboard = true;
                    vm.Games = games;
                    vm.SoundFile = "successful";

                    vm.Title = grp.GroupHeader;
                    vm.Description = grp.GroupDescription;
                    vm.ItemsPerRow = grp.ItemsPerRow ?? 1;
                    vm.Style = grp.BackgroundStyle;

                    if (!IsNullOrEmpty(grp.BackgroungImage))
                        vm.Style += " background-image:url('../../Content/images/games/" + grp.BackgroungImage + "'); ";
                    if (!IsNullOrEmpty(grp.BackgroundColor))
                        vm.Style += " background-color: " + grp.BackgroundColor + "; ";

                    return View(viewName: "~/Views/Games/WriteAnswer.cshtml", model: vm);
                }
            }

            return View();
        }

        private ActionResult PlayWordLetterGame(int id)
        {
            var dlo = new DataLoadOptions();
            dlo.LoadWith<GameGroup>(g => g.GroupToGameInters);
            dlo.LoadWith<LetterOfWordGame>(g => g.LetterGameWords);
            _db.LoadOptions = dlo;

            var grp = _db.GameGroups.FirstOrDefault(gg => gg.pkGameGroupID == id);

            if (grp == null) return View();
            var inter = grp.GroupToGameInters.FirstOrDefault();

            if (inter == null) return View();
            var vm = new WordLetterViewModel();
            var game = _db.LetterOfWordGames.FirstOrDefault(x => x.pkLetterOfWord == inter.fkGameId);
                        
            vm.ShowKeyboard = true;
            vm.Game = game;
            vm.SoundFile = "successful";
            vm.ItemsPerRow = grp.ItemsPerRow ?? 4;

            if (!IsNullOrEmpty(grp.BackgroundStyle))
                vm.Style += grp.BackgroundStyle;
            if (!IsNullOrEmpty(grp.BackgroungImage))
                vm.Style += "background-image:url('../../Content/images/games/" + grp.BackgroungImage + "');";
            if (!IsNullOrEmpty(grp.BackgroundColor))
                vm.Style += "background-color: " + grp.BackgroundColor + ";";

            return View(viewName: "~/Views/Games/WordLetters.cshtml", model: vm);
        }
    }
}