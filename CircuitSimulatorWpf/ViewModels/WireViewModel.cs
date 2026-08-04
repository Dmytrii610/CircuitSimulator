using CommunityToolkit.Mvvm.ComponentModel;
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
        public WireViewModel(PinViewModel pinFrom, PinViewModel pinTo)
        {
            StartPin = pinFrom;
            EndPin = pinTo;
            StartPin.PropertyChanged += (_, e) => RaiseIfPositionChanged(e.PropertyName,true);
            EndPin.PropertyChanged += (_, e) => RaiseIfPositionChanged(e.PropertyName,false);
        }

        private void RaiseIfPositionChanged(string propertyName, bool isFrom)
        {
            if (propertyName == nameof(PinViewModel.AbsoluteX))
            {
                OnPropertyChanged(isFrom ? nameof(X1) : nameof(X2));
            }
            if (propertyName == nameof(PinViewModel.AbsoluteY))
            {
                OnPropertyChanged(isFrom ? nameof(Y1) : nameof(Y2));
            }
        }

    }
}
