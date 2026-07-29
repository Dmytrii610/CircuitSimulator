using CircuitSimulatorWpf.ViewModels;
using System.Windows;
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
    }
}