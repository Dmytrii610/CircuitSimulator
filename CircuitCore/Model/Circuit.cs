using CircuitCore.Model.Components.Base;
using MathGraphLib;
using MatrixLib;

namespace CircuitCore.Model
{
    public class Circuit
    {
        private readonly FastGraph<ElementNode, ICircuitElement> graph = new();
        private readonly List<(ICircuitElement Element, Node A, Node B)> elements = new();
        public IReadOnlyList<ICircuitElement> Elements => elements
            .Select(e => e.Element)
            .ToList();
        public Node AddNode(string label, bool isGround = false)
        {
            return graph.AddNode(new ElementNode(label, isGround));
        }
        public void AddElement(ICircuitElement Element, Node A, Node B)
        {
            graph.AddUndirectedEdge(A, B, Element);
            elements.Add((Element, A, B));
        }
        public void Solve(double dt = 0)
        {
            var index = BuildNodeIndexMap(out int nodeUnknowns);

            var branchElements = elements
                .Select(e => e.Element)
                .OfType<IBranchElement>()
                .ToList();
            for (int i = 0; i < branchElements.Count; i++)
            {
                branchElements[i].BranchIndex = nodeUnknowns + i;
            }

            int total = nodeUnknowns + branchElements.Count;
            var conductance = new Matrix(total, total);
            var current = new Matrix(total, 1);

            foreach (var (element, A,B) in elements)
            {
                element.NodeA = index[A];
                element.NodeB = index[B];
                element.Stamp(conductance, current, dt);
            }
            var result = conductance.Solve(current);
            foreach (var (element, _, _) in elements)
            {
                element.OnSolved(result);
            }
            foreach (var node in graph.GetNodes())
            {
                ref var data = ref graph.GetNodeData(node);
                if (data.IsGround)
                {
                    data.Voltage = 0;
                    continue;
                }
                int idx = index[node];
                data.Voltage = result[idx, 0];
            }
        }
        public Dictionary<Node, int> BuildNodeIndexMap(out int unknownCount)
        {
            var map = new Dictionary<Node, int>();
            int next = 0;
            foreach (var node in graph.GetNodes())
            {
                var data = graph.GetNodeData(node);
                if (data.IsGround)
                {
                    map[node] = NodeIndex.Ground;
                }
                else
                {
                    next++;
                }
            }
            unknownCount = next;
            return map;
        }
        public double GetVoltage(Node node) => graph.GetNodeData(node).Voltage;
    }
}
