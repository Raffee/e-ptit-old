using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public class PyramidViewModel
    {
        public PyramidGame Pyramid { get; set; }

        public string SoundFile { get; set; } = "correct";

        public int Rows { get; set; }

        public int Cols { get; set; }

        public string words { get; set; }

        public string otherCells { get; set; }

        public double padding { get; set; }

        public List<PyramidEntry> PyramidEntries { get; set; }

        public List<PyramidEntry> GetRowEntries(int row)
        {
            var items = this.PyramidEntries.ToList().Where(item => item.Row == row).OrderBy(item => item.Col).ToList();
            return items;
        }
    }
}