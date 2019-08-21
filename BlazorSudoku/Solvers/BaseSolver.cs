using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BlazorSudoku.Solvers
{
    public abstract class BaseSolver : ISolver
    {
        private int _steps = 0;

        #region ISolver Members
        public virtual string Name
        {
            get { throw new NotImplementedException(); }
        }

        public virtual string Description
        {
            get { throw new NotImplementedException(); }
        }

        public virtual string Author
        {
            get { throw new NotImplementedException(); }
        }

        public virtual void Solve(ref ObservableCollection<SolverSolution> solutionList, byte[,] board, int maxSolutionsReturned)
        {
            if (solutionList == null)
            {
                throw new ArgumentNullException(nameof(solutionList));
            }

            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            byte size = (byte)board.GetLength(0);
            System.Diagnostics.Debug.WriteLine("Begin:" + Name);
            DateTime dtStart = DateTime.Now;

            //Prefill in possible answers to each cell
            List<byte> CurrentPossibleAnswers;
            List<byte>[,] PossibleAnswers = new List<byte>[size, size];
            for (byte row = 0; row < size; row++)
            {
                for (byte column = 0; column < size; column++)
                {
                    CurrentPossibleAnswers = AvailableOptionsForCell(board, row, column);
                    if (board[row, column] == 0 && CurrentPossibleAnswers.Count == 0)
                    {
                        //Not possible to solve this board
                        solutionList.Add(new SolverSolution()
                        {
                            SolverName = Name,
                            Board = board,
                            TimeToSolve = DateTime.Now - dtStart,
                            SolverNotes = $"No possible answers for column {column}, row {row}"
                        });
                    }
                    PossibleAnswers[row, column] = CurrentPossibleAnswers;
                }
            }

            //Attempt solve
            _steps = 0;
            int Backtracks = 0;
            RecursiveSolve(board, PossibleAnswers, solutionList, dtStart, 1, ref Backtracks, maxSolutionsReturned);

            //Check result
            if (SolutionCount(solutionList, Name) == 0) System.Diagnostics.Debug.WriteLine("Fail:" + Name);
            System.Diagnostics.Debug.WriteLine("End:" + Name);
        }

        public virtual void FindNextCell(byte[,] board, List<byte>[,] possibleAnswers, out byte NextRow, out byte NextColumn)
        {
            throw new NotImplementedException();
        }

        public virtual byte FindNextOption(List<byte> possibleAnswers)
        {
            throw new NotImplementedException();
        }
        #endregion

        public static bool Validate(byte[,] board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            byte size = (byte)board.GetLength(0);
            byte blocksize = (byte)Math.Sqrt(size);
            for (byte row = 0; row < size; row++)
            {
                bool[] used = new bool[size];
                for (byte column = 0; column < size; column++)
                {
                    if (board[row, column] == 0)
                    {
                        return false;
                    }
                    else if (used[board[row, column] - 1])
                    {
                        return false;
                    }
                    else
                    {
                        used[board[row, column] - 1] = true;
                    }
                }
            }
            for (byte column = 0; column < size; column++)
            {
                bool[] used = new bool[size];
                for (byte row = 0; row < size; row++)
                {
                    if (used[board[row, column] - 1])
                    {
                        return false;
                    }
                    else
                    {
                        used[board[row, column] - 1] = true;
                    }
                }
            }
            for (byte row = 0; row < size - blocksize; row += blocksize)
            {
                for (byte column = 0; column < size - blocksize; column += blocksize)
                {
                    bool[] used = new bool[size];
                    for (byte blockrow = 0; blockrow < blocksize; blockrow++)
                    {
                        for (byte blockcolumn = 0; blockcolumn < blocksize; blockcolumn++)
                        {
                            if (used[board[blockrow + row, blockcolumn + column] - 1])
                            {
                                return false;
                            }
                            else
                            {
                                used[board[blockrow + row, blockcolumn + column] - 1] = true;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public static byte[,] Copy(byte[,] board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            byte[,] n = board.Clone() as byte[,];
            return n;
        }

        public static List<byte>[,] Copy(List<byte>[,] board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            List<byte>[,] n = board.Clone() as List<byte>[,];
            for (byte row = 0; row < n.GetLength(0); row++)
            {
                for (byte column = 0; column < n.GetLength(1); column++)
                {
                    n[row, column] = new List<byte>(n[row, column]);
                }
            }
            return n;
        }

        public static List<byte> AvailableOptionsForCell(byte[,] board, byte row, byte column)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            byte size = (byte)board.GetLength(0);
            byte blocksize = (byte)Math.Sqrt(size);

            List<byte> OptionList = new List<byte>(size);
            if (board[row, column] == 0)
            {
                for (byte i = 1; i <= size; i++)
                {
                    OptionList.Add(i);
                }

                byte CurrentValue;
                //row check
                for (byte i = 0; i < size; i++)
                {
                    CurrentValue = board[row, i];
                    if (CurrentValue > 0)
                    {
                        if (OptionList.Contains(CurrentValue))
                        {
                            OptionList.Remove(CurrentValue);
                        }
                    }
                }

                //column check
                for (byte i = 0; i < size; i++)
                {
                    CurrentValue = board[i, column];
                    if (CurrentValue > 0)
                    {
                        if (OptionList.Contains(CurrentValue))
                        {
                            OptionList.Remove(CurrentValue);
                        }
                    }
                }

                //block check
                byte StartRow = (byte)(row - (row % blocksize));
                byte EndRow = (byte)(StartRow + blocksize - 1);
                byte StartColumn = (byte)(column - (column % blocksize));
                byte EndColumn = (byte)(StartColumn + blocksize - 1);

                for (byte i = StartRow; i <= EndRow; i++)
                {
                    for (byte j = StartColumn; j <= EndColumn; j++)
                    {
                        CurrentValue = board[i, j];
                        if (CurrentValue > 0)
                        {
                            if (OptionList.Contains(CurrentValue))
                            {
                                OptionList.Remove(CurrentValue);
                            }
                        }
                    }
                }
            }

            return OptionList;
        }

        public static bool RemoveOptions(ref List<byte>[,] possibleAnswers, byte removeOption, byte row, byte column)
        {
            if (possibleAnswers == null)
            {
                throw new ArgumentNullException(nameof(possibleAnswers));
            }

            byte size = (byte)possibleAnswers.GetLength(0);
            byte blocksize = (byte)Math.Sqrt(size);
            bool OptionWasRemoved = false;

            //row check
            for (byte i = 0; i < size; i++)
            {
                if (possibleAnswers[row, i].Contains(removeOption))
                {
                    possibleAnswers[row, i].Remove(removeOption);
                    OptionWasRemoved = true;
                }
            }

            //column check
            for (byte i = 0; i < size; i++)
            {
                if (possibleAnswers[i, column].Contains(removeOption))
                {
                    possibleAnswers[i, column].Remove(removeOption);
                    OptionWasRemoved = true;
                }
            }

            //block check
            byte StartRow = (byte)(row - (row % blocksize));
            byte EndRow = (byte)(StartRow + blocksize - 1);
            byte StartColumn = (byte)(column - (column % blocksize));
            byte EndColumn = (byte)(StartColumn + blocksize - 1);

            for (byte i = StartColumn; i <= EndColumn; i++)
            {
                for (byte j = StartRow; j <= EndRow; j++)
                {
                    if (possibleAnswers[j, i].Contains(removeOption))
                    {
                        possibleAnswers[j, i].Remove(removeOption);
                        OptionWasRemoved = true;
                    }
                }
            }

            return OptionWasRemoved;
        }

        public static bool ChooseSingleOptions(ref byte[,] board, ref List<byte>[,] possibleAnswers)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (possibleAnswers == null)
            {
                throw new ArgumentNullException(nameof(possibleAnswers));
            }

            byte size = (byte)board.GetLength(0);
            bool SingleOptionFound = false;

            //DebugOutput(board, possibleAnswers, "ChooseSingleOptions Start");

            for (byte row = 0; row < size; row++)
            {
                for (byte column = 0; column < size; column++)
                {
                    if (possibleAnswers[row, column].Count == 1)
                    {
                        board[row, column] = possibleAnswers[row, column][0];
                        possibleAnswers[row, column] = new List<byte>();
                        SingleOptionFound = (RemoveOptions(ref possibleAnswers, board[row, column], row, column) || SingleOptionFound);
                    }
                }
            }

            //if (SingleOptionFound) DebugOutput(board, possibleAnswers, "ChooseSingleOptions End");

            return SingleOptionFound;
        }

        public static int SolutionCount(ObservableCollection<SolverSolution> solutionList)
        {
            return SolutionCount(solutionList, null);
        }

        public static int SolutionCount(ObservableCollection<SolverSolution> solutionList, string SolverName)
        {
            if (solutionList == null)
            {
                throw new ArgumentNullException(nameof(solutionList));
            }

            int iSolutionTotal = 0;
            for (int i = solutionList.Count - 1; i >= 0; i--)
            {
                if (solutionList[i].IsSolution && (SolverName == null || solutionList[i].SolverName == SolverName))
                {
                    iSolutionTotal++;
                }
            }
            return iSolutionTotal;
        }

        public void RecursiveSolve(byte[,] board, List<byte>[,] possibleAnswers, ObservableCollection<SolverSolution> solutionList, DateTime startTime, int currentDepth, ref int backtracks, int maxSolutionsReturned)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (possibleAnswers == null)
            {
                throw new ArgumentNullException(nameof(possibleAnswers));
            }

            if (solutionList == null)
            {
                throw new ArgumentNullException(nameof(solutionList));
            }

            _steps++;
            byte size = (byte)board.GetLength(0);
            byte blocksize = (byte)Math.Sqrt(size);

            //DebugOutput(board, possibleAnswers, "RecursiveSolve Start");

            //Check if solvable board
            for (byte row = 0; row < size - blocksize; row += blocksize)
            {
                for (byte column = 0; column < size - blocksize; column += blocksize)
                {
                    bool[] used = new bool[size];
                    byte openCells = 0;
                    byte availableOptions = 0;
                    for (byte blockrow = 0; blockrow < blocksize; blockrow++)
                    {
                        for (byte blockcolumn = 0; blockcolumn < blocksize; blockcolumn++)
                        {
                            if (board[blockrow + row, blockcolumn + column] == 0)
                            {
                                if (possibleAnswers[blockrow + row, blockcolumn + column].Count == 0)
                                {
                                    //empty cell with no possible answers
                                    return;
                                }
                                else
                                {
                                    openCells++;
                                }
                            }
                            else
                            {
                                used[board[blockrow + row, blockcolumn + column] - 1] = true;
                            }
                        }
                    }

                    for (int i = 0; i < size; i++)
                    {
                        if (!used[i]) availableOptions++;
                    }

                    if (openCells > availableOptions)
                    {
                        //Not enough options to fill empty cells
                        return;
                    }
                }
            }

            //Find next open spot
            FindNextCell(board, possibleAnswers, out global::System.Byte cellRow, out global::System.Byte cellColumn);

            if (cellRow == 254 && cellColumn == 254)
            {
                //All cells filled
                //DebugOutput(ref board, ref possibleAnswers);
                if (Validate(board))
                {
                    int bt = backtracks;
                    solutionList.Add(new SolverSolution()
                    {
                        SolverName = Name,
                        IsSolution = true,
                        Board = board,
                        BackTracks = bt,
                        SolutionDepth = currentDepth,
                        StepsToSolve = _steps,
                        TimeToSolve = DateTime.Now - startTime
                    });
                }
                return;
            }

            bool IsTrackBack = false;
            while (possibleAnswers[cellRow, cellColumn].Count > 0)
            {
                byte option = FindNextOption(possibleAnswers[cellRow, cellColumn]);
                possibleAnswers[cellRow, cellColumn].Remove(option);

                //Create copies of board and possible answers for recursion
                byte[,] board2 = Copy(board);
                List<byte>[,] possibleAnswers2 = Copy(possibleAnswers);

                //Set current cell of copied board to chosen answer
                board2[cellRow, cellColumn] = option;
                possibleAnswers2[cellRow, cellColumn] = new List<byte>();

                //Remove option from cell's row, column, and block.  Choose any options that are only one left.
                RemoveOptions(ref possibleAnswers2, option, cellRow, cellColumn);
                while (ChooseSingleOptions(ref board2, ref possibleAnswers2))
                {
                }

                //Recurse
                if (IsTrackBack) backtracks++;
                RecursiveSolve(board2, possibleAnswers2, solutionList, startTime, currentDepth + 1, ref backtracks, maxSolutionsReturned);
                if (SolutionCount(solutionList, Name) >= maxSolutionsReturned) return;
                IsTrackBack = true;
            }
        }

        public static void DebugOutput(byte[,] board, List<byte>[,] possibleAnswers, string title)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (possibleAnswers == null)
            {
                throw new ArgumentNullException(nameof(possibleAnswers));
            }

            byte size = (byte)board.GetLength(0);

            System.Diagnostics.Debug.WriteLine("------------------------------------------");
            System.Diagnostics.Debug.WriteLine(title);
            System.Diagnostics.Debug.WriteLine("------------------------------------------");
            for (byte row = 0; row < size; row++)
            {
                for (byte column = 0; column < size; column++)
                {
                    System.Diagnostics.Debug.Write(board[row, column].ToString() + ",");
                }
                System.Diagnostics.Debug.WriteLine("");
            }

            System.Diagnostics.Debug.WriteLine("------------------------------------------");
            for (byte row = 0; row < size; row++)
            {
                for (byte column = 0; column < size; column++)
                {
                    for (byte options = 1; options <= size; options++)
                    {
                        if (possibleAnswers[row, column] != null && possibleAnswers[row, column].Contains(options))
                        {
                            System.Diagnostics.Debug.Write(options.ToString() + ",");
                        }
                        else
                        {
                            System.Diagnostics.Debug.Write(" ,");
                        }
                    }
                    System.Diagnostics.Debug.Write("|");
                }
                System.Diagnostics.Debug.WriteLine("");
            }
        }

        #region FindNextCell Functions
        public static void FindNextCellByScanPattern(byte[,] board, out byte NextRow, out byte NextColumn)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            byte size = (byte)board.GetLength(0);
            NextRow = 254;
            NextColumn = 254;
            for (byte row = 0; row < size; row++)
            {
                for (byte column = 0; column < size; column++)
                {
                    if (board[row, column] == 0)
                    {
                        //Work on this cell
                        NextRow = row;
                        NextColumn = column;
                        return;
                    }
                }
            }
        }

        public static void FindNextCellByRingPattern(byte[,] board, out byte NextRow, out byte NextColumn)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            byte size = (byte)board.GetLength(0);
            byte blocksize = (byte)Math.Sqrt(size);

            NextRow = 254;
            NextColumn = 254;

            //Fill top block row
            for (byte row = 0; row < blocksize; row++)
            {
                for (byte column = 0; column < size; column++)
                {
                    if (board[row, column] == 0)
                    {
                        //Work on this cell
                        NextRow = row;
                        NextColumn = column;
                        return;
                    }
                }
            }

            //Fill left block column
            for (byte column = 0; column < blocksize; column++)
            {
                for (byte row = (byte)(size - 1); row >= blocksize; row--)
                {
                    if (board[row, column] == 0)
                    {
                        //Work on this cell
                        NextRow = row;
                        NextColumn = column;
                        return;
                    }
                }
            }

            //Fill Inner by ringing toward bottom right corner
            byte turnRow = blocksize;
            for (byte column = blocksize; column < size; column++)
            {
                for (byte row = (byte)(size - 1); row >= blocksize; row--)
                {
                    if (board[row, column] == 0)
                    {
                        //Work on this cell
                        NextRow = row;
                        NextColumn = column;
                        return;
                    }
                    if (row == turnRow)
                    {
                        for (byte column2 = (byte)(column + 1); column2 < size; column2++)
                        {
                            if (board[row, column2] == 0)
                            {
                                //Work on this cell
                                NextRow = row;
                                NextColumn = column2;
                                return;
                            }
                        }
                        turnRow++;
                    }
                }
            }
        }

        public static void FindNextCellByRandomPattern(byte[,] board, out byte NextRow, out byte NextColumn)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            byte size = (byte)board.GetLength(0);
            List<Coordinates> OpenCells = new List<Coordinates>();
            NextRow = 254;
            NextColumn = 254;

            for (byte row = 0; row < size; row++)
            {
                for (byte column = 0; column < size; column++)
                {
                    if (board[row, column] == 0)
                    {
                        OpenCells.Add(new Coordinates() { Row = row, Column = column });
                    }
                }
            }

            if (OpenCells.Count == 0) return;
            Random rnd = new Random();
            Coordinates RandomCell = OpenCells[rnd.Next(OpenCells.Count)];
            NextRow = RandomCell.Row;
            NextColumn = RandomCell.Column;
        }

        public static void FindNextCellByFewestOptionsPattern(byte[,] board, List<byte>[,] possibleAnswers, out byte NextRow, out byte NextColumn)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (possibleAnswers == null)
            {
                throw new ArgumentNullException(nameof(possibleAnswers));
            }

            byte size = (byte)board.GetLength(0);
            byte FoundOptions = (byte)(size + 1);
            Coordinates FoundOpenCell = new Coordinates() { Row = 254, Column = 254 };

            for (byte row = 0; row < size; row++)
            {
                for (byte column = 0; column < size; column++)
                {
                    if (board[row, column] == 0 && possibleAnswers[row, column].Count < FoundOptions && possibleAnswers[row, column].Count > 0)
                    {
                        FoundOpenCell.Row = row;
                        FoundOpenCell.Column = column;
                        FoundOptions = (byte)possibleAnswers[row, column].Count;
                    }
                }
            }

            NextRow = FoundOpenCell.Row;
            NextColumn = FoundOpenCell.Column;
        }

        public static void FindNextCellByMostOptionsPattern(byte[,] board, List<byte>[,] possibleAnswers, out byte NextRow, out byte NextColumn)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (possibleAnswers == null)
            {
                throw new ArgumentNullException(nameof(possibleAnswers));
            }

            byte size = (byte)board.GetLength(0);
            byte FoundOptions = 0;
            Coordinates FoundOpenCell = new Coordinates() { Row = 254, Column = 254 };

            for (byte row = 0; row < size; row++)
            {
                for (byte column = 0; column < size; column++)
                {
                    if (board[row, column] == 0 && possibleAnswers[row, column].Count > FoundOptions && possibleAnswers[row, column].Count > 0)
                    {
                        FoundOpenCell.Row = row;
                        FoundOpenCell.Column = column;
                        FoundOptions = (byte)possibleAnswers[row, column].Count;
                    }
                }
            }

            NextRow = FoundOpenCell.Row;
            NextColumn = FoundOpenCell.Column;
        }

        private struct Coordinates
        {
            public byte Row;
            public byte Column;
        }
        #endregion

        #region FindNextOption Functions
        public static byte FindNextOptionByFirst(List<byte> possibleAnswers)
        {
            if (possibleAnswers == null)
            {
                throw new ArgumentNullException(nameof(possibleAnswers));
            }

            return possibleAnswers[0];
        }

        public static byte FindNextOptionByRandom(List<byte> possibleAnswers)
        {
            if (possibleAnswers == null)
            {
                throw new ArgumentNullException(nameof(possibleAnswers));
            }

            Random rnd = new Random();
            return possibleAnswers[rnd.Next(possibleAnswers.Count)];
        }
        #endregion
    }
}
