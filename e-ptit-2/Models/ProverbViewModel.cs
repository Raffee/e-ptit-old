using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public class ProverbViewModel : Proverb
    {
        public ProverbViewModel(Proverb proverb)
        {
            this.Proverb = proverb;
            this.ContentList = proverb.ProverbContents.ToList();
        }

        public Proverb Proverb { get; set; }

        public List<ProverbContent> ContentList { get; set; }

        public List<Proverb> RelatedProverbs { get; set; }
    }
}