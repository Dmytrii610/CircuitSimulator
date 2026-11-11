using CircuitSimulatorWpf.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CircuitSimulatorWpf.Views;

namespace CircuitSimulatorWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IPinOwner draggedOwner;
        private Point dragStartPoint;
        private WireViewModel draggedWire;
        private double dragStartMouseX;
        private double dragStartMidX;

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
            {
                StartDrag(fe, vm, e);
            }
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
        private void CanvasElement_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is CanvasElementViewModel vm)
            {
                var menu = new ContextMenu();
                var deleteItem = new MenuItem { Header = "Usuń" };
                deleteItem.Click += (_, _) => ((MainViewModel)DataContext).DeleteElement(vm);
                menu.Items.Add(deleteItem);

                if (vm.ParameterLabel != null)
                {
                    var editItem = new MenuItem { Header = "Zmień" };
                    editItem.Click += (_, _) =>
                    {
                        var editVm = new EditElementViewModel(vm);
                        var window = new EditElementWindow
                        {
                            DataContext = editVm,
                            Owner = this
                        };
                        if (window.ShowDialog() == true)
                        {
                            editVm.Apply();
                            menu.Items.Add(editItem);
                        }
                    };   
                    
                }
                fe.ContextMenu = menu;
                menu.IsOpen = true;
            }
            e.Handled = true;
        }
        private void GroundMarker_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is GroupMarkerViewModel vm)
            {
                var menu = new ContextMenu();
                var deleteItem = new MenuItem { Header = "Usuń" };
                deleteItem.Click += (_, _) => ((MainViewModel)DataContext).DeleteGround(vm);
                menu.Items.Add(deleteItem);

                fe.ContextMenu = menu;
                menu.IsOpen = true;
            }
            e.Handled = true;
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

        private void WireBend_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is FrameworkElement fe && fe.DataContext is WireViewModel wire)
            {
                draggedWire = wire;
                dragStartMouseX = e.GetPosition(SimulationCanvas).X;
                dragStartMidX = wire.MidX;
                fe.CaptureMouse();
                e.Handled = true;
            }
        }
        private void WireBend_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedWire == null || e.LeftButton != MouseButtonState.Pressed) return;
            double currentMouseX = e.GetPosition(SimulationCanvas).X;
            double delta = currentMouseX - dragStartMouseX;
            double proposed = dragStartMidX + delta;
            double boundedX = Math.Clamp(proposed, 0, SimulationCanvas.ActualWidth);
            draggedWire.SetMidX(boundedX);
        }
        private void WireBend_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe) fe.ReleaseMouseCapture();
            draggedWire = null;
        }
    }
}