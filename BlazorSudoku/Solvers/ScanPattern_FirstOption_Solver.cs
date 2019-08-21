using System.Collections.Generic;

namespace BlazorSudoku.Solvers {
    public class ScanPattern_FirstOption_Solver : BaseSolver {
        public override string Name {
            get {
                return "Scan Pattern, First Option";
            }
        }

        public override string Description {
            get {
                return "This is an algorithm that works the puzzle left to right, top to bottom.  Possible options are chosen in sequence.";
            }
        }

        public override string Author {
            get {
                return "Matthew Taylor";
            }
        }

        public override void FindNextCell(byte[,] board, List<byte>[,] possibleAnswers, out byte NextRow, out byte NextColumn) {
            FindNextCellByScanPattern(board, out NextRow, out NextColumn);
        }

        public override byte FindNextOption(List<byte> possibleAnswers) {
            return FindNextOptionByFirst(possibleAnswers);
        }
    }
}