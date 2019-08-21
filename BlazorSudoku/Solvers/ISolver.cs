using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BlazorSudoku.Solvers {
    public interface ISolver {
        string Name {
            get;
        }
        string Description {
            get;
        }
        string Author {
            get;
        }
        void Solve(ref ObservableCollection<SolverSolution> solutionList, byte[,] board, int maxSolutionsReturned);

        void FindNextCell(byte[,] board, List<byte>[,] possibleAnswers, out byte NextRow, out byte NextColumn);

        byte FindNextOption(List<byte> possibleAnswers);
    }
}