using System;
using System.ComponentModel;
using System.Text;

namespace CircuitSimulatorWpf.ViewModels
{
    public interface IPinOwner : INotifyPropertyChanged
    {
        double X { get; set; }
        double Y { get; set; }
    }
}
