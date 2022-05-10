using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public class MatchingViewModel
    {
        public MatchingGame Game { get; set; }

        public string SoundFile { get; set; } = "bell_sound";
    }
}