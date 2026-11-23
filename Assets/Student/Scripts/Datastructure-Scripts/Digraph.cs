using System;
using System.Collections.Generic;
using System.Linq;

public interface IWeightedDigraph
{
	int V { get; }
	int E { get; }
	IEnumerable<Edge> AdjacentEdges(int v);
	void AddEdge(int v, int w, double weight);
}
public class WeightedDigraph<T> : IWeightedDigraph
	where T : IEquatable<T>
{
	public WeightedDigraph(IEnumerable<T> vertecies)
	{
		m_VertexData = vertecies.ToArray();
		m_Adjacency = new Bag<Edge>[vertecies.Count()];
		Array.Fill(m_Adjacency, new());
	}

	private readonly Bag<Edge>[] m_Adjacency;
	private readonly T[] m_VertexData;

	public int V => m_Adjacency.Length;
	public int E => (from adj in m_Adjacency select adj.Count).Count();
	public void AddEdge(int v, int w, double weight)
	{
		var edge = new Edge(v, w, weight);
		if (!m_Adjacency[v].Contains(edge))
			m_Adjacency[v].Add(edge);
	}
	public void AddEdge(T v, T w, double weight)
	{
		int vIdx = GetVertex(v);
		int wIdx = GetVertex(w);
		if (vIdx == -1 || wIdx == -1) return;
		AddEdge(vIdx, wIdx, weight);
	}

	public int GetVertex(T vertexData)
	{
		return Array.IndexOf(m_VertexData, vertexData);
	}


	public T[] VertexData => m_VertexData;
	public IEnumerable<Edge> AdjacentEdges(int v)
	{
		if (v > m_Adjacency.Length || v < 0)
			return Enumerable.Empty<Edge>();
		return m_Adjacency[v];
	}
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
