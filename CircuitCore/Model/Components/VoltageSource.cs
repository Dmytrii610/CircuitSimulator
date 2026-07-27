using CircuitCore.Model.Components.Base;
using MatrixLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace CircuitCore.Model.Components
{
    public class VoltageSource : BranchCircuitElement, ISource
    {
        
        public double Voltage { get; set; }
        double ISource.Value => Voltage;

        public VoltageSource(string name, int nodeA, int nodeB, double voltage)
            :base(name,nodeA,nodeB)
        {
            Voltage = voltage;
        }
        public override void Stamp(Matrix conductance, Matrix currentVector, double dt)
        {
            
            if (!NodeAIsGround)
            {
                conductance[NodeA, BranchIndex] += 1;
                conductance[BranchIndex, NodeA] += 1;
            }
            if (!NodeBIsGround)
            {
                conductance[NodeB, BranchIndex] -= 1;
                conductance[BranchIndex, NodeB] -= 1;
            }
            currentVector[BranchIndex, 0] += Voltage;
        }
    }
}
