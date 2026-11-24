using System.Linq;

public class Dijkstras
{
	private readonly IndexedPriorityQueue<double> m_PriorityQueue;
	private readonly Edge[] m_EdgeTo;
	private readonly double[] m_DistTo;
	public Edge[] Paths => m_EdgeTo;

	public Dijkstras(IWeightedDigraph graph, int startVertex)
	{
		m_EdgeTo = new Edge[graph.V];
		m_DistTo = new double[graph.V];
		m_PriorityQueue = new();

		foreach (int v in Enumerable.Range(0, graph.V))
		{
			m_DistTo[v] = double.PositiveInfinity;
			m_EdgeTo[v] = Edge.NullEdge;
		}

		m_DistTo[startVertex] = 0;
		m_PriorityQueue[startVertex] = 0;

		while (m_PriorityQueue.TryDeque(out int v))
			foreach (Edge e in graph.AdjacentEdges(v))
				Relax(e);
	}

	private void Relax(Edge e)
	{
		if (m_DistTo[e.W] > m_DistTo[e.V] + e.Weight)
		{
			m_DistTo[e.W] = m_DistTo[e.V] + e.Weight;
			m_EdgeTo[e.W] = e;
			m_PriorityQueue[e.W] = m_DistTo[e.W];
		}
	}
}
