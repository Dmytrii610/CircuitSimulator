namespace MathGraphLib
{
    /// <summary>
    /// Structure which represents a Node in Graph Data Structure
    /// </summary>
    public readonly struct Node
    {
        internal readonly int Id;
        internal Node(int id) => Id = id;
        public override bool Equals(object obj) => obj is Node n && n.Id == Id;
        public override int GetHashCode() => Id;
    }
    /// <summary>
    /// Structure which represents a Enumerator for FastGraph class
    /// </summary>
    public struct OutboundedEdgeEnumerator
    {
        private readonly int[] next;
        private int currentEdgeId;
        private int nextEdgeId;

        public OutboundedEdgeEnumerator(int startEdge, int[] next)
        {
            this.next = next;
            currentEdgeId = -1;
            nextEdgeId = startEdge;
        }
        public int Current => currentEdgeId;
        public bool MoveNext()
        {
            if (nextEdgeId == -1) return false;
            currentEdgeId = nextEdgeId;
            nextEdgeId = next[currentEdgeId];
            return true;
        }


    }
    public struct NodeEnumerator
    {
        private readonly int nodeCount;
        private int current;

        internal NodeEnumerator(int nodeCount)
        {
            this.nodeCount = nodeCount;
            current = -1;
        }
        public Node Current => new Node(current);
        public bool MoveNext()
        {
            current++;
            return current < nodeCount;
        }
        public NodeEnumerator GetEnumerator() => this;
    }
    public class FastGraph<TNodeData,TEdgeData>
    {
        private TNodeData[] nodes;
        private TEdgeData[] edgeData;
        private int[] destination;
        private int[] head; 
        private int[] next; 
        private int nodeCount;
        private int edgeCount;

        public int NodeCount => nodeCount;
        public int EdgeCount => edgeCount;
        public FastGraph()
        {
            nodes = new TNodeData[16];
            edgeData = new TEdgeData[32];
            destination = new int[32];
            head = new int[16];
            next = new int[32];
            Array.Fill(head, -1);
        }
        public Node AddNode(TNodeData node)
        {
            if(nodeCount >= nodes.Length)
            {
                int size = nodes.Length;
                Array.Resize(ref nodes, size * 2);
                Array.Resize(ref head, size * 2);
                Array.Fill(head, -1, nodeCount, head.Length - nodeCount);
            }
            nodes[nodeCount] = node;
            return new Node(nodeCount++);
        }
        public void AddDirectedEdge(Node source, Node target, TEdgeData data)
        {
            if (edgeCount >= destination.Length)
            {
                Array.Resize(ref destination, destination.Length * 2);
                Array.Resize(ref edgeData, edgeData.Length * 2);
                Array.Resize(ref next, next.Length * 2);
            }
            destination[edgeCount] = target.Id;
            edgeData[edgeCount] = data;
            next[edgeCount] = head[source.Id];
            head[source.Id] = edgeCount;
            edgeCount++;
        }
        public void AddUndirectedEdge(Node nodeA, Node nodeB, TEdgeData data)
        {
            if(edgeCount + 1 >= destination.Length)
            {
                int newSize = destination.Length * 2;
                Array.Resize(ref destination, newSize);
                Array.Resize(ref edgeData, newSize);
                Array.Resize(ref next, newSize);
            }
            int edgeId1 = edgeCount++;
            destination[edgeId1] = nodeB.Id;
            edgeData[edgeId1] = data;
            next[edgeId1] = head[nodeA.Id];
            head[nodeA.Id] = edgeId1;

            int edgeId2 = edgeCount++;
            destination[edgeId2] = nodeA.Id;
            edgeData[edgeId2] = data;
            next[edgeId2] = head[nodeB.Id];
            head[nodeB.Id] = edgeId2;
        }
        public ref TNodeData GetNodeData(Node node)
        {
            return ref nodes[node.Id];
        }
        public OutboundedEdgeEnumerator GetOutboundedEdges(Node node)
        {
            int startId = head[node.Id];
            return new OutboundedEdgeEnumerator(startId, next);
        }
        public Node GetEdgeTarget(int edgeId)
        {
            return new Node(destination[edgeId]);
        }
        public ref TEdgeData GetEdgeData(int edgeId) 
        {
            return ref edgeData[edgeId];
        }
        public Node GetNodeByIndex(int index)
        {
            if (index < 0 || index >= nodeCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return new Node(index);
        }
        public NodeEnumerator GetNodes() => new NodeEnumerator(nodeCount);
    }
}
