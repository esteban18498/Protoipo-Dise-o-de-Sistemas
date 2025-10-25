using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public enum Combat_Action_Type 
{
    Attack,
    Utils,
    Block,
    Move
}

public enum Combat_Action_mod // flechitas
{
    Up,
    Front,
    Down,
    Back
}


public interface ICombatAction
{
    int staminaCost { get; }
    Combat_Action_Type actionType { get; }
    public ListKey<Combat_Action_mod> mods { get; }

    ICombatAction createActionInstance(NovaCharacterController character);

    void Execute();

    void Interrupt();

}


public class ListKey<T>
{
    private readonly IList<T> _list;

    public ListKey(IList<T> list)
    {
        _list = list ?? throw new System.ArgumentNullException(nameof(list));
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ListKey<T>);
    }

    public bool Equals(ListKey<T> other)
    {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;

        // Compare list contents
        return _list.SequenceEqual(other._list);
    }

    public IEnumerator GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public override int GetHashCode()
    {
        // Combine hash codes of elements
        // A simple approach is to XOR hash codes of all elements
        // More robust methods exist for order-sensitive hashing
        int hash = 17;
        foreach (var item in _list)
        {
            hash = hash * 23 + (item?.GetHashCode() ?? 0);
        }
        return hash;
    }

    public void add(T item)
    {
        _list.Add(item);
    }

    public T dequuee()
    {
        T first = _list[0];
        _list.RemoveAt(0);
        return first;
    }

    public bool isEmpty()
    {
        return _list.Count == 0;
    }

    public override string ToString()
    {

        string text;


        text = "";
        for (int i = 0; i < _list.Count; i++)
        {
            T item = _list[i];
            text += item.ToString();
            if (i < _list.Count - 1)
            {
                text += ", ";
            }
        }
        return text;
    }

    public List<T> ToList()
    {
        return new List<T>(_list);
    }
}
