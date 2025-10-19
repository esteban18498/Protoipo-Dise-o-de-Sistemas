using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Works on Button, Toggle, Slider, Dropdown, etc.
[RequireComponent(typeof(Selectable))]
public class UIAudioBehaviour : MonoBehaviour,
    IPointerEnterHandler, IPointerClickHandler,
    ISelectHandler, ISubmitHandler
{
    [Header("Global defaults (optional):")]
    [SerializeField] private UIAudioSettings defaults;

    [Header("Per-control overrides (optional):")]
    [SerializeField] private AudioCue hoverCue;
    [SerializeField] private AudioCue selectCue; // keyboard/gamepad focus
    [SerializeField] private AudioCue clickCue;  // mouse click / submit
    [SerializeField] private AudioCue cancelCue; // optional, rarely per-control
    [SerializeField] private AudioCue changeCue; // toggle/sliders

    [Header("Anti-spam")]
    [SerializeField] private float hoverCooldown = 0.05f;
    private float _lastHoverTime;

    private Selectable _sel;

    void Awake() => _sel = GetComponent<Selectable>();

    // ---------- Mouse hover ----------
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_sel || !_sel.interactable) return;
        if (Time.unscaledTime - _lastHoverTime < hoverCooldown) return;
        _lastHoverTime = Time.unscaledTime;

        var cue = hoverCue ? hoverCue : defaults ? defaults.hoverDefault : null;
        if (cue) AudioManager.Instance.PlaySfx(cue); // 2D by default
    }

    // ---------- Mouse click ----------
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_sel || !_sel.interactable) return;
        var cue = clickCue ? clickCue : defaults ? defaults.clickDefault : null;
        if (cue) AudioManager.Instance.PlaySfx(cue);
    }

    // ---------- Gamepad/Keyboard selection focus ----------
    public void OnSelect(BaseEventData eventData)
    {
        if (!_sel || !_sel.interactable) return;
        var cue = selectCue ? selectCue : defaults ? defaults.selectDefault : null;
        if (cue) AudioManager.Instance.PlaySfx(cue);
    }

    // ---------- Submit (Enter/Space / gamepad A) ----------
    public void OnSubmit(BaseEventData eventData)
    {
        if (!_sel || !_sel.interactable) return;
        var cue = clickCue ? clickCue : defaults ? defaults.clickDefault : null;
        if (cue) AudioManager.Instance.PlaySfx(cue);
    }

    // ---------- Helpers you can call from UI events ----------
    public void PlayChange()
    {
        var cue = changeCue ? changeCue : defaults ? defaults.changeDefault : null;
        if (cue) AudioManager.Instance.PlaySfx(cue);
    }
    public void PlayCancel()
    {
        var cue = cancelCue ? cancelCue : defaults ? defaults.cancelDefault : null;
        if (cue) AudioManager.Instance.PlaySfx(cue);
    }
}
