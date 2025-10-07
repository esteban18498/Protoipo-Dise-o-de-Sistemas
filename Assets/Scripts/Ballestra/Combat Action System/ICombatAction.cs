using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<Combat_Action_mod> mods { get; }

    void Execute();

}
