using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public class StoryViewModel : Story
    {
        public StoryViewModel(Story story)
        {
            this.Story = story;
            this.ContentList = story.StoryContents.ToList();
        }

        public Story Story { get; set; }

        public List<StoryContent> ContentList { get; set; }

        public List<Story> RelatedStories { get; set; }
    }
}