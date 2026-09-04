using CircuitCore.Math;
using CircuitCore.Model;
using CircuitCore.Model.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MathGraphLib;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Threading;

namespace CircuitSimulatorWpf.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<ElementCategoryViewModel> Categories { get; } = new();
        public ObservableCollection<CanvasElementViewModel> PlacedElements { get; } = new();
        public ObservableCollection<GroupMarkerViewModel> GroundMarkers { get; } = new();
        public ObservableCollection<WireViewModel> Wires { get; } = new();
        
        
        private PinViewModel pendingPin;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(StartSimulationCommand))]
        [NotifyCanExecuteChangedFor(nameof(StopSimulationCommand))]
        private bool isSimulating;
        private Circuit circuit;
        private CircuitSolver solver;
        private DispatcherTimer timer;

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
            measureElements.Items.Add(new ElementPaletteItemViewModel
            {
                DisplayName = "Voltmeter",
                IconPath = "pack://application:,,,/Assets/voltmeter.svg",
                CreateElement = () => new Voltmeter("V1", 0, 0)
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
            if (WireAlreadyExists(pendingPin, pin))
            {
                pendingPin = null;
                return;
            }
            Wires.Add(new WireViewModel(pendingPin, pin));
            pendingPin = null;
        }

        private bool WireAlreadyExists(PinViewModel pendingPin, PinViewModel pin)
        {
            return Wires.Any(w => (
            w.StartPin == pendingPin && w.EndPin == pin)
            || (w.StartPin == pin && w.EndPin == pendingPin));
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
        private List<string> ValidateGroundConnection(Dictionary<PinViewModel,int> pinToGroup, HashSet<int> groundGroups)
        {
            var err = new List<string>();
            var visited = new HashSet<int>(groundGroups);
            var queue = new Queue<int>(groundGroups);
            var adj = BuildGroupsAdjacency(pinToGroup);

            while (queue.Any())
            {
                var current = queue.Dequeue();
                foreach (var neighbor in adj[current])
                {
                    if (visited.Add(neighbor)) queue.Enqueue(neighbor);
                }
            }

            var allGroups = pinToGroup.Values.Distinct();
            foreach (var g in allGroups.Except(visited))
            {
                err.Add($"Node {g} does not have a connection with ground.");
            }
            return err;
        }

        private Dictionary<int,List<int>> BuildGroupsAdjacency(Dictionary<PinViewModel,int> pinToGroup)
        {
            var adj = new Dictionary<int, List<int>>();
            void AddEdge(int a, int b)
            {
                if (!adj.TryGetValue(a, out var lA)) adj[a] = lA = new List<int>();
                lA.Add(b);
                if (!adj.TryGetValue(b, out var lB)) adj[b] = lB = new List<int>();
                lB.Add(a);
            }
            foreach (var group in pinToGroup.Values.Distinct())
            {
                if (!adj.ContainsKey(group)) adj[group] = new List<int>();
            }
            foreach (var element in PlacedElements)
            {
                var groupA = pinToGroup[element.PinA];
                var groupB = pinToGroup[element.PinB];
                if (groupA != groupB) AddEdge(groupA, groupB);
            }
            return adj;
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
            try
            {
                circuit.Solve();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        [RelayCommand(CanExecute = nameof(CanStart))]
        private void StartSimulation()
        {
            try
            {
                circuit = BuildCircuit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd budowy obwodu");
                return;
            }

            solver = new CircuitSolver(circuit, timeStep: 0.01);

            foreach (var wire in Wires)
                wire.IsActive = true;

            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            timer.Tick += (_, _) => StepSimulation();
            timer.Start();

            IsSimulating = true;
        }
        private bool CanStart() => !IsSimulating;

        private void StepSimulation()
        {
            try
            {
                solver.Step();
            }
            catch (Exception ex)
            {
                StopSimulation();
                MessageBox.Show(ex.Message, "Błąd symulacji — zatrzymano");
                return;
            }

            foreach (var el in PlacedElements)
                el.RefreshMeasurement();
        }

        [RelayCommand(CanExecute = nameof(CanStop))]
        private void StopSimulation()
        {
            timer?.Stop();
            timer = null;

            foreach (var wire in Wires)
                wire.IsActive = false;

            IsSimulating = false;
        }
        private bool CanStop() => IsSimulating;
    }
}