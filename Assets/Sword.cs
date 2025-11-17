using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public float baseDamage = 10f;
    public float damage = 10f;
    public float healAmount = 5f;

    public Color normalColor;
    public Color activeHitColor;
    public Color blockColor;

    public bool isBlocking = false;


    public HERO hero; 

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
            HERO enemy = collision.GetComponent<HERO>();
            if (enemy != null)
            {
                //hit enemy
                enemy.Gethit( damage);
                //enemy.TakeDamage(10);
            }
            return;
        }
        //Debug.Log("trigger Block!");
        SwordColliderBind otherSwordBind = collision.GetComponent<SwordColliderBind>();
        Sword otherSword = otherSwordBind?.sword;
        if (otherSword != null)
        {
            
            //other swword blocked
          otherSword.hero.Parried();
          hero.GetHeal(healAmount); // Heal the hero for half the damage value upon a successful parry

        }
            
    }
}
