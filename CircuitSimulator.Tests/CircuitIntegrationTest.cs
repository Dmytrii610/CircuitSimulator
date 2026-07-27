using NUnit.Framework;
using CircuitCore.Model;
using CircuitCore.Model.Components;

[TestFixture]
public class CircuitIntegrationTests
{
    [Test]
    public void VoltageDivider_ProducesCorrectVoltage()
    {
        var circuit = new Circuit();
        var gnd = circuit.AddNode("GND", isGround: true);
        var vPlus = circuit.AddNode("V+");
        var mid = circuit.AddNode("Mid");

        var source = new VoltageSource("V1", 0, 0, 5.0);
        var r1 = new Resistor("R1", 0, 0, 1000);
        var r2 = new Resistor("R2", 0, 0, 1000);

        circuit.AddElement(source, vPlus, gnd);
        circuit.AddElement(r1, vPlus, mid);
        circuit.AddElement(r2, mid, gnd);

        circuit.Solve();

        Assert.That(circuit.GetVoltage(mid), Is.EqualTo(2.5).Within(1e-6));
    }

    [Test]
    public void FloatingNode_ThrowsException()
    {
        var circuit = new Circuit();
        var gnd = circuit.AddNode("GND", isGround: true);
        circuit.AddNode("Floating");

        var resistor = new Resistor("R1", 0, 0, 100);
        circuit.AddElement(resistor, gnd, gnd);

        Assert.Throws<InvalidOperationException>(() => circuit.Solve());
    }
}