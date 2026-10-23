using CircuitCore.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitCore.Math
{
    public class CircuitSolver
    {
        private readonly Circuit circuit;
        public double CurrentTime { get; private set; }
        public double TimeStep { get; }

        public event Action<double> StepCompleted;

        public CircuitSolver(Circuit circuit, double timeStep)
        {
            if (!double.IsFinite(timeStep) || timeStep <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeStep), timeStep, "Time step must be positive and finite.");
            }

            this.circuit = circuit;
            TimeStep = timeStep;
        }
        public void Step()
        {
            circuit.Solve(TimeStep);
            CurrentTime += TimeStep;
            StepCompleted?.Invoke(CurrentTime);
        }
        public void Run(double duration)
        {
            int steps = (int)System.Math.Round(duration / TimeStep);
            for (int i = 0; i < steps; i++)
            {
                Step();
            }
            
        }
        public void Reset()
        {
            CurrentTime = 0;
            foreach (var element in circuit.Elements)
            {
                element.ResetState();
            }
        }
        public IEnumerable<IMeasure> GetMeasurements() => circuit.Elements.OfType<IMeasure>();
    }
}
