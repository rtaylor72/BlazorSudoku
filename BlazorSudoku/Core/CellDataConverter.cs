//using System;
//using System.Windows.Data;

//namespace BlazorSudoku.Core {
//    public class CellDataConverter : IValueConverter {

//        #region IValueConverter Members

//        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
//            if (value.ToString() == "0") return string.Empty;
//            return value.ToString();
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
//            byte returnValue = 0;
//            byte.TryParse(value.ToString(), out returnValue);
//            return returnValue;
//        }

//        #endregion
//    }
//}