using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public partial class SelectOneGame
    {
        public List<SelectOneGameAnswer> Answers => SelectOneGameAnswers.ToList();

        public string Answer
        {
            get
            {
                var answer = SelectOneGameAnswers.FirstOrDefault(x => x.isCorrectAnswer);
                return answer != null ? answer.AnswerText : "";
            }
        }
    }
}