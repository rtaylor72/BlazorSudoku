using BlazorSudoku.Solvers;
using System;
using System.Collections.ObjectModel;

namespace BlazorSudoku.Core
{
    public static class Utility
    {
        public static Board CreateNewBoard(byte boardsize)
        {
            Board result = new Board(boardsize);

            for (byte row = 0; row < boardsize; row++)
            {
                for (byte column = 0; column < boardsize; column++)
                {
                    Cell cell = result[row, column];
                    cell.CellDecoration = CellDecoration.Normal;
                    cell.Value = 0;
                }
            }

            return result;
        }

        public static Board CreateNewBoard(byte boardsize, int givenCells, bool isUnique)
        {
            if (givenCells > (boardsize * boardsize) - 1)
            {
                givenCells = (boardsize * boardsize) - 1;
            }

            //Create a valid board
            Board result = CreateNewBoard(boardsize);
            ISolver solver = new ScanPattern_RandomOption_Solver();
            ObservableCollection<SolverSolution> sl = new ObservableCollection<SolverSolution>
            {
                new SolverSolution() { BackTracks = 0, IsSolution = false, Board = result.ToArray(), SolutionDepth = 0, SolverName = "Puzzle", StepsToSolve = 0, TimeToSolve = new TimeSpan() }
            };
            byte[,] board = CreateNewSolverStarterBoard(boardsize);
            solver.Solve(ref sl, board, 1);

            //Randomly remove cells
            Random rnd = new Random();
            if (sl.Count > 0)
            {
                for (int i = 0; i < sl.Count; i++)
                {
                    if (sl[i].IsSolution)
                    {
                        board = BaseSolver.Copy(sl[i].Board);
                        for (byte clearCellCount = 0; clearCellCount < (boardsize * boardsize) - givenCells; clearCellCount++)
                        {
                            while (1 == 1)
                            {
                                byte row = (byte)rnd.Next(boardsize);
                                byte column = (byte)rnd.Next(boardsize);

                                if (board[row, column] == 0) continue;
                                board[row, column] = 0;
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            if (isUnique)
            {
                //Set cells to make unique
                solver.Solve(ref sl, board, 2);
                while (sl.Count > 1)
                {
                    byte[,] solutionBoard1 = sl[0].Board;
                    byte[,] solutionBoard2 = sl[1].Board;
                    bool tryagain = false;

                    for (byte row = 0; row < boardsize; row++)
                    {
                        for (byte column = 0; column < boardsize; column++)
                        {
                            if (solutionBoard1[row, column] != solutionBoard2[row, column])
                            {
                                board[row, column] = solutionBoard1[row, column];
                                tryagain = true;
                                System.Diagnostics.Debug.WriteLine("Resolve for: " + row + " " + column);
                                break;
                            }
                        }
                        if (tryagain) break;
                    }
                    sl.Clear();
                    solver.Solve(ref sl, board, 2);
                }

                //Get Given Count
                int iGivenCount = 0;
                for (byte row = 0; row < boardsize; row++)
                {
                    for (byte column = 0; column < boardsize; column++)
                    {
                        if (board[row, column] > 0)
                        {
                            iGivenCount++;
                        }
                    }
                }

                //See if any cells can be cleared and still be unique, but keep given count
                for (byte row = 0; row < boardsize; row++)
                {
                    for (byte column = 0; column < boardsize; column++)
                    {
                        if (iGivenCount <= givenCells) break;
                        if (board[row, column] > 0)
                        {
                            byte[,] solutionBoard1 = BaseSolver.Copy(board);
                            solutionBoard1[row, column] = 0;
                            sl.Clear();
                            solver.Solve(ref sl, solutionBoard1, 2);
                            if (sl.Count == 1)
                            {
                                board = solutionBoard1;
                                iGivenCount--;
                            }
                        }
                    }
                    if (iGivenCount <= givenCells) break;
                }
            }

            //Set remaining cells as readonly
            result.FromArray(board);
            SetCurrentCellsAsReadOnly(result);

            return result;
        }

        public static Collection<SolverSolution> SolveBoard(Board board)
        {
            ISolver solver = new ScanPattern_RandomOption_Solver();
            ObservableCollection<SolverSolution> sl = new ObservableCollection<SolverSolution>();
            if (board != null) solver.Solve(ref sl, board.ToArray(), 50);
            return sl;
        }

        private static byte[,] CreateNewSolverStarterBoard(byte boardsize)
        {
            Random rnd = new Random();

            byte[,] board = new byte[boardsize, boardsize];

            if (1 == 1)
            {
                bool[] used = new bool[boardsize];
                for (int column = 0; column < boardsize; column++)
                {
                    byte rndNum;
                    do
                    {
                        rndNum = (byte)(rnd.Next(boardsize) + 1);
                    } while (used[rndNum - 1]);

                    board[0, column] = rndNum;
                    used[rndNum - 1] = true;
                }
            }

            return board;
        }

        public static void SetCurrentCellsAsReadOnly(Board board)
        {
            if (board != null)
            {
                for (byte row = 0; row < board.Size; row++)
                {
                    for (byte column = 0; column < board.Size; column++)
                    {
                        Cell cell = board[row, column];
                        if (cell.Value > 0) cell.CellDecoration = CellDecoration.ReadOnly;
                    }
                }
            }
        }
    }
}
