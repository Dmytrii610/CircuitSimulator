using CircuitCore.Model.Components.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulatorWpf.ViewModels
{
    public class ElementPaletteItemViewModel
    {
        public string DisplayName { get; init; }
        public string IconPath { get; init; }
        public Func<ICircuitElement> CreateElement { get; init; }
    }
}
