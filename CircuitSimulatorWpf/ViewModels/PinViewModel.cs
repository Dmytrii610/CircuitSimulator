using CommunityToolkit.Mvvm.ComponentModel;
namespace CircuitSimulatorWpf.ViewModels
{
    public enum PinRole
    {
        A,
        B
    }
    public partial class PinViewModel : ObservableObject
    {
        public IPinOwner Owner { get; }
        public PinRole Role { get; }
        public double OffsetX { get; }
        public double OffsetY { get; }
        public double AbsoluteX => Owner.X + OffsetX;
        public double AbsoluteY => Owner.Y + OffsetY;
        public PinViewModel(IPinOwner owner, PinRole role, double offsetX, double offsetY) 
        {
            Owner = owner;
            Role = role;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Owner.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(IPinOwner.X) || e.PropertyName == nameof(IPinOwner.Y))
                {
                    OnPropertyChanged(nameof(AbsoluteX));
                    OnPropertyChanged(nameof(AbsoluteY));
                }
            };
        }
    }
}
