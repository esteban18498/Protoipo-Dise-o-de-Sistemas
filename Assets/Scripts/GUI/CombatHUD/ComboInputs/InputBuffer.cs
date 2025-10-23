using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class InputBuffer : IReadonlyInputBuffer
{
    private readonly List<DirKey> _dirs = new(3);
    public IReadOnlyList<DirKey> Directions => _dirs;
    public ActKey Action { get; private set; } = ActKey.None;

    public event Action OnChanged;

    public void Clear()
    {
        _dirs.Clear();
        Action = ActKey.None;
        OnChanged?.Invoke();
    }

    public void PushDirection(DirKey dir)
    {
        if (dir == DirKey.None) return;
        if (_dirs.Count == 3)
        {
            // corre a la izquierda (tipo cola)
            _dirs.RemoveAt(0);
        }
        _dirs.Add(dir);
        OnChanged?.Invoke();
    }

    public void SetAction(ActKey act)
    {
        Action = act;
        OnChanged?.Invoke();
    }

    public ComboSnapshot Snapshot() => new(_dirs, Action);
}

public readonly struct ComboSnapshot
{
    public readonly DirKey d1, d2, d3;
    public readonly ActKey act;

    public ComboSnapshot(IReadOnlyList<DirKey> dirs, ActKey action)
    {
        d1 = dirs.Count > 0 ? dirs[0] : DirKey.None;
        d2 = dirs.Count > 1 ? dirs[1] : DirKey.None;
        d3 = dirs.Count > 2 ? dirs[2] : DirKey.None;
        act = action;
    }

    public bool IsEmpty => d1 == DirKey.None && d2 == DirKey.None && d3 == DirKey.None && act == ActKey.None;
}
