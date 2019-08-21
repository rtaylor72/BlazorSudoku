using System.Collections.Generic;

namespace BlazorSudoku.Solvers {
    public class RingPattern_FirstOption_Solver : BaseSolver {
        public override string Name {
            get {
                return "Ring Pattern, First Option";
            }
        }

        public override string Description {
            get {
                return "This is an algorithm that works the puzzle by scanning the top block cells, then works on the left block cells, then rings bottom to top, left to right until it reaches the bottom right corner.  Possible options are chosen in sequence.";
            }
        }

        public override string Author {
            get {
                return "Matthew Taylor";
            }
        }

        public override void FindNextCell(byte[,] board, List<byte>[,] possibleAnswers, out byte NextRow, out byte NextColumn) {
            FindNextCellByRingPattern(board, out NextRow, out NextColumn);
        }

        public override byte FindNextOption(List<byte> possibleAnswers) {
            return FindNextOptionByFirst(possibleAnswers);
        }
    }
}