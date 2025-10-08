using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UIElements;

public class NovaCharacterController : MonoBehaviour
{
    // --- State ---

    [Header("State")]
    [SerializeField] public SpotController CurrentSpot;
    private SpotController lastSpot;
    [SerializeField] private ICharacterState characterState;
    public Combat_state combat_state = Combat_state.free_move;


    // --- Combat Action Queue ---
    [Header("Combat Action ")]
    public CombatActionDictionary CombatActionDictionary;
    public CombatActionQueue actionQueue = new CombatActionQueue();


    // --- References ---
    [Header("Internal References")]
    [SerializeField] private Transform anchor;
    [SerializeField] private RigCharacter rigCharacter;
    [SerializeField] private GameObject body;

    private ArenaController arena;

    // --- Parameters ---
    [Header("Character Stats")]
    [SerializeField] private float speed = 5f;




    void Awake()
    {
        if (CurrentSpot == null)
        {
            throw new System.Exception("CurrentSpot is not set. Please assign a SpotController in the inspector.");
        }
        else
        {
            anchor.position = CurrentSpot.transform.position;
            arena = CurrentSpot.GetComponentInParent<ArenaController>();
        }

        if (rigCharacter == null)
        {
            throw new System.Exception("rigCharacter is not set. Please assign a RigCharacter in the inspector.");
        }
        else
        {
            rigCharacter.speed = speed;
        }

        CombatActionDictionary = new CombatActionDictionary(this);
    }
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



    // Move anchor to the next spot in the arenaController's list
    public void MoveToNextSpot()
    {
        if (arena == null) return;
        var nextSpot = arena.getNextSpot(CurrentSpot);
        if (nextSpot == null) return;
        lastSpot = CurrentSpot;
        CurrentSpot = nextSpot;
        anchor.position = CurrentSpot.transform.position;
    }

    // Move anchor to the previous spot in the arenaController's list
    public void MoveToPreviousSpot()
    {
        if (arena == null) return;
        var nextSpot = arena.getPrevSpot(CurrentSpot);
        if (nextSpot == null) return;
        lastSpot = CurrentSpot;
        CurrentSpot = nextSpot;
        anchor.position = CurrentSpot.transform.position;
    }

    public void MoveBackToLastSpot()
    {
        if (lastSpot == null) return;
        CurrentSpot = lastSpot;
        anchor.position = CurrentSpot.transform.position;
    }

    public void RequestFreezState()
    {

        if (combat_state != Combat_state.free_move)
        {
            return;
        }

        CombatManager.Instance.EnterFreezState();
    }

    public void EnterFreezState()
    {
        combat_state = Combat_state.freez;


        //actionQueue.EnqueueAction(new Action_Advance(this));

        /*
        CombatActionDictionary.MovesDictionary.TryGetValue(
            new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>{}), out ICombatAction action);
        Debug.Log($"Enqueued {action} action");
        actionQueue.EnqueueAction(action);
        */
    }

    public void EnterPerformState()
    {
        combat_state = Combat_state.perfom;
        actionQueue.FillQueueWithSteps(this);
        StartCoroutine(performActionTimer());
    }

    public void EnterFreeMoveState()
    {
        combat_state = Combat_state.free_move;
        actionQueue.ClearQueue();

    }

    public IEnumerator performActionTimer()
    {
        // Example: Wait for 1 second before performing the next action
        while (actionQueue.HasActions())
        {
            actionQueue.ExecuteNextAction();

            yield return new WaitForSeconds(1f); // Wait 1 second between actions
        }
        EnterFreeMoveState();
    }
}

