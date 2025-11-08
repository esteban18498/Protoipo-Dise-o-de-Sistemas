using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public float baseDamage = 10f;
    public float damage = 10f;

    public Color normalColor;
    public Color activeHitColor;
    public Color blockColor;

    public bool isBlocking = false;


    //[SerializeField] private NovaCharacterController character; 

    public void SetNormalColor()
    {
        this.GetComponentInChildren<SpriteRenderer>().color = normalColor;
    }

    public void SetActiveHitColor()
    {
        this.GetComponentInChildren<SpriteRenderer>().color = activeHitColor;
    }

    public void SetBlockColor()
    {
        this.GetComponentInChildren<SpriteRenderer>().color = blockColor;
    }

    public void triggerColliderEnter(Collider2D collision)
    {
        if (!isBlocking)
        {
            HealthComponent enemy = collision.GetComponent<HealthComponent>();
            if (enemy != null)
            {
                //hit enemy
                enemy.ApplyDamage(damage);
                //enemy.TakeDamage(10);
            }
            return;
        }

        SwordColliderBind otherSwordBind = collision.GetComponent<SwordColliderBind>();
        Sword otherSword = otherSwordBind?.sword;
        if (otherSword != null)
        {
            //other swword blocked
          // otherSword.character.InterruptCurrentAction();
        }
            
    }
}
