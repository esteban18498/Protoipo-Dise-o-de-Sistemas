// PlayerComboUIController.cs
using UnityEngine;

public class PlayerComboUIController : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private MonoBehaviour phaseSourceBehaviour; // arrastrar CombatManager en escena
    private ICombatPhaseSource phaseSource;

    [Header("UI")]
    [SerializeField] private ComboPanelUI panelUI;
    [SerializeField] private ComboHistoryUI historyUI;

    public InputBuffer Buffer { get; private set; } = new();

    private CombatPhase _lastPhase = CombatPhase.FreeMove;

    public void PushDirection(DirKey d) => Buffer.PushDirection(d);


    public void PressAction(ActKey a, bool comboSuccess) // Cuando el jugador presiona acción, se “commitea”
    {
        Buffer.SetAction(a);
        var snap = Buffer.Snapshot(); // snapshot para mostrar en history


        if (comboSuccess)
        {
            panelUI.FlashSuccess();
            if (historyUI) historyUI.AddRow(snap);
            Buffer.Clear();
        }
        else
        {
            panelUI.FlashFail(); // o esperar un frame que se vea el fail con la acción puesta

            Buffer.Clear();
        }
    }


    private void Awake()
    {
        if (panelUI != null && panelUI.gameObject == gameObject)
            Debug.LogError($"{name}: panelUI cannot be on the same GameObject as the controller. " +
                           "Move the controller to a different GO.", this);

        phaseSource = phaseSourceBehaviour as ICombatPhaseSource;
        panelUI.Bind(Buffer);

        if (phaseSource != null)
        {
            _lastPhase = phaseSource.Current;
            phaseSource.OnPhaseChanged += OnPhaseChanged;
            OnPhaseChanged(phaseSource.Current); // inicializa
        }
        else
        {
            // Fallback: si no hay fuente de fases, mantener visible
            panelUI.SetVisible(true);
            if (historyUI) historyUI.SetVisible(true);
        }
    }

    private void OnEnable()
    {
        if (phaseSource == null)
            phaseSource = phaseSourceBehaviour as ICombatPhaseSource;

        if (phaseSource != null)
        {
            phaseSource.OnPhaseChanged -= OnPhaseChanged; // avoid double-subscribe
            phaseSource.OnPhaseChanged += OnPhaseChanged;
            OnPhaseChanged(phaseSource.Current); // force initial visibility
        }
    }

    private void OnDisable()
    {
        if (phaseSource != null)
            phaseSource.OnPhaseChanged -= OnPhaseChanged;
    }


    private void OnDestroy()
    {
        if (phaseSource != null) phaseSource.OnPhaseChanged -= OnPhaseChanged;
    }

  
    private void OnPhaseChanged(CombatPhase phase)
    {
        Debug.Log($"[ComboUI] Phase changed to: {phase}", this);
        // Panel e histórico solo visibles en OnGuard
        bool onGuard = (phase == CombatPhase.OnGuard);
        panelUI.SetVisible(onGuard);
        if (historyUI) historyUI.SetVisible(onGuard);

        // Limpiar histórico y buffer al SALIR de OnGuard (cuando pase a Performance o FreeMove)
        if (_lastPhase == CombatPhase.OnGuard && phase != CombatPhase.OnGuard)
        {
            if (historyUI) historyUI.ClearAll();
            Buffer.Clear();
        }

        _lastPhase = phase;
    }

}
