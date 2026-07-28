using CircuitCore.Math;
using MatrixLib;

namespace CircuitCore.Model.Components
{
    public class Ammeter : VoltageSource, IMeasure
    {
        public string MeasurementLabel => $"Prąd: {Name}";
        public double MeasuredValue { get; private set; }

        public Ammeter(string name, int nodeA, int nodeB)
            :base(name,nodeA,nodeB,0)
        {

        }
        public override void OnSolved(Matrix result)
        {
            MeasuredValue = result[BranchIndex, 0];
        }
    }
}
