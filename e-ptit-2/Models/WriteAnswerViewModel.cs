using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public class WriteAnswerViewModel
    {
        public string SoundFile { get; set; } = "correct";

        public bool ShowKeyboard { get; set; } = true;

        public List<WriteAnswerGame> Games { get; set; }

        public string Style { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int ItemsPerRow { get; set; }
    }
}