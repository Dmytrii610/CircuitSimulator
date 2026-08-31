using CircuitSimulatorWpf.ViewModels;
using System;
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
        private IPinOwner draggedOwner;
        private Point dragStartPoint;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void StartDrag(FrameworkElement fe, IPinOwner owner, MouseButtonEventArgs e)
        {
            draggedOwner = owner;
            dragStartPoint = e.GetPosition(SimulationCanvas);
            fe.CaptureMouse();
            e.Handled = true;
        }

        private void ContinueDrag(FrameworkElement fe, MouseEventArgs e)
        {
            if (draggedOwner == null || e.LeftButton != MouseButtonState.Pressed)
                return;

            var currentPosition = e.GetPosition(SimulationCanvas);
            double deltaX = currentPosition.X - dragStartPoint.X;
            double deltaY = currentPosition.Y - dragStartPoint.Y;

            double newX = draggedOwner.X + deltaX;
            double newY = draggedOwner.Y + deltaY;

            double maxX = Math.Max(0, SimulationCanvas.ActualWidth - fe.ActualWidth);
            double maxY = Math.Max(0, SimulationCanvas.ActualHeight - fe.ActualHeight);

            draggedOwner.X = Math.Clamp(newX, 0, maxX);
            draggedOwner.Y = Math.Clamp(newY, 0, maxY);

            dragStartPoint = currentPosition;
        }

        private void EndDrag(FrameworkElement fe)
        {
            fe.ReleaseMouseCapture();
            draggedOwner = null;
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

        private void CanvasElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is CanvasElementViewModel vm)
                StartDrag(fe, vm, e);
        }

        private void CanvasElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement fe)
                ContinueDrag(fe, e);
        }

        private void CanvasElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe)
                EndDrag(fe);
        }

        private void GroundMarker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is GroupMarkerViewModel vm)
                StartDrag(fe, vm, e);
        }

        private void GroundMarker_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement fe)
                ContinueDrag(fe, e);
        }

        private void GroundMarker_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe)
                EndDrag(fe);
        }

        private void Pin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is PinViewModel pin)
            {
                var vm = (MainViewModel)DataContext;
                vm.HandlePinClick(pin);
                e.Handled = true;
            }
        }
    }
}