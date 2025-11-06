using System;
using System.Collections;
using UnityEngine;

public class TrainingCharacter : NovaCharacterController
{
    // --- State ---

    //[Header("State")]
    //[SerializeField] public SpotController CurrentSpot;

    //public Combat_state combat_state = Combat_state.free_move;

    // --- Combat Action Queue ---
    //[Header("Combat Action ")]
    //public CombatActionDictionary CombatActionDictionary;
    //public CombatActionQueue actionQueue;


    // --- References ---
    //[Header("Internal References")]
    //[SerializeField] private Transform anchor;
    //public RigCharacter RigCharacter;


    // --- Parameters ---
    // [Header("Character Stats")]





    //public Action OnFreezEnd;



    void Start()
    {

    }

    void FixedUpdate()
    {
        /*
        body.transform.position = Vector3.Lerp(
            body.transform.position,
            anchor.position,
            Time.deltaTime * speed
        );
        */
    }

    void Update()
    {
        switch (combat_state)
        {
            case Combat_state.free_move:

                break;
            case Combat_state.freez:

                break;
            case Combat_state.perfom:

                break;
            default:
                throw new System.Exception("Unknown combat state: " + combat_state);
        }
    }







    new public void RequestFreezState()
    {
/*
        if (combat_state != Combat_state.free_move)
        {
            return;
        }

        CombatManager.Instance.EnterFreezState();
        */
    }

    new public void EnterFreezState()
    {
       // combat_state = Combat_state.freez;


    }

    new public void EnterPerformState()
    {
        /*
        OnFreezEnd.Invoke();
        
        combat_state = Combat_state.perfom;
        actionQueue.FillQueueWithSteps();
        StartCoroutine(performActionTimer());
        */
    }

    new public void EnterFreeMoveState()
    {
        /*
        combat_state = Combat_state.free_move;
        actionQueue.ClearQueue();
*/
    }

    new public IEnumerator performActionTimer()
    {
        // Example: Wait for 1 second before performing the next action
        while (actionQueue.HasActions())
        {
            actionQueue.ExecuteNextAction();

            yield return new WaitForSeconds(1f); // Wait 1 second between actions
        }
        EnterFreeMoveState();
    }

    new public void InterruptCurrentAction()
    {
        actionQueue.executingAction?.Interrupt();
    }
}

