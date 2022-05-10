using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public class CrosswordViewModel
    {
        public CrosswordGame Crossword { get; set; }

        public string SoundFile { get; set; } = "correct";

        public bool ShowKeyboard { get; set; } = true;

        public int Rows { get; set; }

        public int Cols { get; set; }

        public string words { get; set; }

        public string otherCells { get; set; }

        public bool isSeparateHints { get; set; } = false;

        public bool HintsIn2Columns { get; set; }
    }
}