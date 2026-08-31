using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xaml;

namespace CircuitSimulatorWpf.ViewModels
{
    public partial class GroupMarkerViewModel : ObservableObject, IPinOwner
    {
        [ObservableProperty] private double x;
        [ObservableProperty] private double y;
        
        public PinViewModel Pin { get; }
        public string DisplayName { get; init; }
        public string IconPath { get; init; }
        public GroupMarkerViewModel(double x, double y)
        {
            this.x = x;
            this.y = y;
            Pin = new PinViewModel(this, PinRole.A, 4, 0);
        }

    }
}
