using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Block : ICombatAction // concrect implementantion of void action to do nothing
{
    public int staminaCost => 10;

    Transform targetAnchor;
    Transform originAnchor;

    Sword sword;

    Coroutine returnCoroutineRef;

    public Combat_Action_Type actionType => Combat_Action_Type.Block;

    public ListKey<Combat_Action_mod> mods => new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>());

    NovaCharacterController recieverCharacter;


    public Action_Block(NovaCharacterController character)
    {
        recieverCharacter = character;

        targetAnchor = recieverCharacter.GetComponent<Transform>().Find("Anchor").Find("Center").Find("R Block");
        sword = recieverCharacter.RigCharacter.Sword.GetComponent<Sword>();}

    public void Execute()
    {
        //move sowrd from anchor to front anchor
        originAnchor = recieverCharacter.RigCharacter.Sword.Anchor; // save current sword anchor
        recieverCharacter.RigCharacter.Sword.Anchor = targetAnchor; // set sword anchor to front anchor
        sword.SetBlockColor();
        sword.isBlocking = true;
        
        //start coroutine to move sword back after delay

        returnCoroutineRef = recieverCharacter.StartCoroutine(returnCoroutine());}



    public ICombatAction createActionInstance(NovaCharacterController character)
    {
        return new Action_Step(character);
    }

    public void Interrupt()
    {
        // Interrupt logic for super attack
    }
    
    
    IEnumerator returnCoroutine()
    {
        // Wait for 0.5 seconds (action time)
        yield return new WaitForSeconds(0.6f);

        // Move sword back to character
        recieverCharacter.RigCharacter.Sword.Anchor = originAnchor;
        sword.isBlocking = false;
        sword.SetNormalColor();
    }
}