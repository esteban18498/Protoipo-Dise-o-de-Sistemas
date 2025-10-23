using System.Collections.Generic;
using System;

public interface IReadonlyInputBuffer
{
    IReadOnlyList<DirKey> Directions { get; }   // len 0..3
    ActKey Action { get; }                      // None o A/B/C/D
    event Action OnChanged;
}