using System;
using System.Collections.Generic;

public class BreadthFirstSearch
{
	readonly Queue<int> m_Queue;
	readonly bool[] m_Marked;
	readonly int[] m_EdgeTo;
	public BreadthFirstSearch(IWeightedDigraph graph, int startVertex)
	{
		m_Queue = new();
		m_Marked = new bool[graph.V];
		m_EdgeTo = new int[graph.V];

		Array.Fill(m_EdgeTo, -1);

		m_Queue.Enqueue(startVertex);
		while (m_Queue.TryDequeue(out int v))
		{
			foreach (var e in graph.AdjacentEdges(v))
				if (!m_Marked[e.W])
				{
					m_Queue.Enqueue(e.W);
					m_Marked[e.W] = true;
					m_EdgeTo[e.W] = e.V;
				}
		}
	}
	public int[] Paths => m_EdgeTo;

}
