using NUnit.Framework;
using CircuitCore.Model.Components;
using MatrixLib;

[TestFixture]
public class ResistorTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void StampWritesCorrectConductances()
    {
        var conductance = new Matrix(2, 2);
        var current = new Matrix(2, 1);
        var resistor = new Resistor("R1",0,1,100);

        resistor.Stamp(conductance, current, 0);
        Assert.That(conductance[0, 0], Is.EqualTo(0.01).Within(1e-9));
        Assert.That(conductance[1, 1], Is.EqualTo(0.01).Within(1e-9));
        Assert.That(conductance[0, 1], Is.EqualTo(-0.01).Within(1e-9));
        Assert.That(conductance[1, 0], Is.EqualTo(-0.01).Within(1e-9));
    }
}
