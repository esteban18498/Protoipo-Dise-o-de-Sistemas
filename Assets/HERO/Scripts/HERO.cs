using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HEROState
{
    Idle,
    Attacking,
    Defending,
}


public class HERO : MonoBehaviour
{
    public SequenceManager sequenceManager;
    public HEROState currentState = HEROState.Idle;

    public Animator animator;

    public Sword sword;

    public HealthComponent health;

    public void Start()
    {
        sequenceManager = GetComponent<SequenceManager>();
        sequenceManager.hero = this;
        animator = GetComponent<Animator>();
        health = GetComponent<HealthComponent>();
        sword.hero = this;
    }

    public void CombatAnimationComplete()
    {
        TurnBasedSystem.Instance.OnCombatAnimationsComplete();
    }

    public void AttackAnimation()
    {
        animator.SetTrigger("Attack1");
    }

    public void BlockAnimation()
    {
        animator.SetTrigger("Block");
    }

    public void ParryActiveFrame()
    {
        sword.isBlocking = true;
    }

    public void ParryEndFrame()
    {
        sword.isBlocking = false;
    }

    public void Parried()
    {
        animator.SetTrigger("Parried");
        Debug.Log("Parried!");
    }

    public void Gethit(float damage)
    {

        health.ApplyDamage(damage);
        sword.isBlocking = false;

    }

}
