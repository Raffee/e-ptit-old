using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public class WordLetterViewModel
    {
        public string SoundFile { get; set; } = "bell_sound";

        public bool ShowKeyboard { get; set; } = true;

        public LetterOfWordGame Game { get; set; }

        public string Style { get; set; }

        public int ItemsPerRow { get; set; }

        public List<LetterGameWord> GameWords => Game.LetterGameWords.ToList();
    }
}