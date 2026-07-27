using MatrixLib;

namespace CircuitCore.Model.Components.Base
{
    public interface ICircuitElement
    {
        Guid Id { get; }
        string Name { get; set; }
        int NodeA { get; set; }
        int NodeB { get; set; }
        void Stamp(Matrix conductance, Matrix currentVector, double dt);
        void OnSolved(Matrix result) { }
    }
}
