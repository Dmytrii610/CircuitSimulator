using CircuitCore.Model.Components.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CircuitCore.Math;
using SharpVectors.Dom;
using System;
using System.Collections.Generic;
using System.Text;
using CircuitCore.Model.Components;

namespace CircuitSimulatorWpf.ViewModels
{
    public partial class CanvasElementViewModel : ObservableObject, IPinOwner
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
        public string ParameterLabel => Element switch
        {
            Ammeter or Ohmmeter => null,
            Resistor => "Rezystancja [Ohm]",
            Capasitor => "Pojemność [F]",
            Inductor => "Induktywność [H]",
            VoltageSource => "Wartość napięcia zasilającego [V]",
            CurrentSource => "Wartość prądu zasilającego [A]",
            _ => null
        };

        public double ParameterValue
        {
            get => Element switch
            {
                Ammeter or Ohmmeter => 0,
                Resistor r => r.Resistance,
                Capasitor c => c.Capacity,
                Inductor i => i.Inductance,
                VoltageSource vS => vS.Voltage,
                CurrentSource cS => cS.Current,
                _ => 0
            };
            set
            {
                if (Element is Ammeter or Ohmmeter) return;
                if (Element is Resistor r) r.Resistance = value;
                if (Element is Capasitor c) c.Capacity = value;
                if (Element is Inductor i) i.Inductance = value;
                if (Element is VoltageSource vS) vS.Voltage = value;
                if (Element is CurrentSource cS) cS.Current = value;
                OnPropertyChanged(nameof(ParameterValue));
            }
        }
        public string MeasurementText
            => Element is IMeasure m ? $"{m.MeasurementLabel} : {m.MeasuredValue:0.###}" : null;
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
        public void RefreshMeasurement()
        {
            if (Element is IMeasure)
            {
                OnPropertyChanged(nameof(MeasurementText));
            }
        }
    }
}
