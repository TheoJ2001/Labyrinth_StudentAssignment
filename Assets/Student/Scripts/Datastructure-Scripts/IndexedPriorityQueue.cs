using System;
using System.Collections.Generic;

public class IndexedPriorityQueue<T>
	where T : IEquatable<T>
{
	private readonly SparseSet<T> m_Keys = new();
	private readonly List<int> m_Heap = new();
	private readonly IComparer<T> m_Comparer;
	public int Count { get; private set; } = 0;
	public IndexedPriorityQueue(IComparer<T> comparer = null)
	{
		m_Comparer = comparer ?? Comparer<T>.Default;  //the default Comparer<T> turns the binary heap into a min binary heap
	}

	public T this[int idx]
	{
		get => m_Keys[idx];
		set
		{
			if (value is null) return;
			if (!m_Keys.Contains(idx)) Insert(idx, value);
			else
			{
				var cmp = m_Comparer.Compare(value, m_Keys[idx]);
				if (cmp < 0) DecreaseKey(idx, value);
				else if (cmp > 0) IncreaseKey(idx, value);
			}
		}
	}

	public KeyValuePair<int, T> Dequeue()
	{
		int topIdx = m_Heap[0];
		T topKey = m_Keys[topIdx];
		m_Keys.Delete(topIdx);
		Exchange(0, --Count);
		m_Heap.RemoveAt(Count);
		Sink(0);
		return new(topIdx, topKey);
	}

	public bool TryDeque(out KeyValuePair<int, T> key)
	{
		key = default;
		if (Count == 0) return false;
		key = Dequeue();
		return true;
	}
	public bool IsEmpty => m_Keys.Count == 0;
	public void Insert(int idx, T key)
	{
		if (m_Keys.Contains(idx)) return;
		Count++;
		m_Keys[idx] = key;
		m_Heap.Add(idx);
		Swim(m_Heap.Count - 1);
	}
	public void DecreaseKey(int idx, T newKey)
	{
		if (!m_Keys.Contains(idx)) return;
		m_Keys[idx] = newKey;
		Sink(idx);
	}
	public void IncreaseKey(int idx, T newKey)
	{
		if (!m_Keys.Contains(idx)) return;
		m_Keys[idx] = newKey;
		Swim(idx);
	}
	private bool HasHigherPriority(int a, int b) => m_Comparer.Compare(m_Keys[m_Heap[a]], m_Keys[m_Heap[b]]) < 0;
	private void Exchange(int i, int j)
	{
		(m_Heap[i], m_Heap[j]) = (m_Heap[j], m_Heap[i]);
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
		int Count = m_Heap.Count;
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
