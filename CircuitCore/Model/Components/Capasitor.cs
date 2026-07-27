using CircuitCore.Model.Components.Base;
using MatrixLib;

namespace CircuitCore.Model.Components
{
    public class Capasitor : CircuitElement
    {
        public double Capacity { get; set; }
        private double previousVoltage = 0;

        public Capasitor(string name, int nodeA, int nodeB, double capacity)
            :base(name,nodeA,nodeB)
        {
            Capacity = capacity;
        }
        public override void Stamp(Matrix conductance, Matrix currentVector, double dt)
        {
            double gEq = Capacity / dt;
            double iEq = gEq * previousVoltage;
            if (!NodeAIsGround)
            {
                conductance[NodeA, NodeA] += gEq;
                currentVector[NodeA, 0] += iEq;
            }
            if (!NodeBIsGround)
            {
                conductance[NodeB, NodeB] += gEq;
                currentVector[NodeB, 0] -= iEq;
            }
            if (!NodeAIsGround && !NodeBIsGround)
            {
                conductance[NodeA, NodeB] -= gEq;
                conductance[NodeB, NodeA] -= gEq;
            }
        }
        public override void OnSolved(Matrix result)
        {
            previousVoltage = VoltageAt(result, NodeA) - VoltageAt(result, NodeB);
        }
    }
}