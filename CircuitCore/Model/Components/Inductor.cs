using CircuitCore.Model.Components.Base;
using MatrixLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitCore.Model.Components
{
    public class Inductor : BranchCircuitElement
    {
        public double Inductance { get; set; }
        public double previousCurrent { get; private set; }

        public Inductor(string name, int nodeA, int nodeB, double inductance)
            :base(name,nodeA,nodeB)
        {
            Inductance = inductance;
        }
        public override void Stamp(Matrix conductance, Matrix currentVector, double dt)
        {
            var k = BranchIndex;
            var coefficient = Inductance / dt;

            if (!NodeAIsGround)
            {
                conductance[NodeA, k] = 1;
                conductance[k, NodeA] = -1;
            }

            if (!NodeBIsGround)
            {
                conductance[NodeB, k] = 1;
                conductance[k, NodeB] = -1;
            }
            conductance[k, k] -= coefficient;
            currentVector[k, 0] = coefficient * previousCurrent;
        }
        public override void OnSolved(Matrix result)
        {
            previousCurrent = result[BranchIndex, 0];
        }
    }
}
