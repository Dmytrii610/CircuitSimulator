using CircuitCore.Model.Components.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using SharpVectors.Dom;
using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulatorWpf.ViewModels
{
    public partial class CanvasElementViewModel : ObservableObject
    {
        public ICircuitElement Element { get; }
        public string IconPath { get; init; }
        public string DisplayName { get; init; }
        [ObservableProperty]
        private double x;
        [ObservableProperty]
        private double y;
        public PinViewModel PinA { get; }
        public PinViewModel PinB { get; }
        public CanvasElementViewModel(ICircuitElement element, string iconPath, string displayName, double x, double y)
        {
            Element = element;
            IconPath = iconPath;
            DisplayName = displayName;
            this.x = x;
            this.y = y;

            PinA = new PinViewModel(this, PinRole.A, 0, 20);
            PinB = new PinViewModel(this, PinRole.B, 40, 20);
        }
    }
}
