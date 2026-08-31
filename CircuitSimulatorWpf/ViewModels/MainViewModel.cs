using CircuitCore.Model;
using CircuitCore.Model.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MathGraphLib;
using System.Collections.ObjectModel;
using System.Windows;

namespace CircuitSimulatorWpf.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<ElementCategoryViewModel> Categories { get; } = new();
        public ObservableCollection<CanvasElementViewModel> PlacedElements { get; } = new();
        public ObservableCollection<GroupMarkerViewModel> GroundMarkers { get; } = new();
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

            var symbols = new ElementCategoryViewModel { Name = "Symbole" };
            symbols.Items.Add(new ElementPaletteItemViewModel
            {
                DisplayName = "Masa (GND)",
                IconPath = "pack://application:,,,/Assets/ground.svg",
                IsGroundSymbol = true,
                CreateElement = null
            });
            Categories.Add(symbols);

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
            if (paletteItem.IsGroundSymbol)
            {
                GroundMarkers.Add(new GroupMarkerViewModel(position.X, position.Y));
                return;
            }

            var element = paletteItem.CreateElement();
            PlacedElements.Add(new CanvasElementViewModel(element, paletteItem.IconPath, paletteItem.DisplayName, position.X, position.Y));
        }
        private (Dictionary<PinViewModel, int> pinToGroup, int groupCount) BuildGroups()
        {
            var pins = PlacedElements.SelectMany(e => new[] { e.PinA, e.PinB })
                                     .Concat(GroundMarkers.Select(g => g.Pin))
                                     .ToList();
            var parent = new Dictionary<PinViewModel, PinViewModel>();
            foreach (var pin in pins)
            {
                parent[pin] = pin;
            }
            PinViewModel Find(PinViewModel p)
            {
                while (parent[p] != p)
                {
                    p = parent[p];
                }
                return p;
            }
            void Union(PinViewModel a, PinViewModel b)
            {
                var ra = Find(a);
                var rb = Find(b);
                if (ra != rb) parent[ra] = rb;
            }
            foreach (var wire in Wires)
            {
                Union(wire.StartPin, wire.EndPin);
            }
            var rootToIndex = new Dictionary<PinViewModel, int>();
            var pinToGroup = new Dictionary<PinViewModel, int>();
            foreach (var p in pins)
            {
                var root = Find(p);
                if (!rootToIndex.TryGetValue(root, out int idx))
                {
                    idx = rootToIndex.Count;
                    rootToIndex[root] = idx;
                }
                pinToGroup[p] = idx;
            }
            return (pinToGroup, rootToIndex.Count);
        }
        public Circuit BuildCircuit()
        {
            var (pinToGroup, groupCount) = BuildGroups();

            // grupy zawierające którykolwiek pin masy = uziemione
            var groundGroups = new HashSet<int>(GroundMarkers.Select(g => pinToGroup[g.Pin]));

            var circuit = new Circuit();
            var groupToNode = new Node[groupCount];
            for (int i = 0; i < groupCount; i++)
                groupToNode[i] = circuit.AddNode($"N{i}", isGround: groundGroups.Contains(i));

            foreach (var el in PlacedElements)
            {
                int groupA = pinToGroup[el.PinA];
                int groupB = pinToGroup[el.PinB];
                circuit.AddElement(el.Element, groupToNode[groupA], groupToNode[groupB]);
            }

            return circuit;
        }
        [RelayCommand]
        private void RunSimulation()
        {
            var circuit = BuildCircuit();
            circuit.Solve();
        }
    }
}