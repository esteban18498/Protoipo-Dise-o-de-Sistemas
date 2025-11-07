using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TurnPhase
{
    CombatStart,
    Start,
    WaitingForInputs,
    WaitingForCombatAnimations,
    SwitchTurns,
    CombatEnd,
}   

public class TurnBasedSystem : MonoBehaviour
{
    //singleton instance
    public static TurnBasedSystem Instance;

    void Awake()
    {
        //if instance is null, set it to this
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public TurnPhase currentPhase;

    public HERO AttackingHero;
    public HERO DefendingHero;


    // Start is called before the first frame update
    void Start()
    {
        AttackingHero.currentState = HEROState.Attacking;
        DefendingHero.currentState = HEROState.Defending;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentPhase)
        {
            case TurnPhase.CombatStart:
                // Initialize combat
                currentPhase = TurnPhase.Start;
                break;

            case TurnPhase.Start:
                // Start turn for AttackingHero
                AttackingHero.sequenceManager.StartSequence();
                currentPhase = TurnPhase.WaitingForInputs;
                break;

            case TurnPhase.WaitingForInputs:
                // Wait for player inputs handled in InputHandler and SequenceManager
                break;

            case TurnPhase.WaitingForCombatAnimations:
                // Wait for animations to complete
                break;

            case TurnPhase.SwitchTurns:
                // Switch roles
                HERO temp = AttackingHero;
                AttackingHero = DefendingHero;
                DefendingHero = temp;

                AttackingHero.currentState = HEROState.Attacking;
                DefendingHero.currentState = HEROState.Defending;

                currentPhase = TurnPhase.Start;
                break;

            case TurnPhase.CombatEnd:
                // Handle end of combat
                break;
        }
    }

    public void SequenceCompleted()
    {
        if (currentPhase != TurnPhase.WaitingForInputs)
            return;
        // Called by SequenceManager when sequence is complete
        currentPhase = TurnPhase.WaitingForCombatAnimations;

    }

    public void SequenceFailed()
    {
        if (currentPhase != TurnPhase.WaitingForCombatAnimations)
            return;
        // Called by SequenceManager when sequence fails
        currentPhase = TurnPhase.SwitchTurns;
    }

    public void OnCombatAnimationsComplete()
    {
        // Called when combat animations are done
        currentPhase = TurnPhase.SwitchTurns;
    }
}
