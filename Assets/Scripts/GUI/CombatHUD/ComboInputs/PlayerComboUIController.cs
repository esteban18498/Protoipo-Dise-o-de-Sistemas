using UnityEngine;

public class PlayerComboUIController : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private MonoBehaviour phaseSourceBehaviour;   // CombatManager
    private ICombatPhaseSource phaseSource;

    [Header("UI")]
    [SerializeField] private ComboPanelUI panelUI;     // InputPanel_Left/Right
    [SerializeField] private ComboHistoryUI historyUI; // History_Left/Right

    [Header("Behaviour")]
    [Tooltip("If false, ignores CombatManager events. You control Show/Hide manually (e.g., from Player.Freez/EndPerform).")]
    [SerializeField] private bool usePhaseEvents = true;
    [SerializeField] private bool pollPhaseEveryFrame = true;


    public InputBuffer Buffer { get; private set; } = new();
    private CombatPhase _lastPhase = CombatPhase.FreeMove;
    private CombatPhase _lastAppliedPhase = (CombatPhase)(-1);

    private bool _subscribed;

    public void PushDirection(DirKey d) => Buffer.PushDirection(d);

    public void PressAction(ActKey a, bool comboSuccess)
    {
        Buffer.SetAction(a);
        var snap = Buffer.Snapshot();

        if (comboSuccess)
        {
            panelUI.FlashSuccess();
            if (historyUI) historyUI.AddRow(snap);
        }
        else
        {
            panelUI.FlashFail();
        }
        Buffer.Clear();
    }

    // ----- explicit control (useful while debugging) -----
    public void ForceShow()
    {
        Debug.Log("[ComboUI] ForceShow()", this);
        if (panelUI) panelUI.SetVisible(true);
        if (historyUI) historyUI.SetVisible(true);
    }

    public void ForceHide(bool clearHistory = true)
    {
        Debug.Log("[ComboUI] ForceHide()", this);
        if (panelUI) panelUI.SetVisible(false);
        if (historyUI) historyUI.SetVisible(false);
        if (clearHistory && historyUI) historyUI.ClearAll();
        Buffer.Clear();
    }
    // -----------------------------------------------------

    private void Awake()
    {
        panelUI.Bind(Buffer);

        if (panelUI && panelUI.gameObject == gameObject)
            Debug.LogError($"{name}: Move PlayerComboUIController to a different GameObject than the panel.", this);
    }

    private void Start()
    {
        // Auto-wire the manager if not assigned
        if (phaseSourceBehaviour == null)
            phaseSourceBehaviour = FindObjectOfType<CombatManager>();

        phaseSource = phaseSourceBehaviour as ICombatPhaseSource;

        if (usePhaseEvents)
        {
            if (phaseSource == null)
            {
                Debug.LogError($"{name}: phaseSourceBehaviour not set or not ICombatPhaseSource.", this);
            }
            else if (!_subscribed)
            {
                phaseSource.OnPhaseChanged += OnPhaseChanged;
                _subscribed = true;
                Debug.Log($"[ComboUI] Subscribed to {phaseSourceBehaviour.name} (id {phaseSourceBehaviour.GetInstanceID()})", this);
                OnPhaseChanged(phaseSource.Current);
            }
        }
        else
        {
            // Manual mode: keep it visible until you call ForceHide
            ForceShow();
        }
    }

    private void Update()
    {
        if (!pollPhaseEveryFrame) return;
        if (phaseSource == null) return;

        var current = phaseSource.Current;
        if (current != _lastAppliedPhase)
        {
            OnPhaseChanged(current);
            _lastAppliedPhase = current;
        }
    }

    private void OnDestroy()
    {
        if (_subscribed && phaseSource != null)
            phaseSource.OnPhaseChanged -= OnPhaseChanged;
    }

    private void OnPhaseChanged(CombatPhase phase)
    {
        Debug.Log($"[ComboUI] Phase changed to: {phase}", this);

        bool onGuard = (phase == CombatPhase.OnGuard);
        if (panelUI) panelUI.SetVisible(onGuard);
        if (historyUI) historyUI.SetVisible(onGuard);

        // Clear when LEAVING OnGuard
        if (_lastPhase == CombatPhase.OnGuard && !onGuard)
        {
            if (historyUI) historyUI.ClearAll();
            Buffer.Clear();
        }

        _lastPhase = phase;
    }

    // Unity events to wire into combat manager
    public void ShowOnGuard()
    {
        if (panelUI) panelUI.SetVisible(true);
        if (historyUI) historyUI.SetVisible(true);
        Debug.Log("[ComboUI] ShowOnGuard", this);
    }

    public void HideOnNonGuardAndClear()
    {
        if (panelUI) panelUI.SetVisible(false);
        if (historyUI) historyUI.SetVisible(false);
        if (historyUI) historyUI.ClearAll();
        Buffer.Clear();
        Debug.Log("[ComboUI] HideOnNonGuardAndClear", this);
    }

}
