using CircuitSimulatorWpf.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CircuitSimulatorWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CanvasElementViewModel draggedElement;
        private Point dragStartPoint;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
        private void PaletteItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement fe)
            {
                if (fe.DataContext is ElementPaletteItemViewModel item)
                {
                    DragDrop.DoDragDrop(fe, item, DragDropEffects.Copy);
                }
            }
        }
        private void SimulationCanvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(ElementPaletteItemViewModel)) is ElementPaletteItemViewModel paletteItem)
            {
                var position = e.GetPosition((ItemsControl)sender);
                var vm = (MainViewModel)((FrameworkElement)sender).DataContext;
                vm.PlaceElement(paletteItem, position);
            }
        }
        private void CanvasElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedElement == null || e.LeftButton != MouseButtonState.Pressed)
                return;

            var currentPosition = e.GetPosition(SimulationCanvas);
            double deltaX = currentPosition.X - dragStartPoint.X;
            double deltaY = currentPosition.Y - dragStartPoint.Y;

            draggedElement.X += deltaX;
            draggedElement.Y += deltaY;

            dragStartPoint = currentPosition;
        }
        private void CanvasElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is CanvasElementViewModel vm)
            {
                draggedElement = vm;
                dragStartPoint = e.GetPosition(SimulationCanvas);
                fe.CaptureMouse();
                e.Handled = true;
            }
        }   
        private void CanvasElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe)
                fe.ReleaseMouseCapture();

            draggedElement = null;
        }
    }
}