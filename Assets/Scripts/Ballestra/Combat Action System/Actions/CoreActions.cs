using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Action_Step : ICombatAction // concrect implementantion of void action to do nothing
{
    public int staminaCost => 0;

    public Combat_Action_Type actionType => Combat_Action_Type.Utils;

    public List<Combat_Action_mod> mods => new List<Combat_Action_mod>();

    NovaCharacterController recieverCharacter;

    public void Execute()
    {
        // Do nothing
        Debug.Log($"{recieverCharacter.name} Executing Step Action: Doing nothing.");
    }

    public Action_Step(NovaCharacterController character)
    {
        recieverCharacter = character;
    }
    
}

public class Action_Advance : ICombatAction // concrect implementantion of action to advance forward one spot
{
    public int staminaCost => 0;

    public Combat_Action_Type actionType => Combat_Action_Type.Move;

    public List<Combat_Action_mod> mods => new List<Combat_Action_mod>();

    NovaCharacterController recieverCharacter;

    public void Execute()
    {
        // foward spot based on current orientation by transform scale
        if (recieverCharacter.transform.localScale.x > 0) // facing right
        {
            recieverCharacter.MoveToNextSpot();
        }
        else // facing left
        {
            recieverCharacter.MoveToPreviousSpot();
        }

        Debug.Log($"{recieverCharacter.name} moved to {recieverCharacter.CurrentSpot.name}");

    }

    public Action_Advance(NovaCharacterController character)
    {
        recieverCharacter = character;
    }
    
}