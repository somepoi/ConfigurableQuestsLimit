using System.Collections;
using System.Collections.Generic;

namespace cakeslice;

public class LinkedSet<T> : IEnumerable<T>, IEnumerable
{
	private LinkedList<T> list;

	private Dictionary<T, LinkedListNode<T>> dictionary;

	public int Count => list.Count;

	public LinkedSet()
	{
		list = new LinkedList<T>();
		dictionary = new Dictionary<T, LinkedListNode<T>>();
	}

	public LinkedSet(IEqualityComparer<T> comparer)
	{
		list = new LinkedList<T>();
		dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
	}

	public bool Add(T t)
	{
		if (dictionary.ContainsKey(t))
		{
			return false;
		}
		LinkedListNode<T> value = list.AddLast(t);
		dictionary.Add(t, value);
		return true;
	}

	public bool Remove(T t)
	{
		if (dictionary.TryGetValue(t, out var value))
		{
			dictionary.Remove(t);
			list.Remove(value);
			return true;
		}
		return false;
	}

	public void Clear()
	{
		list.Clear();
		dictionary.Clear();
	}

	public bool Contains(T t)
	{
		return dictionary.ContainsKey(t);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return list.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return list.GetEnumerator();
	}
}
