using CircuitCore.Model;
using CircuitCore.Model.Components;
using MatrixLib;
using System;
using System.Collections.Generic;
using System.Text;

[TestFixture]
public class CapasitorTests
{
    [Test]
    public void Stamp_FirstStep_UsesZeroPreviousVoltage()
    {
        var conductance = new Matrix(2, 2);
        var current = new Matrix(2, 1);
        var capasitor = new Capasitor("C1", 0, 1, 0.0001);
        double dt = 0.01;
        double expectedGeq = 0.0001 / dt;

        capasitor.Stamp(conductance, current, dt);

        Assert.That(conductance[0, 0], Is.EqualTo(expectedGeq).Within(1e-9));
        Assert.That(conductance[1, 1], Is.EqualTo(expectedGeq).Within(1e-9));
        Assert.That(conductance[0, 1], Is.EqualTo(0).Within(1e-9));
        Assert.That(conductance[1, 0], Is.EqualTo(0).Within(1e-9));
    }
    [Test]
    public void OnSolved_UpdatesPreviousVoltageFromResult()
    {
        var capacitor = new Capasitor("C1", nodeA: 0, nodeB: NodeIndex.Ground, capacity: 0.0001);
        var result = new Matrix(1, 1);
        result[0, 0] = 3.0;

        capacitor.OnSolved(result);

        var conductance = new Matrix(1, 1);
        var current = new Matrix(1, 1);
        double dt = 0.01;

        capacitor.Stamp(conductance, current, dt);

        double expectedGeq = 0.0001 / dt;
        double expectedIeq = expectedGeq * 3.0;
        Assert.That(current[0, 0], Is.EqualTo(expectedIeq).Within(1e-9));
    }
}