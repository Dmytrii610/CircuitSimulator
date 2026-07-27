using CircuitCore.Model.Components.Base;
using MatrixLib;

namespace CircuitCore.Model.Components
{
    public class CurrentSource : CircuitElement, ISource
    {
        public double Current { get; set; }
        double ISource.Value => Current;

        public CurrentSource(string name, int nodeA, int nodeB, double current)
            :base(name,nodeA,nodeB)
        {
            Current = current;
        }
        public override void Stamp(Matrix conductance, Matrix currentVector, double dt)
        {
            if (!NodeAIsGround) currentVector[NodeA, 0] -= Current;
            if (!NodeBIsGround) currentVector[NodeB, 0] += Current;
        }
    }
}
