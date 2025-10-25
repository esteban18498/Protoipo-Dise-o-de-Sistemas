using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

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
    public CombatActionQueue actionQueue;


    // --- References ---
    [Header("Internal References")]
    [SerializeField] private Transform anchor;
    public RigCharacter RigCharacter;


    private ArenaController arena;

    // --- Parameters ---
    [Header("Character Stats")]
    [SerializeField] private float speed = 5f;



    public Action OnFreezEnd;


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

        if (RigCharacter == null)
        {
            throw new System.Exception("rigCharacter is not set. Please assign a RigCharacter in the inspector.");
        }
        else
        {
            RigCharacter.speed = speed;
        }

        actionQueue = new CombatActionQueue(this);
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
        Debug.Log("chars freez");
        Debug.Log(combat_state.ToString());
        


    }

    public void EnterPerformState()
    {
        OnFreezEnd.Invoke();
        
        combat_state = Combat_state.perfom;
        actionQueue.FillQueueWithSteps();
        StartCoroutine(performActionTimer());
    }

    public void EnterFreeMoveState()
    {
        combat_state = Combat_state.free_move;
        Debug.Log("enter free move");
        Debug.Log(combat_state.ToString());
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
        //EnterFreeMoveState();
    }

    public void InterruptCurrentAction()
    {
        actionQueue.executingAction?.Interrupt();
    }
}

