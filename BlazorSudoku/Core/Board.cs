using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BlazorSudoku.Core
{
    public class Board : INotifyPropertyChanged
    {
        private const string ERROR_ARRAY_INCORRECT_SIZE = "Array is incorrect size";

        public ObservableCollection<ObservableCollection<BoardBlock>> BoardBlock { get; }

        public byte Size
        {
            get
            {
                return (byte)(BoardBlock.Count * BoardBlock.Count);
            }
        }

        public Board(byte iPuzzleSize)
        {
            int size = (int)Math.Sqrt(iPuzzleSize);
            BoardBlock = new ObservableCollection<ObservableCollection<BoardBlock>>();
            for (int i = 0; i < size; i++)
            {
                ObservableCollection<BoardBlock> Col = new ObservableCollection<BoardBlock>();
                for (int j = 0; j < size; j++)
                {
                    BoardBlock g = new BoardBlock(iPuzzleSize);
                    Col.Add(g);
                }
                BoardBlock.Add(Col);
            }
        }

        public byte[,] ToArray()
        {
            byte[,] board = new byte[Size, Size];
            for (byte row = 0; row < Size; row++)
            {
                for (byte column = 0; column < Size; column++)
                {
                    board[row, column] = this[row, column].Value;
                }
            }
            return board;
        }

        public void FromArray(byte[,] board)
        {
            if (board == null || board.GetLength(0) != board.GetLength(1) || board.GetLength(0) != Size)
            {
                throw new ArgumentOutOfRangeException(nameof(board), board, message: ERROR_ARRAY_INCORRECT_SIZE);
            }

            for (byte row = 0; row < Size; row++)
            {
                for (byte column = 0; column < Size; column++)
                {
                    this[row, column].Value = board[row, column];
                }
            }
        }

        bool _IsValid = true;
        public bool IsValid
        {
            get
            {
                return _IsValid;
            }
            set
            {
                _IsValid = value;
                UpdateProperty("IsValid");
            }
        }

        public bool CheckIsValid()
        {
            //Clear values
            for (byte row = 0; row < Size; row++)
            {
                for (byte column = 0; column < Size; column++)
                {
                    this[row, column].IsValid = true;
                }
            }

            //Check for valid
            bool valid = true;

            _IsFull = true;
            for (byte row = 0; row < Size; row++)
            {
                bool isRowValid = CheckRowIsValid(row);
                bool[] IsColumnValid = new bool[Size];
                for (byte column = 0; column < Size; column++)
                {
                    if (this[row, column].Value == 0) _IsFull = false;
                    IsColumnValid[column] = CheckColumnIsValid(column);
                }
                for (byte column = 0; column < Size; column++)
                {
                    bool BoardBlockValid = BoardBlock[row / BoardBlock.Count][column / BoardBlock.Count].CheckIsValid();
                    if (!IsColumnValid[column] || !isRowValid || !BoardBlockValid) valid = false;
                    this[row, column].IsValid = IsColumnValid[column] & isRowValid & this[row, column].IsValid;
                }
            }
            IsValid = valid;
            UpdateProperty("IsFull");
            return valid;
        }

        bool _IsFull = false;
        public bool IsFull
        {
            get
            {
                return _IsFull;
            }
        }

        private bool CheckRowIsValid(byte row)
        {
            bool[] used = new bool[Size];
            for (byte column = 0; column < Size; column++)
            {
                Cell cell = this[row, column];
                if (cell.Value > 0)
                {
                    if (used[cell.Value - 1])
                    {
                        return false;
                    }
                    else
                    {
                        used[cell.Value - 1] = true;
                    }
                }
            }
            return true;
        }

        private bool CheckColumnIsValid(byte column)
        {
            bool[] used = new bool[Size];
            for (byte row = 0; row < Size; row++)
            {
                Cell cell = this[row, column];
                if (cell.Value > 0)
                {
                    if (used[cell.Value - 1])
                    {
                        return false;
                    }
                    else
                    {
                        used[cell.Value - 1] = true;
                    }
                }
            }
            return true;
        }

        public Cell this[byte row, byte column]
        {
            get
            {
                return BoardBlock[row / BoardBlock.Count][column / BoardBlock.Count][(byte)(row % BoardBlock.Count), (byte)(column % BoardBlock.Count)];
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void UpdateProperty(string strPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(strPropertyName));
        }
        #endregion
    }
}