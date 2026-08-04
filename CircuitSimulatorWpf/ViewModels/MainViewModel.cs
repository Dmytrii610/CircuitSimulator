using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CircuitCore.Model.Components;
using System.Windows;

namespace CircuitSimulatorWpf.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<ElementCategoryViewModel> Categories { get; } = new();
        public ObservableCollection<CanvasElementViewModel> PlacedElements { get; } = new();
        public ObservableCollection<WireViewModel> Wires { get; } = new();
        private PinViewModel pendingPin;
        public MainViewModel()
        {
            var sources = new ElementCategoryViewModel { Name = "Źródła zasilania" };
            sources.Items.Add(new ElementPaletteItemViewModel
            {
                DisplayName = "Źródło napięcia DC",
                IconPath = "pack://application:,,,/Assets/dc_voltage_source.svg",
                CreateElement = () => new VoltageSource("VS1", 0, 0, 5.0)
            });
            Categories.Add(sources);

            var dcElements = new ElementCategoryViewModel { Name = "Elementy" };
            dcElements.Items.Add(new ElementPaletteItemViewModel
            {
                DisplayName = "Rezystor",
                IconPath = "pack://application:,,,/Assets/resistor.svg",
                CreateElement = () => new Resistor("R1", 0, 0, 1000)
            });
            dcElements.Items.Add(new ElementPaletteItemViewModel
            {
                DisplayName = "Kondensator",
                IconPath = "pack://application:,,,/Assets/capasitor.svg",
                CreateElement = () => new Capasitor("C1", 0, 0, 100)
            });
            Categories.Add(dcElements);
            var measureElements = new ElementCategoryViewModel { Name = "Elementy pomiarowe" };
            measureElements.Items.Add(new ElementPaletteItemViewModel
            {
                DisplayName = "Ammeter",
                IconPath = "pack://application:,,,/Assets/ammeter.svg",
                CreateElement = () => new Ammeter("A1", 0, 0)
            });
            Categories.Add(measureElements);
        }
        public void HandlePinClick(PinViewModel pin)
        {
            if (pendingPin == null)
            {
                pendingPin = pin;
                return;
            }

            if (pendingPin == pin)
            {
                pendingPin = null;
                return;
            }

            Wires.Add(new WireViewModel(pendingPin, pin));
            pendingPin = null;
        }
        public void PlaceElement(ElementPaletteItemViewModel paletteItem, Point position)
        {
            var element = paletteItem.CreateElement();
            var canvasElement = new CanvasElementViewModel(
                element,
                paletteItem.IconPath,
                paletteItem.DisplayName,
                position.X,
                position.Y);

            PlacedElements.Add(canvasElement);
        }
    }
}