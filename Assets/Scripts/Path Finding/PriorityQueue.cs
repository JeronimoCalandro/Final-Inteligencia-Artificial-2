using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    Dictionary<T, float> _allElems = new Dictionary<T, float>();
    public int Count { get { return _allElems.Count; } }

    public void Enqueue(T elem, float cost)
    {
        if (!_allElems.ContainsKey(elem)) _allElems.Add(elem, cost);
        else _allElems[elem] = cost;
    }

    public T Dequeue()
    {
        float lowestValue = Mathf.Infinity;
        T elem = default;

        foreach (var item in _allElems)
        {
            if (item.Value < lowestValue)
            {
                lowestValue = item.Value;
                elem = item.Key;
            }
        }
        _allElems.Remove(elem);
        return elem;
    }
}
