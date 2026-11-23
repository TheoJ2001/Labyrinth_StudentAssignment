using System;
using System.Collections;
using System.Collections.Generic;

public class Bag<T> : IEnumerable<T>
	where T : IEquatable<T>
{
	private readonly List<T> m_Data = new();
	public int Count => m_Data.Count;
	public void Add(T item) => m_Data.Add(item);
	public bool Contains(T item) => m_Data.Contains(item);
	public IEnumerator<T> GetEnumerator() => m_Data.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
