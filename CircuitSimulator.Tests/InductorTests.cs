using CircuitCore.Math;
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
        [Test]
        public void RLCircuit_SmallerTimeStep_ReducesErrorAtOneSecond()
        {
            var circuitTest1 = new Circuit();
            var gnd = circuitTest1.AddNode("GND", true);
            var mid1 = circuitTest1.AddNode("mid1");
            var mid2 = circuitTest1.AddNode("mid2");

            var source1 = new VoltageSource("V1", 0, 0, 1.0);
            var resistor1 = new Resistor("R1", 0, 0, 1.0);
            var inductor1 = new Inductor("L1", 0, 0, 1.0);
            circuitTest1.AddElement(source1, mid1, gnd);
            circuitTest1.AddElement(resistor1, mid1, mid2);
            circuitTest1.AddElement(inductor1, mid2, gnd);

            var circuitTest2 = new Circuit();
            var gndT = circuitTest2.AddNode("GND", true);
            var mid1T = circuitTest2.AddNode("mid1");
            var mid2T = circuitTest2.AddNode("mid2");

            var source2 = new VoltageSource("V1", 0, 0, 1.0);
            var resistor2 = new Resistor("R1", 0, 0, 1.0);
            var inductor2 = new Inductor("L1", 0, 0, 1.0);
            circuitTest2.AddElement(source2, mid1T, gndT);
            circuitTest2.AddElement(resistor2, mid1T, mid2T);
            circuitTest2.AddElement(inductor2, mid2T, gndT);

            for (int step = 0; step < 10; step++)
            {
                circuitTest1.Solve(0.1);
            }
            for (int step = 0; step < 100; step++)
            {
                circuitTest2.Solve(0.01);
            }

            var (coarseCurrent, fineCurrent) = (inductor1.previousCurrent, inductor2.previousCurrent);
            var exact = 1.0 - Math.Exp(-1.0);
            var coarseError = Math.Abs(coarseCurrent - exact);
            var fineError = Math.Abs(fineCurrent - exact);

            Assert.That(fineError, Is.LessThan(coarseError));
            Assert.That(fineError, Is.LessThan(0.002));
        }
        [Test]
        public void RLCircuit_AfterSourceVoltageBecomesZero_CurrentDecays()
        {
            var circuit = new Circuit();
            var gnd = circuit.AddNode("GND", true);
            var mid1 = circuit.AddNode("mid1");
            var mid2 = circuit.AddNode("mid2");

            var source = new VoltageSource("V1", 0, 0, 1.0);
            var resistor = new Resistor("R1", 0, 0, 1.0);
            var inductor = new Inductor("L1", 0, 0, 1.0);
            circuit.AddElement(source, mid1, gnd);
            circuit.AddElement(resistor, mid1, mid2);
            circuit.AddElement(inductor, mid2, gnd);
            for (int step = 0; step < 10; step++)
            {
                circuit.Solve(0.1);
            }
            var currentAfterSimulation = inductor.previousCurrent;
            Assert.That(currentAfterSimulation, Is.GreaterThan(0));

            source.Voltage = 0.0;
            var previousCurrent = currentAfterSimulation;
            for (int step = 0; step < 10; step++)
            {
                circuit.Solve(0.1);
                var current = inductor.previousCurrent;

                Assert.That(current, Is.GreaterThan(0));
                Assert.That(current, Is.LessThan(previousCurrent));
                previousCurrent = current;
            }
            double expected = currentAfterSimulation * Math.Pow(1.0 / 1.1, 10);

            Assert.That(inductor.previousCurrent,Is.EqualTo(expected).Within(1e-10));
        }
        [Test]
        public void RLCircuit_Reset_RepeatsTransientFromInitialState()
        {
            var circuit = new Circuit();
            var gnd = circuit.AddNode("GND", true);
            var mid1 = circuit.AddNode("mid1");
            var mid2 = circuit.AddNode("mid2");

            var source = new VoltageSource("V1", 0, 0, 1.0);
            var resistor = new Resistor("R1", 0, 0, 1.0);
            var inductor = new Inductor("L1", 0, 0, 1.0);
            circuit.AddElement(source, mid1, gnd);
            circuit.AddElement(resistor, mid1, mid2);
            circuit.AddElement(inductor, mid2, gnd);

            var solver = new CircuitSolver(circuit, 0.1);
            for (int step = 0; step < 10; step++)
            {
                solver.Step();
            }
            var firstRun = inductor.previousCurrent;
            Assert.That(firstRun, Is.GreaterThan(0));

            solver.Reset();
            
            Assert.That(solver.CurrentTime, Is.EqualTo(0));
            Assert.That(inductor.previousCurrent, Is.EqualTo(0));
            for (int step = 0; step < 10; step++)
            {
                solver.Step();
            }
            Assert.That(inductor.previousCurrent, Is.EqualTo(firstRun).Within(1e-10));
            Assert.That(solver.CurrentTime, Is.EqualTo(1.0).Within(1e-10));

        }
    }
}
