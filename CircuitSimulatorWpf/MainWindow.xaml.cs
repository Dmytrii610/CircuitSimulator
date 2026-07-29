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
    }
}