using System;
using System.Collections.Generic;
using System.Text;
using MatrixLib;
using NUnit.Framework;
using CircuitCore.Model.Components;
using CircuitCore.Model;
namespace CircuitSimulator.Tests
{
    [TestFixture]
    public class CurrentSourceTests
    {
        [Test]
        public void Stamp_WritesCorrectBranchEquations()
        {
            var conductance = new Matrix(2, 2);
            var current = new Matrix(2, 1);
            var cSource = new CurrentSource("I1", 0, 1, 1.0);

            var expectedCurrent = 1.0;
            cSource.Stamp(conductance, current, 0);
            Assert.That(current[0, 0], Is.EqualTo(-expectedCurrent).Within(1e-9));
            Assert.That(current[1, 0], Is.EqualTo(expectedCurrent).Within(1e-9));
        }
        [Test]
        public void ResistorWithCurrentSource_MatchesOhmsLaw()
        {
            var circuit = new Circuit();
            var gnd = circuit.AddNode("GND", true);
            var node = circuit.AddNode("N1");

            var source = new CurrentSource("I", 0, 1, 5.0);
            var resistor = new Resistor("R1", 0, 0, 0.1);

            circuit.AddElement(source, gnd, node);
            circuit.AddElement(resistor, node, gnd);

            circuit.Solve();

            Assert.That(circuit.GetVoltage(node), Is.EqualTo(0.5).Within(1e-9));
        }
    }
}
