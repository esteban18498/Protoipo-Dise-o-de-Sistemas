using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    
}

public class Action_Advance : ICombatAction // concrect implementantion of action to advance forward one spot
{
    public int staminaCost => 0;

    public Combat_Action_Type actionType => Combat_Action_Type.Move;

    public ListKey<Combat_Action_mod> mods => new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>());

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
}

public class Action_Attack : ICombatAction // concrect implementantion of attack action
{
    public int staminaCost => 1;

    public Combat_Action_Type actionType => Combat_Action_Type.Attack;

    public ListKey<Combat_Action_mod> mods => new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>());

    NovaCharacterController recieverCharacter;

    public void Execute()
    {
        // attack logic:

        //get character front anchor
        
        //move sowrd from anchor to front anchor
        
        // wait for 0.3 action time

        //check for collision with enemy at anchor
        //deal damage if hit



        //wait for 0.3 action time

        //move sword back to character


        Debug.Log($"{recieverCharacter.name} Executing Attack Action: Attacking.");
    }

    public Action_Attack(NovaCharacterController character)
    {
        recieverCharacter = character;
    }

    public ICombatAction createActionInstance(NovaCharacterController character)
    {
        return new Action_Attack(character);
    }
}

public class Action_Unknown : ICombatAction // concrect implementantion of attack action
{
    public int staminaCost => 1;

    public Combat_Action_Type actionType => Combat_Action_Type.Utils;

    public ListKey<Combat_Action_mod> mods => new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>());

    NovaCharacterController recieverCharacter;

    public void Execute()
    {
        // trigger attack logic/animation


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
}


public class Action_superAttack : ICombatAction // concrect implementantion of attack action
{
    public int staminaCost => 1;

    public Combat_Action_Type actionType => Combat_Action_Type.Attack;

    public ListKey<Combat_Action_mod> mods => new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>(){
        Combat_Action_mod.Up, Combat_Action_mod.Front });

    NovaCharacterController recieverCharacter;

    public void Execute()
    {
        // trigger attack logic/animation




        Debug.Log($"{recieverCharacter.name} does a super attakck.");
    }

    public Action_superAttack(NovaCharacterController character)
    {
        recieverCharacter = character;
    }

    public ICombatAction createActionInstance(NovaCharacterController character)
    {
        return new Action_Attack(character);
    }
}