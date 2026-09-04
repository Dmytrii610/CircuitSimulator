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
        public PointCollection Points => BuildRoute();

        [ObservableProperty] private bool isActive;
        public WireViewModel(PinViewModel pinFrom, PinViewModel pinTo)
        {
            StartPin = pinFrom;
            EndPin = pinTo;
            StartPin.PropertyChanged += (_, e) => OnPinMoved(e.PropertyName);
            EndPin.PropertyChanged += (_, e) => OnPinMoved(e.PropertyName);
        }

       
        private void OnPinMoved(string propertyName)
        {
            if (propertyName == nameof(PinViewModel.AbsoluteX) || propertyName == nameof(PinViewModel.AbsoluteY))
            {
                OnPropertyChanged(nameof(X1));
                OnPropertyChanged(nameof(Y1));
                OnPropertyChanged(nameof(X2));
                OnPropertyChanged(nameof(Y2));
                OnPropertyChanged(nameof(Points));
            }
        }
        private PointCollection BuildRoute()
        {
            if (Math.Abs(Y1 - Y2) < 0.5)
                return new PointCollection { new Point(X1, Y1), new Point(X2, Y2) };

            if (Math.Abs(X1 - X2) < 0.5)
                return new PointCollection { new Point(X1, Y1), new Point(X2, Y2) };

            double midX = (X1 + X2) / 2;
            return new PointCollection
            {
                new Point(X1, Y1),
                new Point(midX, Y1),
                new Point(midX, Y2),
                new Point(X2, Y2)
            };
        }
    }
}
