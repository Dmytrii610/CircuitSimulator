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
                DisplayName = "Źródło napięcia",
                IconPath = "/Assets/voltage_source.png",
                CreateElement = () => new VoltageSource("VS1", 0, 0, 5.0)
            });
            Categories.Add(sources);

            var dcElements = new ElementCategoryViewModel { Name = "Elementy DC" };
            dcElements.Items.Add(new ElementPaletteItemViewModel
            {
                DisplayName = "Rezystor",
                IconPath = "/Assets/resistor.png",
                CreateElement = () => new Resistor("R1", 0, 0, 1000)
            });
            Categories.Add(dcElements);
        }
    }
}