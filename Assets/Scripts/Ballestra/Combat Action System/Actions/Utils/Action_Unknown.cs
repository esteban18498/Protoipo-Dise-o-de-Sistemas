using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Unknown : ICombatAction // concrect implementantion of attack action
{
    public int staminaCost => 0;

    public Combat_Action_Type actionType => Combat_Action_Type.Utils;

    public ListKey<Combat_Action_mod> mods => new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>());

    NovaCharacterController recieverCharacter;

    public void Execute()
    {
        Debug.Log($"{recieverCharacter.name} Don't know what to do.");
    }

    public Action_Unknown(NovaCharacterController character)
    {
        recieverCharacter = character;
    }

    public ICombatAction createActionInstance(NovaCharacterController character)
    {
        return new Action_Attack(character);
    }
        public void Interrupt()
    {
        // Interrupt logic for super attack
    }
}
