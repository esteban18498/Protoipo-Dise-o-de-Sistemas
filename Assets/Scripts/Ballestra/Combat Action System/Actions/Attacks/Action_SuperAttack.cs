using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Action_superAttack : ICombatAction // concrect implementantion of attack action
{
    public int staminaCost => 20;

    public Combat_Action_Type ActionType => Combat_Action_Type.Attack;

    public ListKey<Combat_Action_mod> Mods => new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>(){
        Combat_Action_mod.Up, Combat_Action_mod.Front });

    NovaCharacterController recieverCharacter;

    public void Execute()
    {
        // trigger attack logic/animation




        Debug.Log($"{recieverCharacter.name} does a super attakck. not implemented yet.");
    }

    public Action_superAttack(NovaCharacterController character)
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