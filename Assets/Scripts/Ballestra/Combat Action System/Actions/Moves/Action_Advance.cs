using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Action_Advance : ICombatAction // concrect implementantion of action to advance forward one spot
{
    public int staminaCost => 10;

    public Combat_Action_Type ActionType => Combat_Action_Type.Move;

    public ListKey<Combat_Action_mod> Mods => new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>()
    {
        Combat_Action_mod.Front,
    });

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

        //Debug.Log($"{recieverCharacter.name} moved to {recieverCharacter.CurrentSpot.name}");

    }

    public Action_Advance(NovaCharacterController character)
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