using CircuitCore.Model.Components.Base;
using MatrixLib;


namespace CircuitCore.Model.Components
{
    public class Resistor : CircuitElement
    {
        public double Resistance { get; set; }

        public Resistor(string name, int nodeA, int nodeB, double resistanse)
            :base(name,nodeA,nodeB)
        {
            Resistance = resistanse;
        }

        public override void Stamp(Matrix conductance, Matrix currentVector, double dt)
        {
            var g = 1 / Resistance;

            if (!NodeAIsGround) conductance[NodeA, NodeA] += g;
            if (!NodeBIsGround) conductance[NodeB, NodeB] += g;
            if (!NodeAIsGround && !NodeBIsGround)
            {
                conductance[NodeB, NodeA] -= g;
                conductance[NodeA, NodeB] -= g;
            }
        }
    }
}
