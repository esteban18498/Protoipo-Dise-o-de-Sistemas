// InputTokens.cs
using UnityEngine;

public enum DirKey { None, Up, Right, Down, Left }
public enum ActKey { None, A, B, C, D } // teclado: J,K,L,I  / gamepad: X,A,B,Y

[CreateAssetMenu(menuName = "UI/Input Icon Set")]
public class InputIconSet : ScriptableObject
{
    public Sprite up;
    public Sprite right;
    public Sprite down;
    public Sprite left;

    public Sprite actA;
    public Sprite actB;
    public Sprite actC;
    public Sprite actD;

    public Sprite emptySlot;

    public Sprite Get(DirKey k) => k switch
    {
        DirKey.Up => up,
        DirKey.Right => right,
        DirKey.Down => down,
        DirKey.Left => left,
        _ => emptySlot
    };

    public Sprite Get(ActKey k) => k switch
    {
        ActKey.A => actA,
        ActKey.B => actB,
        ActKey.C => actC,
        ActKey.D => actD,
        _ => emptySlot
    };
}
