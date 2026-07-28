using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CircuitCore.Model.Components;
using CircuitCore.Model;
namespace CircuitSimulator.Tests
{
    [TestFixture]
    public class OhmmeterTests
    {
        [Test]
        public void Ohmmeter_MeasuresCorrectResistance()
        {
            var circuit = new Circuit();
            var gnd = circuit.AddNode("GND", true);
            var node = circuit.AddNode("mid_1");

            var resistor = new Resistor("R1",0,0,0.5);
            var ohmmeter = new Ohmmeter("Ohm1", 0, 0);

            circuit.AddElement(ohmmeter, gnd, node);
            circuit.AddElement(resistor, node, gnd);

            circuit.Solve();

            Assert.That(ohmmeter.MeasuredValue, Is.EqualTo(-resistor.Resistance).Within(1e-9));
        }
    }
}
