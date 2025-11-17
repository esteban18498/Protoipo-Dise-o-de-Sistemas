using System.Collections;
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
    // Singleton
    public static TurnBasedSystem Instance;

    [Header("Timeline")]
    public TimeSlideBar timeSlideBar;

    [Header("Turn Settings")]
    [Tooltip("Fallback fixed turn time if dynamic calculation fails.")]
    public float turnTime = 10.0f;

    private Coroutine coroutine;

    [Header("Heroes")]
    public HERO AttackingHero;
    public HERO DefendingHero;

    [Header("Input Handlers")]
    public InputHandler player1;
    public InputHandler player2;

    [Header("Debug")]
    public TurnPhase currentPhase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initial states
        if (AttackingHero != null)
            AttackingHero.currentState = HEROState.Attacking;

        if (DefendingHero != null)
            DefendingHero.currentState = HEROState.Defending;
    }

    private void Update()
    {
        switch (currentPhase)
        {
            case TurnPhase.CombatStart:
                // Initialize combat + player inputs
                if (PlayerAsignation.instance != null)
                {
                    if (player1 != null)
                    {
                        player1.playerInput = PlayerAsignation.instance.player1;
                        player1.gameObject.SetActive(true);
                    }

                    if (player2 != null)
                    {
                        player2.playerInput = PlayerAsignation.instance.player2;
                        player2.gameObject.SetActive(true);
                    }
                }

                currentPhase = TurnPhase.Start;
                break;

            case TurnPhase.Start:
                // Start turn for AttackingHero
                if (AttackingHero != null && AttackingHero.sequenceManager != null)
                {
                    AttackingHero.sequenceManager.StartSequence();

                    // 🔥 Dynamic turn time based on both heroes' HP
                    float dynamicTurnTime = GetDynamicTurnTime();
                    coroutine = StartCoroutine(TurnCooldown(dynamicTurnTime));
                }

                currentPhase = TurnPhase.WaitingForInputs;
                break;

            case TurnPhase.WaitingForInputs:
                // Waiting for input; handled by InputHandler / SequenceManager
                break;

            case TurnPhase.WaitingForCombatAnimations:
                // Waiting for combat animations; SequenceManager / Animation events
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                    coroutine = null;
                }
                break;

            case TurnPhase.SwitchTurns:

                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                    coroutine = null;
                }

                // Check if defender died
                if (DefendingHero != null && DefendingHero.health != null &&
                    DefendingHero.health.Current <= 0)
                {
                    currentPhase = TurnPhase.CombatEnd;
                    break;
                }

                // Swap attacker/defender
                HERO temp = AttackingHero;
                AttackingHero = DefendingHero;
                DefendingHero = temp;

                if (AttackingHero != null)
                    AttackingHero.currentState = HEROState.Attacking;

                if (DefendingHero != null)
                    DefendingHero.currentState = HEROState.Defending;

                currentPhase = TurnPhase.Start;
                break;

            case TurnPhase.CombatEnd:
                // End of combat → back to main menu (or results screen later)
                SceneManager.LoadScene("MainMenu");
                break;
        }
    }

    /// <summary>
    /// Called by SequenceManager when the input sequence was successfully entered.
    /// </summary>
    public void SequenceCompleted()
    {
        if (currentPhase != TurnPhase.WaitingForInputs)
            return;

        currentPhase = TurnPhase.WaitingForCombatAnimations;
    }

    /// <summary>
    /// Called by SequenceManager when the sequence fails.
    /// </summary>
    public void SequenceFailed()
    {
        if (currentPhase != TurnPhase.WaitingForCombatAnimations)
            return;

        currentPhase = TurnPhase.SwitchTurns;
    }

    /// <summary>
    /// Called from animation events / SequenceManager when all combat anims are done.
    /// </summary>
    public void OnCombatAnimationsComplete()
    {
        currentPhase = TurnPhase.SwitchTurns;
    }

    /// <summary>
    /// Computes the current turn duration based on the sum of both heroes' HP,
    /// using TimeSlideBar's HP→time mapping. Falls back to 'turnTime' if anything is missing.
    /// </summary>
    private float GetDynamicTurnTime()
    {
        // Safety: if timer or heroes/health are missing, use fallback
        if (timeSlideBar == null ||
            AttackingHero == null || AttackingHero.health == null ||
            DefendingHero == null || DefendingHero.health == null)
        {
            return turnTime;
        }

        float currentSum = AttackingHero.health.Current + DefendingHero.health.Current;
        float maxSum = AttackingHero.health.Max + DefendingHero.health.Max;

        return timeSlideBar.CalculateArenaTimeFromHealth(currentSum, maxSum);
    }

    private IEnumerator TurnCooldown(float seconds)
    {
        if (timeSlideBar != null)
            timeSlideBar.StartTimer(seconds);

        if (seconds <= 0f)
            yield break;

        yield return new WaitForSeconds(seconds);

        if (AttackingHero != null && AttackingHero.sequenceManager != null)
        {
            AttackingHero.sequenceManager.ExecuteSecuenceAtMid();
        }
    }
}
