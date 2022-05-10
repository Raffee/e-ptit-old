using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public class MazeViewModel
    {
        public Maze Maze { get; set; }

        public string SoundFile { get; set; } = "bell_sound";

        public int startX { get; set; }

        public int endX { get; set; }

        public int startY { get; set; }

        public int endY { get; set; }

        public int mazeWidth { get; set; }

        public int mazeHeight { get; set; }

        public int lineRed { get; set; }

        public int lineGreen { get; set; }

        public int lineBlue { get; set; }

        public int width { get; set; }

        public int height { get; set; }

        public string imagePath { get; set; }
    }
}