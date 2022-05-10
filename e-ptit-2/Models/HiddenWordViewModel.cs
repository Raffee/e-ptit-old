using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public class HiddenWordViewModel
    {
        public HiddenWordGame HiddenWordGame { get; set; }

        public string SoundFile { get; set; } = "correct";

        public int Rows { get; set; }

        public int Cols { get; set; }

        public string words { get; set; }

        public string wordObjsList { get; set; }

        public string targetLetters { get; set; }

        public string correctAnswer { get; set; }
    }
}