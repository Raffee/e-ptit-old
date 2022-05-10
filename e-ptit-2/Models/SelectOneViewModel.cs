using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public class SelectOneViewModel
    {
        public string SoundFile { get; set; } = "bell_sound";

        public List<SelectOneGame> Games { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Style { get; set; }

        public bool ShowAnswerButton { get; set; } = true;

        public string BackgroundImage { get; set; }
    }
}