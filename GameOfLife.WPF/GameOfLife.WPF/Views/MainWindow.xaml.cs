using GameOfLife.WPF.Converters;
using GameOfLife.WPF.ViewModels;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameOfLife.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CellPositionConverter _cellPosConverter = new CellPositionConverter();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BoardCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                var boardCanvas = sender as Canvas;
                if (boardCanvas == null) return;

                var mousePos = e.GetPosition(boardCanvas);

                int x = (int)_cellPosConverter.ConvertBack(mousePos.X, typeof(int), null, CultureInfo.CurrentCulture);
                int y = (int)_cellPosConverter.ConvertBack(mousePos.Y, typeof(int), null, CultureInfo.CurrentCulture);
                
                System.Drawing.Point logicalPosition = new System.Drawing.Point(x, y);

                if (vm.ToggleCellCommand.CanExecute(logicalPosition))
                {
                    vm.ToggleCellCommand.Execute(logicalPosition);
                }
            }
        }
    }
}