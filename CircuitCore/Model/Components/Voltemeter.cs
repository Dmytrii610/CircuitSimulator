using CircuitCore.Math;
using CircuitCore.Model.Components.Base;
using MatrixLib;

namespace CircuitCore.Model.Components
{
    public class Voltemeter : CircuitElement, IMeasure
    {
        public string MeasurementLabel => $"Napięcie: {Name}";
        public double MeasuredValue { get; private set; }

        public Voltemeter(string name, int nodeA, int nodeB)
            : base(name, nodeA, nodeB)
        {

        }
        public override void Stamp(Matrix conductance, Matrix currentVector, double dt)
        {
            throw new NotImplementedException();
        }
        public override void OnSolved(Matrix result)
        {
            MeasuredValue = VoltageAt(result, NodeA) - VoltageAt(result, NodeB);
        }
    }
}
