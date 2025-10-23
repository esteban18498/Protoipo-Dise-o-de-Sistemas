using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;



public enum Combat_state {
 free_move = 0,
 freez = 1,
 perfom = 2
}

public class CombatManager : MonoBehaviour, ICombatPhaseSource
{
    static public CombatManager Instance;

    [Header("UI Phase Events (Inspector-callable)")]
    public UnityEvent OnEnterFreeMove;
    public UnityEvent OnEnterOnGuard;      // freez
    public UnityEvent OnEnterPerformance;

    [Header("Combat Management)")]
    public float freezDuration = 3.0f;
    public float perfomDuration = 3.0f;


    public Image time_bar;
    public float timer_start = 0.0f;


    public Combat_state state = Combat_state.free_move;
    [SerializeField] private TextMeshProUGUI state_text;

    [SerializeField] private NovaCharacterController Character1; // left
    [SerializeField] private NovaCharacterController Character2; // right


    #region ICombatPhaseSource    
    // ==== ICombatPhaseSource ====
    public event System.Action<CombatPhase> OnPhaseChanged;
    public CombatPhase Current => Map(state);

    private CombatPhase Map(Combat_state combatState) // mapeo a la UI
        => combatState switch
        {
            Combat_state.free_move => CombatPhase.FreeMove,     
            Combat_state.freez => CombatPhase.OnGuard,      
            Combat_state.perfom => CombatPhase.Performance, 
            _ => CombatPhase.FreeMove
        };

    private void RaisePhaseChanged()
    {
        Debug.Log($"[CM] Raising phase: {Current} (state={state})", this);
        OnPhaseChanged?.Invoke(Current);

        switch (Current)
        {
            case CombatPhase.FreeMove:
                OnEnterFreeMove?.Invoke();
                break;
            case CombatPhase.OnGuard:
                OnEnterOnGuard?.Invoke();
                break;
            case CombatPhase.Performance:
                OnEnterPerformance?.Invoke();
                break;
        }
    }

    // ============================
    #endregion    

    public void Awake()
    {
        Instance = this;
    }



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case Combat_state.free_move:
                state_text.text = "free move";
                time_bar.fillAmount = 0.0f;
                break;
            case Combat_state.freez:
                state_text.text = "freez";
                time_bar.fillAmount = 1-(Time.time - timer_start) / freezDuration;
                break;
            case Combat_state.perfom:
                state_text.text = "performing";
                time_bar.fillAmount = 1-(Time.time -timer_start) / perfomDuration;
                break;
        }
    }

    public void EnterFreezState()
    {
        if (state != Combat_state.free_move) return;

        state = Combat_state.freez;
        RaisePhaseChanged(); // ← notifica OnGuard

        Character1.EnterFreezState();
        Character2.EnterFreezState();

        timer_start = Time.time;
        StartCoroutine(EndFreez());
    }

    public IEnumerator EndFreez()
    {
        yield return new WaitForSeconds(freezDuration);

        state = Combat_state.perfom;
        RaisePhaseChanged(); // ← notifica Performance

        Character1.EnterPerformState();
        Character2.EnterPerformState();

        timer_start = Time.time;
        StartCoroutine(EndPerform());
    }

    public IEnumerator EndPerform()
    {
        yield return new WaitForSeconds(perfomDuration);

        state = Combat_state.free_move;
        RaisePhaseChanged(); // ← notifica FreeMove

        Character1.EnterFreeMoveState();
        Character2.EnterFreeMoveState();
    }


}
