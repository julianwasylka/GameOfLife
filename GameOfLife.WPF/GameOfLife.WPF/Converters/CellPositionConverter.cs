using System;
using System.Globalization;
using System.Windows.Data;

namespace GameOfLife.WPF.Converters
{
    public class CellPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value * AppConfig.CellSize;
        }

        // Not needed
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double pixelValue = (double)value;

            return (int)(pixelValue / AppConfig.CellSize);
        }
    }
}