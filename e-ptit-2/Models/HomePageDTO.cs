using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public class HomePageDTO
    {
        public List<Story> Stories { get; set; }
        public List<GameViewModel> LanguageGames { get; set; }
        public List<GameViewModel> OtherGames { get; set; }
        public List<Proverb> Proverbs { get; set; }
    }
}