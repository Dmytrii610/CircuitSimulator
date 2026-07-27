using NUnit.Framework;
using CircuitCore.Model.Components;
using MatrixLib;

[TestFixture]
public class ResistorParametrizedTests
{
    [TestCase(100, 0.01)]
    [TestCase(1000, 0.001)]
    [TestCase(50, 0.02)]
    public void Stamp_ForVariousResistances_ProducesCorrectConductance(double resistance, double expectedG)
    {
        var conductance = new Matrix(2, 2);
        var current = new Matrix(2, 1);
        var resistor = new Resistor("R1", 0, 1, resistance);
        resistor.Stamp(conductance, current, 0);
        Assert.That(conductance[0, 0], Is.EqualTo(expectedG).Within(1e-9));
    }
}