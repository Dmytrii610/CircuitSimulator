using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CircuitSimulatorWpf.ViewModels
{
    public partial class ElementCategoryViewModel : ObservableObject
    {
        public string Name { get; init; }
        [ObservableProperty]
        private bool isExpanded;
        public ObservableCollection<ElementPaletteItemViewModel> Items { get; } = new();
    }
}
