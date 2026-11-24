using System;
using System.Collections.Generic;

public class IndexedPriorityQueue<Priority>
	where Priority : IEquatable<Priority>
{
	private readonly SparseSet<Priority> m_Priorities = new();
	private readonly SparseSet<int> m_HeapPositions = new();
	private readonly List<int> m_Heap = new();
	private readonly IComparer<Priority> m_Comparer;
	public int Count { get; private set; } = 0;
	public bool IsEmpty => Count == 0;
	public IndexedPriorityQueue(IComparer<Priority> comparer = null)
	{
		m_Comparer = comparer ?? Comparer<Priority>.Default;  //the default Comparer<T> turns the binary heap into a min binary heap
	}

	public Priority this[int idx]
	{
		get => m_Priorities[idx];
		set
		{
			if (value is null) return;
			if (!m_Priorities.Contains(idx)) Insert(idx, value);
			else
			{
				var cmp = m_Comparer.Compare(m_Priorities[idx], value);
				if (cmp > 0) IncreasePriority(idx, value);
				else if (cmp < 0) DecreasePriority(idx, value);
			}
		}
	}

	public int Dequeue()
	{
		int topIdx = m_Heap[0];
		m_Priorities.Delete(topIdx);
		m_HeapPositions.Delete(topIdx);
		Exchange(0, --Count);
		m_Heap.RemoveAt(Count);
		Sink(0);
		return topIdx;
	}

	public bool TryDeque(out int idx)
	{
		idx = -1;
		if (Count == 0) return false;
		idx = Dequeue();
		return true;
	}

	public void Insert(int idx, Priority key)
	{
		if (m_Priorities.Contains(idx)) return;
		m_Priorities[idx] = key;
		m_Heap.Add(idx);
		m_HeapPositions[idx] = Count;
		Count++;
		Swim(Count - 1);
	}
	public void IncreasePriority(int idx, Priority newPriority)
	{
		if (!m_Priorities.Contains(idx)) return;
		m_Priorities[idx] = newPriority;
		Swim(m_HeapPositions[idx]);
	}
	public void DecreasePriority(int idx, Priority newPriority)
	{
		if (!m_Priorities.Contains(idx)) return;
		m_Priorities[idx] = newPriority;
		Sink(m_HeapPositions[idx]);
	}

	private bool HasHigherPriority(int a, int b) => m_Comparer.Compare(m_Priorities[m_Heap[a]], m_Priorities[m_Heap[b]]) < 0;
	private void Exchange(int i, int j)
	{
		(m_Heap[i], m_Heap[j]) = (m_Heap[j], m_Heap[i]);
		m_HeapPositions[m_Heap[i]] = i;
		m_HeapPositions[m_Heap[j]] = j;
	}

	private void Swim(int k)
	{
		while (k > 0)
		{
			int parent = (k - 1) / 2;
			if (!HasHigherPriority(k, parent)) break;
			Exchange(k, parent);
			k = parent;
		}
	}
	private void Sink(int k)
	{
		while (2 * k + 1 < Count)
		{
			int j = 2 * k + 1;
			if (j + 1 < Count && HasHigherPriority(j + 1, j)) j++;
			if (!HasHigherPriority(k, j)) break;
			Exchange(k, j);
			k = j;
		}
	}
}
