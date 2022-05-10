using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public partial class MatchingGame : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public List<MatchingGameQuestion> GetQuestions()
        {
            return this.MatchingGameQuestions.ToList();
        }
        public List<MatchingGameAnswer> GetAnswers()
        {
            return this.MatchingGameAnswers.ToList();
        }
    }
}