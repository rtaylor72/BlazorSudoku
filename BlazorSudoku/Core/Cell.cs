using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BlazorSudoku.Core {
    public class Cell : INotifyPropertyChanged {
        public Cell(byte iPuzzleSize) {
            PossibleValues = new List<string>(iPuzzleSize + 1)
            {
                String.Empty
            };
            for (byte i = 1; i <= iPuzzleSize; i++) {
                PossibleValues.Add(item: i.ToString());
            }
        }

        private byte _Value = 0;
        public byte Value {
            get {
                return _Value;
            }
            set {
                if (_Value != value) {
                    _Value = value;
                    UpdateProperty("Value");
                }
            }
        }

        CellDecoration _CellDecoration = CellDecoration.Normal;
        public CellDecoration CellDecoration {
            get {
                return _CellDecoration;
            }
            set {
                if (_CellDecoration != value) {
                    _CellDecoration = value;
                    UpdateProperty("CellDecoration");
                }
            }
        }

        bool _IsValid = true;
        public bool IsValid {
            get {
                return _IsValid;
            }
            set {
                if (_IsValid != value) {
                    _IsValid = value;
                    UpdateProperty("IsValid");
                }
            }
        }

        public List<string> PossibleValues {
            get;
            private set;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void UpdateProperty(string strPropertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(strPropertyName));
        }
        #endregion
    }
}