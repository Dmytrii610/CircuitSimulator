using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CircuitCore.Model;
using CircuitCore.Model.Components;
namespace CircuitSimulator.Tests
{
    [TestFixture]
    public class AmmeterTests
    {
        [Test]
        public void Ammeter_MeasuresCorrectCurrent()
        {
            var circuit = new Circuit();
            var gnd = circuit.AddNode("GND", true);
            var mid1 = circuit.AddNode("Mid_1");
            var mid2 = circuit.AddNode("Mid_2");

            var vSource = new VoltageSource("E1", 0, 0, 0.5);
            var resistor = new Resistor("R1", 0, 0, 0.1);
            var ammeter = new Ammeter("A1", 0, 0);

            circuit.AddElement(vSource, gnd, mid1);
            circuit.AddElement(ammeter, mid1, mid2);
            circuit.AddElement(resistor, mid2, gnd);

            circuit.Solve();

            Assert.That(ammeter.MeasuredValue, Is.EqualTo(-5).Within(1e-9));
        }
        [Test]
        public void Ammeter_MeasuresCorrectCurrent_UsingCurrentSource()
        {
            var circuit = new Circuit();
            var gnd = circuit.AddNode("GND", true);
            var mid1 = circuit.AddNode("mid_1");
            var mid2 = circuit.AddNode("mid_2");

            var cSource = new CurrentSource("I1", 0, 0, 5);
            var ammeter = new Ammeter("A1", 0, 0);
            var resistor = new Resistor("R1", 0, 0, 1000);

            circuit.AddElement(cSource, gnd, mid1);
            circuit.AddElement(ammeter, mid1, mid2);
            circuit.AddElement(resistor, mid2, gnd);

            circuit.Solve();
            Assert.That(ammeter.MeasuredValue, Is.EqualTo(5).Within(1e-9));
        }
    }
}
