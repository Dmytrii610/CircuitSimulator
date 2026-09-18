using CircuitCore.Model;
using CircuitCore.Model.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator.Tests
{
    [TestFixture]
    public class InductorTests
    {
        [Test]
        public void ConstantVoltage_CurrentIncreasesLinearly()
        {
            var circuit = new Circuit();
            var ground = circuit.AddNode("GND", true);
            var mid = circuit.AddNode("Mid");

            var source = new VoltageSource("V1", 0, 0, 1.0);
            var inductor = new Inductor("L1",0, 0, 1.0);
            circuit.AddElement(source, ground, mid);
            circuit.AddElement(inductor, ground, mid);

            var expected = new double[] { 0.01, 0.02, 0.03, 0.04 };
            for (int step = 0; step < expected.Length; step++)
            {
                circuit.Solve(0.01);
                Assert.That(inductor.previousCurrent, Is.EqualTo(expected[step]).Within(1e-10),$"Incorrect current value in {step + 1}: get {inductor.previousCurrent}, but expected {expected[step]}");
            }
        }
        [Test]
        public void VoltageDifferenceBetweenNonGroundNodes_CurrentIncreasesLinearly()
        {
            var circuit = new Circuit();
            var ground = circuit.AddNode("GND", true);
            var nA = circuit.AddNode("nodeA");
            var nB = circuit.AddNode("nodeB");

            var V1 = new VoltageSource("V1", 0, 0, 3.0);
            var V2 = new VoltageSource("V2", 0, 0, 2.0);
            var inductor = new Inductor("L1", 0, 0, 1);
            circuit.AddElement(V1, nA, ground);
            circuit.AddElement(V2, nB, ground);
            circuit.AddElement(inductor, nA, nB);

            var expected = new double[] { 0.01, 0.02, 0.03, 0.04 };
            for (int step = 0; step < expected.Length; step++)
            {
                circuit.Solve(0.01);
                Assert.That(inductor.previousCurrent, Is.EqualTo(expected[step]).Within(1e-10), $"Incorrect current value in {step + 1}: get {inductor.previousCurrent}, but expected {expected[step]}");
            }

        }
        [Test]
        public void RLCircuit_CurrentOnInductorIncreasesLinearly()
        {
            var circuit = new Circuit();
            var ground = circuit.AddNode("GND", true);
            var mid1 = circuit.AddNode("mid1");
            var mid2 = circuit.AddNode("mid2");

            var V1 = new VoltageSource("V1", 0, 0, 1.0);
            var resistor = new Resistor("R1", 0, 0, 1.0);
            var inductor = new Inductor("L1", 0, 0, 1.0);
            circuit.AddElement(V1, mid1, ground);
            circuit.AddElement(resistor, mid1, mid2);
            circuit.AddElement(inductor, mid2, ground);

            var expected = new double[]
            {
                0.0909090909090909,
                0.1735537190082645,
                0.2486851990984222,
                0.3169865446349293
            };
            for (int step = 0; step < expected.Length; step++)
            {
                circuit.Solve(0.1);
                Assert.That(inductor.previousCurrent, Is.EqualTo(expected[step]).Within(1e-10), $"Incorrect current value in {step + 1}: get {inductor.previousCurrent}, but expected {expected[step]}");
            }
        }
    }
}
