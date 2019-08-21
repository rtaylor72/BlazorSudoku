using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BlazorSudoku.Core {
    public class BoardBlock : INotifyPropertyChanged {
        private readonly ObservableCollection<ObservableCollection<Cell>> _rows;

        public ObservableCollection<ObservableCollection<Cell>> GridRows {
            get {
                return _rows;
            }
        }

        public BoardBlock(byte iPuzzleSize) {
            byte iBlockSize = (byte)Math.Sqrt(iPuzzleSize);
            _rows = new ObservableCollection<ObservableCollection<Cell>>();
            for (int i = 0; i < iBlockSize; i++) {
                ObservableCollection<Cell> Col = new ObservableCollection<Cell>();
                for (int j = 0; j < iBlockSize; j++) {
                    Cell c = new Cell(iPuzzleSize);
                    Col.Add(c);
                }
                _rows.Add(Col);
            }
        }

        bool _IsValid = true;
        public bool IsValid {
            get {
                return _IsValid;
            }
            set {
                _IsValid = value;
                UpdateProperty("IsValid");
            }
        }

        public bool CheckIsValid() {
            bool valid = true;
            bool[] used = new bool[_rows.Count * _rows.Count];
            foreach (ObservableCollection<Cell> row in _rows) {
                foreach (Cell cell in row) {
                    if (cell.Value > 0) {
                        if (used[cell.Value - 1]) {
                            //duplicate value
                            foreach (ObservableCollection<Cell> rx in _rows) {
                                foreach (Cell cx in rx) {
                                    if (cx.Value == cell.Value) cx.IsValid = false;
                                }
                            }
                            valid = false;
                        } else {
                            used[cell.Value - 1] = true;
                        }
                    }
                }
            }
            IsValid = valid;
            return true;
        }

        public Cell this[byte row, byte col] {
            get {
                return _rows[row][col];
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void UpdateProperty(string strPropertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(strPropertyName));
        }
        #endregion
    }
}
