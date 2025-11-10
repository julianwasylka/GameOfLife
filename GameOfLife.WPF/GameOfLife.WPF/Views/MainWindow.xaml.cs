using Microsoft.Win32;  
using System.Windows.Media;  
using System.Windows.Media.Imaging;
using System.IO;
using GameOfLife.WPF.Converters;
using GameOfLife.WPF.ViewModels;
using Microsoft.Win32;
using System.Drawing;
using System.Globalization;
using System.IO;
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

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainViewModel vm) return;
            if (vm.IsRunning)
            {
                MessageBox.Show("Zatrzymaj symulację przed zapisem obrazu", "Uwaga");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Obraz PNG (*.png)|*.png",
                Title = "Zapisz mapę jako obraz",
                FileName = "mapa.png"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var itemsPresenter = FindVisualChild<ItemsPresenter>(BoardControl);
                    if (itemsPresenter == null) return;

                    var canvas = VisualTreeHelper.GetChild(itemsPresenter, 0) as Canvas;
                    if (canvas == null) return;

                    int width = (int)canvas.ActualWidth;
                    int height = (int)canvas.ActualHeight;

                    Transform oldTransform = BoardControl.LayoutTransform;
                    BoardControl.LayoutTransform = null;

                    BoardControl.Measure(new System.Windows.Size(width, height));
                    BoardControl.Arrange(new Rect(0, 0, width, height));

                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Pbgra32);
                    rtb.Render(BoardControl);

                    BoardControl.LayoutTransform = oldTransform;

                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));

                    using (var fs = new FileStream(dialog.FileName, FileMode.Create))
                    {
                        encoder.Save(fs);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas zapisu obrazu: {ex.Message}", "Błąd zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    return typedChild;
                }
                var result = FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}