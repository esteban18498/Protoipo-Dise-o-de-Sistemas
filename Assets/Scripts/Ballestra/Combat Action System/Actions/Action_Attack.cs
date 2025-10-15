using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Attack : ICombatAction // concrect implementantion of attack action
{
    public int staminaCost => 1;
    public int damage => 10;

    public Combat_Action_Type actionType => Combat_Action_Type.Attack;

    public ListKey<Combat_Action_mod> mods => new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>());

    NovaCharacterController recieverCharacter;

    public void Execute()
    {
        // attack logic:

        //get character front anchor
        Transform frontAnchor = recieverCharacter.GetComponent<Transform>().Find("Anchor").Find("Front");//bad performace... doable for now... can be better on construction


        //move sowrd from anchor to front anchor
        Transform saveSwordAnchor = recieverCharacter.RigCharacter.Sword.Anchor; // save current sword anchor
        recieverCharacter.RigCharacter.Sword.Anchor = frontAnchor; // set sword anchor to front anchor
        
        //start coroutine to move sword back after delay

        recieverCharacter.StartCoroutine(HitBoxCoroutine(frontAnchor, saveSwordAnchor));
    }

    public Action_Attack(NovaCharacterController character)
    {
        recieverCharacter = character;
    }

    public ICombatAction createActionInstance(NovaCharacterController character)
    {
        return new Action_Attack(character);
    }


    IEnumerator HitBoxCoroutine(Transform frontAnchor, Transform saveSwordAnchor)
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

        recieverCharacter.RigCharacter.Sword.Anchor = saveSwordAnchor;
    }
}