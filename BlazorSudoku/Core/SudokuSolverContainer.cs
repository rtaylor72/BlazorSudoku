using BlazorSudoku.Solvers;
using System;
using System.Collections.ObjectModel;

namespace BlazorSudoku.Core
{
    public class SudokuSolverContainer : MarshalByRefObject
    {
        ISolver solver;
        public void Init(Type t)
        {
            solver = Activator.CreateInstance(t) as ISolver;
        }
        public string Name
        {
            get { return solver.Name; }
        }
        public string Description
        {
            get { return solver.Description; }
        }
        public string Author
        {
            get { return solver.Author; }
        }
        public void Solve(ref ObservableCollection<SolverSolution> solutionList, byte[,] board, int maxSolutionsReturned)
        {
            solver.Solve(ref solutionList, board, maxSolutionsReturned);
        }
    }
}