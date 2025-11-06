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


    public void Start()
    {
        sequenceManager = GetComponent<SequenceManager>();
        sequenceManager.hero = this;
        animator = GetComponent<Animator>();
    }

    public void CombatAnimationComplete()
    {
        TurnBasedSystem.Instance.OnCombatAnimationsComplete();
        Debug.Log("HERO Combat Animation Complete, notifying TurnBasedSystem.");
    }

}
