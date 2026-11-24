using System;
using System.Collections.Generic;
using System.Linq;

public interface IWeightedDigraph
{
	int V { get; }
	int E { get; }
	IEnumerable<Edge> AdjacentEdges(int v);
}
public class WeightedDigraph<T> : IWeightedDigraph
	where T : IEquatable<T>
{
	public WeightedDigraph(int capacity)
	{
		m_VertexLookup = new(capacity);
		m_VertexData = new(capacity);
		m_Adjacency = new(capacity);
	}

	private readonly List<Bag<Edge>> m_Adjacency;
	private readonly List<T> m_VertexData;
	private readonly Dictionary<T, int> m_VertexLookup;

	public int V => m_Adjacency.Count;
	public int E => m_Adjacency.Sum(adj => adj.Count);

	public void AddVertex(T v)
	{
		if (!m_VertexLookup.TryGetValue(v, out _))
		{
			m_VertexLookup[v] = V;
			m_VertexData.Add(v);
			m_Adjacency.Add(new());
		}
	}

	private void AddEdge(int v, int w, double weight) => m_Adjacency[v].Add(new(v, w, weight));
	public void AddEdge(T v, T w, double weight)
	{
		if (m_VertexLookup.TryGetValue(v, out int vIdx) &&
				m_VertexLookup.TryGetValue(w, out int wIdx))
			AddEdge(vIdx, wIdx, weight);
	}

	public IEnumerable<Edge> AdjacentEdges(int v)
	{
		if (v > m_Adjacency.Count || v < 0)
			return Enumerable.Empty<Edge>();
		return m_Adjacency[v];
	}

	public int GetVertex(T vertexData) => m_VertexLookup[vertexData];
	public T[] VertexData => m_VertexData.ToArray();
}


public readonly struct Edge : IEquatable<Edge>
{
	public Edge(int v, int w, double weight)
	{
		V = v;
		W = w;
		Weight = weight;
	}
	public readonly int V;
	public readonly int W;
	public readonly double Weight;

	public static Edge NullEdge => new(null);
	private Edge(object _)
	{
		V = -1;
		W = -1;
		Weight = -1;
	}
	public bool IsNull => V == -1 && W == -1;

	public readonly bool Equals(Edge other)
	{
		if (IsNull && other.IsNull) return true;
		if (IsNull || other.IsNull) return false;
		return V == other.V && W == other.W;
	}
	public override readonly int GetHashCode() => !IsNull ? HashCode.Combine(V, W) : HashCode.Combine(V, W, IsNull);

	public class WeightComparer : IComparer<Edge>
	{
		public int Compare(Edge x, Edge y)
		{
			if (x.IsNull && y.IsNull) return 0;
			if (x.IsNull) return -1;
			if (y.IsNull) return 1;
			return x.Weight.CompareTo(y.Weight);
		}
	}
}
