using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Step : ICombatAction // concrect implementantion of void action to do nothing
{
    public int staminaCost => 0;

    public Combat_Action_Type actionType => Combat_Action_Type.Utils;

    public ListKey<Combat_Action_mod> mods => new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>());

    NovaCharacterController recieverCharacter;

    public void Execute()
    {
        // Do nothing
        //Debug.Log($"{recieverCharacter.name} Executing Step Action: Doing nothing.");
    }

    public Action_Step(NovaCharacterController character)
    {
        recieverCharacter = character;
    }

    public ICombatAction createActionInstance(NovaCharacterController character)
    {
        return new Action_Step(character);
    }

        public void Interrupt()
    {
        // Interrupt logic for super attack
    }
    
}