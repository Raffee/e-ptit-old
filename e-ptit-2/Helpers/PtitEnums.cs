using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_ptit_2.Helpers
{
    public class PtitEnums
    {
        public enum GameType
        {
            Matching = 1,
            SelectOne = 2,
            WriteAnswer = 3,
            FindDifference = 4,
            Crossword = 5,
            WordSearch = 6,
            Maze = 7,
            Sudoku4 = 8,
            Sudoku6 = 9,
            WordLetterGame = 10,
            Pyramid = 11,
            SelectOneImages = 12,
            MagicSquares = 13,
            ConnectDots = 14,
        }

        public enum GameMenuType
        {
            Language = 1,
            Logical = 2,
            Math = 3,
            Other = 4
        }
    }
}