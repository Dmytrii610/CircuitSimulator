using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CircuitCore.Model.Components;

namespace CircuitSimulatorWpf.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<ElementCategoryViewModel> Categories { get; } = new();

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
        }
    }
}