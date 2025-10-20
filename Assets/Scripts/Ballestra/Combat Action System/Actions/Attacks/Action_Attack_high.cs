using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Attack_high : ICombatAction // concrect implementantion of attack action
{
    public int staminaCost => 10;
    public int damage => 10;

    public Combat_Action_Type actionType => Combat_Action_Type.Attack;

    public ListKey<Combat_Action_mod> mods => new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>()
    {
        Combat_Action_mod.Up,
    });

    NovaCharacterController recieverCharacter;

    Transform targetAnchor;
    Transform originAnchor;

    Sword sword;

    Coroutine hitCoroutineRef;
    
    

    public void Execute()
    {
        //move sowrd from anchor to front anchor
        originAnchor = recieverCharacter.RigCharacter.Sword.Anchor; // save current sword anchor
        recieverCharacter.RigCharacter.Sword.Anchor = targetAnchor; // set sword anchor to front anchor
        sword.SetActiveHitColor();
        
        //start coroutine to move sword back after delay

        hitCoroutineRef = recieverCharacter.StartCoroutine(HitBoxCoroutine());
    }

    public Action_Attack_high(NovaCharacterController character)
    {
        recieverCharacter = character;

        targetAnchor = recieverCharacter.GetComponent<Transform>().Find("Anchor").Find("Front").Find("FrontTop");
        sword = recieverCharacter.RigCharacter.Sword.GetComponent<Sword>();

    }

    public ICombatAction createActionInstance(NovaCharacterController character)
    {
        return new Action_Attack(character);
    }


    IEnumerator HitBoxCoroutine()
    {
        // Wait for 0.3 seconds (action time)
        yield return new WaitForSeconds(0.5f);

        // Move sword back to character

        //check for collision with enemy at anchor

        //        Collider[] hitColliders = Physics.OverlapSphere(frontAnchor.position, 0.5f);
        List<Collider2D> hitColliders = new List<Collider2D>();
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.NoFilter();
        contactFilter.useTriggers = false; // Ignore trigger colliders

        Physics2D.OverlapCollider(recieverCharacter.RigCharacter.Sword.GetComponentInChildren<Collider2D>(), contactFilter, hitColliders);
        foreach (var hitCollider in hitColliders)
        {
            NovaCharacterController enemy = hitCollider.GetComponentInParent<NovaCharacterController>();
            if (enemy != null && enemy != recieverCharacter)
            {
                //Debug.Log($"{recieverCharacter.name} hit {enemy.name} with an attack!");

                enemy.GetComponent<IHealth>().ApplyDamage(damage);

            }
        }


        //move sword back to character
        recieverCharacter.RigCharacter.Sword.Anchor = originAnchor;
        sword.SetNormalColor();
    }

    public void Interrupt()
    {
        if (hitCoroutineRef != null)
        {
            recieverCharacter.StopCoroutine(hitCoroutineRef);
            hitCoroutineRef = null;
        }
        // Move sword back to character immediately
        recieverCharacter.RigCharacter.Sword.Anchor = originAnchor;
        sword.SetNormalColor();
    }
}