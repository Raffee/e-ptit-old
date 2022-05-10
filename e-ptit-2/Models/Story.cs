using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public partial class Story : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public List<StoryContent> GetStoryContents()
        {
            return this.StoryContents.ToList();
        }
    }
}