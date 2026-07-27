namespace CircuitCore.Model.Components.Base
{
    public abstract class BranchCircuitElement : CircuitElement, IBranchElement
    {
        public int BranchIndex { get; set; } = NodeIndex.Ground;
        protected BranchCircuitElement(string name, int nodeA, int nodeB)
            :base(name,nodeA,nodeB)
        {

        }
    }
}
