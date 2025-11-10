using System;
using System.Globalization;
using System.Windows.Data;

namespace GameOfLife.WPF.Converters
{
    public class BoardSizeToPixelsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int boardSize = System.Convert.ToInt32(values[0]);

                int cellSize = System.Convert.ToInt32(values[1]);

                return (double)(boardSize * cellSize);
            }
            catch
            {
                return 0.0;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}