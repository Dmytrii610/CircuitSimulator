using NUnit.Framework;
using CircuitCore.Model.Components;
using MatrixLib;

namespace CircuitSimulator.Tests
{
    [TestFixture]
    public class VoltageSourceTest
    {
        [Test]
        public void Stamp_WritesCorrectBranchEquations()
        {
            var conductance = new Matrix(3, 3);
            var current = new Matrix(3, 1);
            var vSource = new VoltageSource("V1", 0, 1, 5.0);
            vSource.BranchIndex = 2;

            vSource.Stamp(conductance, current, 0);
            Assert.That(conductance[0, 2], Is.EqualTo(1).Within(1e-9));
            Assert.That(conductance[2, 0], Is.EqualTo(1).Within(1e-9));
            Assert.That(conductance[1, 2], Is.EqualTo(-1).Within(1e-9));
            Assert.That(conductance[2, 1], Is.EqualTo(-1).Within(1e-9));
            Assert.That(current[2, 0], Is.EqualTo(5.0).Within(1e-9));

        }
    }
}
