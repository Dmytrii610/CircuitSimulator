using MatrixLib;

namespace CircuitCore.Math
{
    public interface IMeasure
    {
       string MeasurementLabel { get; }
       double MeasuredValue { get; }
    }
}
