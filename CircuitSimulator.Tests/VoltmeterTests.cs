using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CircuitCore.Model;
using CircuitCore.Model.Components;
using NUnit.Framework.Internal;
namespace CircuitSimulator.Tests
{
    [TestFixture]
    public class VoltmeterTests
    {
        [Test]
        public void Voltmeter_MeasuresCorrectVoltage()
        {
            var circuit = new Circuit();
            var gnd = circuit.AddNode("GND", true);
            var mid1 = circuit.AddNode("mid_1");

            var vSource = new VoltageSource("E1", 0, 0, 0.5);
            var voltmeter = new Voltmeter("V1", 0, 0);
            var resistor = new Resistor("R1", 0, 0, 0.1);

            circuit.AddElement(vSource, gnd, mid1);
            circuit.AddElement(resistor, mid1, gnd);
            circuit.AddElement(voltmeter, mid1, gnd);

            circuit.Solve();
            Assert.That(voltmeter.MeasuredValue, Is.EqualTo(-0.5).Within(1e-9));
        }
        [Test]
        public void Voltmeter_MeasuresCorrectVoltage_UsingCurrentSource()
        {
            var circuit = new Circuit();
            var gnd = circuit.AddNode("GND", true);
            var mid1 = circuit.AddNode("mid_1");

            var cSource = new CurrentSource("I1", 0, 0, 5);
            var voltmeter = new Voltmeter("V1", 0, 0);
            var resistor = new Resistor("R1", 0, 0, 1000);

            circuit.AddElement(cSource, gnd, mid1);
            circuit.AddElement(resistor, mid1, gnd);
            circuit.AddElement(voltmeter, mid1, gnd);

            circuit.Solve();
            Assert.That(voltmeter.MeasuredValue, Is.EqualTo(5000).Within(1e-9));
        }
    }
}
