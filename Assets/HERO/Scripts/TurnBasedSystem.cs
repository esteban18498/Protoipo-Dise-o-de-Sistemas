using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    public TimeSlideBar timeSlideBar;

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

    public float turnTime = 10.0f;

    private Coroutine coroutine;

    public HERO AttackingHero;
    public HERO DefendingHero;

    public InputHandler player1;
    public InputHandler player2;


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
                player1.playerInput = PlayerAsignation.instance.player1;
                player2.playerInput = PlayerAsignation.instance.player2;

                player1.gameObject.SetActive(true);
                player2.gameObject.SetActive(true);

                currentPhase = TurnPhase.Start;
                break;

            case TurnPhase.Start:
                // Start turn for AttackingHero
                AttackingHero.sequenceManager.StartSequence();
                coroutine = StartCoroutine(turnCooldown(turnTime));
                currentPhase = TurnPhase.WaitingForInputs;
                break;

            case TurnPhase.WaitingForInputs:
                // Wait for player inputs handled in InputHandler and SequenceManager
                break;

            case TurnPhase.WaitingForCombatAnimations:
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                    coroutine = null;
                }
                // Wait for animations to complete
                break;

            case TurnPhase.SwitchTurns:

                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                    coroutine = null;
                }

                if(DefendingHero.health.Current <= 0)
                {
                    currentPhase = TurnPhase.CombatEnd;
                    break;
                }


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
                SceneManager.LoadScene("MainMenu");
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

    IEnumerator turnCooldown(float seconds)
    {
        timeSlideBar.StartTimer(seconds);
        if (seconds <= 0)
        {
            yield break;
        }
        yield return new WaitForSeconds(seconds);
        AttackingHero.sequenceManager.ExecuteSecuenceAtMid();

    }
}
