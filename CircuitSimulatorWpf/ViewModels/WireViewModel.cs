using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;
using System.Windows;
namespace CircuitSimulatorWpf.ViewModels
{
    public partial class WireViewModel : ObservableObject
    {
        public PinViewModel StartPin { get; }
        public PinViewModel EndPin { get; }
        public double X1 => StartPin.AbsoluteX;
        public double Y1 => StartPin.AbsoluteY;
        public double X2 => EndPin.AbsoluteX;
        public double Y2 => EndPin.AbsoluteY;
        public double MidX => manualMidX ?? (X1 + X2) / 2;
        public Point P0 => new Point(X1,Y1);
        public Point P1 => new Point(MidX,Y1);
        public Point P2 => new Point(MidX,Y2);
        public Point P3 => new Point(X2,Y2);
        public Brush WireBrush => IsActive ? Brushes.Blue : Brushes.Black;

        private double? manualMidX;

        [ObservableProperty] private bool isActive;
        public WireViewModel(PinViewModel pinFrom, PinViewModel pinTo)
        {
            StartPin = pinFrom;
            EndPin = pinTo;
            StartPin.PropertyChanged += (_, e) => OnPinMoved(e.PropertyName);
            EndPin.PropertyChanged += (_, e) => OnPinMoved(e.PropertyName);
        }
        public void SetMidX(double x)
        {
            manualMidX = x;
            RaisePointsChanged();
        }

       
        private void OnPinMoved(string propertyName)
        {
            if (propertyName == nameof(PinViewModel.AbsoluteX) || propertyName == nameof(PinViewModel.AbsoluteY))
            {
                OnPropertyChanged(nameof(X1));
                OnPropertyChanged(nameof(Y1));
                OnPropertyChanged(nameof(X2));
                OnPropertyChanged(nameof(Y2));
                RaisePointsChanged();
            }
        }
        private void RaisePointsChanged()
        {
            OnPropertyChanged(nameof(MidX));
            OnPropertyChanged(nameof(P0));
            OnPropertyChanged(nameof(P1));
            OnPropertyChanged(nameof(P2));
            OnPropertyChanged(nameof(P3));

        }
        partial void OnIsActiveChanged(bool value)
        {
            OnPropertyChanged(nameof(WireBrush));
        }
    }
}
