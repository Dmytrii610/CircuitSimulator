namespace CircuitCore.Model.Components.Base
{
    public interface IBranchElement : ICircuitElement
    {
        int BranchIndex { get; set; }
    }
}
