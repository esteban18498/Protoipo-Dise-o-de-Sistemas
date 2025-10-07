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
