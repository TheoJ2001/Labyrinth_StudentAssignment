using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class SparseSet<T> : IEnumerable<KeyValuePair<int, T>>, IEnumerable<T>
	where T : IEquatable<T>
{
	private readonly List<T> m_Dense;
	private readonly List<int[]> m_Sparse;
	private readonly List<int> m_Indecies;
	private readonly int m_PageSize;

	public SparseSet(int pageSize = 128)
	{
		Debug.Assert(pageSize >= 1);
		m_PageSize = pageSize;
		m_Dense = new();
		m_Indecies = new();
		m_Sparse = new() { CreatePage() };
	}
	public int Count { get; private set; } = 0;

	public T this[int idx]
	{
		get
		{
			if (idx < 0) return default;

			int pageIndex = idx / m_PageSize;
			int pageOffset = idx % m_PageSize;

			if (pageIndex >= m_Sparse.Count || m_Sparse[pageIndex] is null)
				return default;

			var index = m_Sparse[pageIndex][pageOffset];

			if (index == -1)
				return default;

			return m_Dense[index];
		}
		set
		{
			int pageIndex = idx / m_PageSize;
			int pageOffset = idx % m_PageSize;

			if (pageIndex >= m_Sparse.Count)
				AddPages(pageIndex - m_Sparse.Count + 1);

			m_Sparse[pageIndex] ??= CreatePage();

			int denseIdx = m_Sparse[pageIndex][pageOffset];
			if (denseIdx == -1)
			{
				Insert(idx, value);
				return;
			}
			m_Dense[denseIdx] = value;
		}
	}
	public void Delete(int idx)
	{
		int pageIndex = idx / m_PageSize;
		int pageOffset = idx % m_PageSize;

		if (pageIndex >= m_Sparse.Count || m_Sparse[pageIndex] is null)
			return;

		int delIdx = m_Sparse[pageIndex][pageOffset];
		if (delIdx == -1) return;

		m_Sparse[pageIndex][pageOffset] = -1;

		if (delIdx == Count - 1)
		{
			m_Dense.RemoveAt(delIdx);
			m_Indecies.RemoveAt(delIdx);
			Count--;
		}
		else
		{
			int lastSparseIdx = m_Indecies[Count - 1];

			m_Sparse[lastSparseIdx / m_PageSize][lastSparseIdx % m_PageSize] = delIdx;

			(m_Dense[delIdx], m_Dense[Count - 1]) = (m_Dense[Count - 1], m_Dense[delIdx]);
			(m_Indecies[delIdx], m_Indecies[Count - 1]) = (m_Indecies[Count - 1], m_Indecies[delIdx]);

			m_Dense.RemoveAt(m_Dense.Count - 1);
			m_Indecies.RemoveAt(m_Indecies.Count - 1);
			Count--;
		}
		if (Count == 0)
		{
			m_Sparse.Clear();
			m_Sparse.Add(CreatePage());
		}
		else
		{
			if (m_Sparse[pageIndex].Aggregate((a, b) => a + b) == -m_PageSize)
				m_Sparse[pageIndex] = null;
		}
	}
	public bool Contains(int idx)
	{
		int pageIndex = idx / m_PageSize;
		int pageOffset = idx % m_PageSize;

		if (pageIndex >= m_Sparse.Count || m_Sparse[pageIndex] is null)
			return false;

		return m_Sparse[pageIndex][pageOffset] != -1;
	}

	public bool Contains(T value) => m_Dense.Contains(value);
	public int IndexOf(T data)
	{
		int idx = m_Dense.IndexOf(data);
		if (idx == -1) return -1;
		return m_Indecies[idx];
	}
	private int[] CreatePage()
	{
		int[] newPage = new int[m_PageSize];
		Array.Fill(newPage, -1);
		return newPage;
	}
	private void AddPages(int count)
	{
		if (count <= 0) return;
		foreach (var _ in Enumerable.Range(0, count))
			m_Sparse.Add(null);
	}

	private void Insert(int idx, T data)
	{
		Count++;
		m_Dense.Add(data);
		m_Indecies.Add(idx);
		m_Sparse[idx / m_PageSize][idx % m_PageSize] = Count - 1;
	}
	public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
	{
		return new SparseEnumerator(m_Dense, m_Indecies);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => m_Dense.GetEnumerator();


	public class SparseEnumerator : IEnumerator<KeyValuePair<int, T>>
	{
		private int m_CurrentIdx = -1;
		private readonly List<T> m_DenseList;
		private readonly List<int> m_Indecies;
		public KeyValuePair<int, T> Current { get; private set; }
		object IEnumerator.Current => Current;

		public SparseEnumerator(List<T> denseList, List<int> indecies)
		{
			m_DenseList = denseList;
			m_Indecies = indecies;
		}
		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (++m_CurrentIdx < m_DenseList.Count)
			{
				Current = new(m_Indecies[m_CurrentIdx], m_DenseList[m_CurrentIdx]);
				return true;
			}
			else
			{
				Current = default;
				return false;
			}
		}

		public void Reset()
		{
			m_CurrentIdx = -1;
			Current = default;
		}
	}
}
