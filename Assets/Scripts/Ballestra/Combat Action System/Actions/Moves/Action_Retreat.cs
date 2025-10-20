using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Action_Retreat : ICombatAction // concrect implementantion of action to advance forward one spot
{
    public int staminaCost => 10;

    public Combat_Action_Type actionType => Combat_Action_Type.Move;

    public ListKey<Combat_Action_mod> mods => new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>()
    {

    });

    NovaCharacterController recieverCharacter;

    public void Execute()
    {
        // foward spot based on current orientation by transform scale
        if (recieverCharacter.transform.localScale.x > 0) // facing right
        {
            recieverCharacter.MoveToPreviousSpot();
        }
        else // facing left
        {
            recieverCharacter.MoveToNextSpot();            
        }

        //Debug.Log($"{recieverCharacter.name} moved to {recieverCharacter.CurrentSpot.name}");

    }

    public Action_Retreat(NovaCharacterController character)
    {
        recieverCharacter = character;
    }

    public ICombatAction createActionInstance(NovaCharacterController character)
    {
        return new Action_Advance(character);
    }

        public void Interrupt()
    {
        // Interrupt logic for super attack
    }
}