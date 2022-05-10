using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public class GameViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string LargeIcon { get; set; }

        public string Color { get; set; }

        public string URL { get; set; }

        public int GameTypeId { get; set; }

        public int GameMenuTypeId { get; set; }

        public string GameMenuType { get; set; }

        public string GameType { get; set; }
    }
}