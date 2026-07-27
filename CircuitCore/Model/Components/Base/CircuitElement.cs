using MatrixLib;

namespace CircuitCore.Model.Components.Base
{
    public abstract class CircuitElement : ICircuitElement
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public int NodeA { get; set; }
        public int NodeB { get; set; }

        protected CircuitElement(string name, int nodeA, int nodeB)
        {
            Name = name;
            NodeA = nodeA;
            NodeB = nodeB;
        }
        protected bool NodeAIsGround => NodeA == NodeIndex.Ground;
        protected bool NodeBIsGround => NodeB == NodeIndex.Ground;
        protected double VoltageAt(Matrix result, int nodeIndex) => nodeIndex == NodeIndex.Ground ? 0 : result[nodeIndex, 0];
        public abstract void Stamp(Matrix conductance, Matrix currentVector, double dt);
        public virtual void OnSolved(Matrix result) { }
    }
}
