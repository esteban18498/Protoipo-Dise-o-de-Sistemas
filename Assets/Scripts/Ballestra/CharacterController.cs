using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterController : MonoBehaviour
{
    // --- State ---

    [Header("State")]
    [SerializeField] public SpotController CurrentSpot;


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
    
        // Move anchor to the next spot in the arenaController's list
    public void MoveToNextSpot()
    {
        if (arena == null) return;
        var nextSpot = arena.getNextSpot(CurrentSpot);
        if (nextSpot == null) return;
        CurrentSpot = nextSpot;
        anchor.position = CurrentSpot.transform.position;
    }

    // Move anchor to the previous spot in the arenaController's list
    public void MoveToPreviousSpot()
    {
        if (arena == null) return;
        var nextSpot = arena.getPrevSpot(CurrentSpot);
        if (nextSpot == null) return;
        CurrentSpot = nextSpot;
        anchor.position = CurrentSpot.transform.position;
    }
}
