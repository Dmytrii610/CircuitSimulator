using CircuitCore.Math;
using MatrixLib;

namespace CircuitCore.Model.Components
{
    public class Ohmmeter : CurrentSource, IMeasure
    {
        public string MeasurementLabel => $"Rezystancja: {Name}";
        public double MeasuredValue { get; private set; }
        public Ohmmeter(string name, int nodeA, int nodeB, double testCurrent = 1e-3)
            :base(name,nodeA,nodeB,testCurrent)
        {

        }
        public override void OnSolved(Matrix result)
        {
            MeasuredValue = (VoltageAt(result, NodeA) - VoltageAt(result, NodeB)) / Current;
        }
    }
}
