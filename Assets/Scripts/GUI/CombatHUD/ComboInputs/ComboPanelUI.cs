using UnityEngine;
using UnityEngine.UI;

public class ComboPanelUI : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField] private InputIconSet iconSet;

    [Header("Slots (1..4)")]
    [SerializeField] private Image slot1; // dir #1
    [SerializeField] private Image slot2; // dir #2
    [SerializeField] private Image slot3; // dir #3
    [SerializeField] private Image slot4; // action

    [Header("Visual feedback")]
    [SerializeField] private CanvasGroup group;
    [SerializeField] private Image backdrop; // para flash rojo/verde (opcional)

    private IReadonlyInputBuffer buffer;

    public void Bind(IReadonlyInputBuffer buf)
    {
        if (buffer != null) buffer.OnChanged -= Refresh;
        buffer = buf;
        if (buffer != null) buffer.OnChanged += Refresh;
        Refresh();
    }

    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    public void Refresh()
    {
        if (!iconSet) return;
        if (buffer == null) { Clear(); return; }

        var dirs = buffer.Directions;
        slot1.sprite = iconSet.Get(dirs.Count > 0 ? dirs[0] : DirKey.None);
        slot2.sprite = iconSet.Get(dirs.Count > 1 ? dirs[1] : DirKey.None);
        slot3.sprite = iconSet.Get(dirs.Count > 2 ? dirs[2] : DirKey.None);
        slot4.sprite = iconSet.Get(buffer.Action);
    }

    public void Clear()
    {
        slot1.sprite = iconSet.Get(DirKey.None);
        slot2.sprite = iconSet.Get(DirKey.None);
        slot3.sprite = iconSet.Get(DirKey.None);
        slot4.sprite = iconSet.Get(ActKey.None);
    }

    // feedback simple
    public void FlashSuccess()
    {
        if (!isActiveAndEnabled || backdrop == null) return;
        StartCoroutine(Flash(backdrop, new Color(0f, 1f, 0f, 0.4f)));
    }

    public void FlashFail()
    {
        if (!isActiveAndEnabled || backdrop == null) return;
        StartCoroutine(Flash(backdrop, new Color(1f, 0f, 0f, 0.4f)));
    }

    private System.Collections.IEnumerator Flash(Image img, Color c)
    {
        img.color = c; img.enabled = true;
        yield return new WaitForSeconds(0.15f);
        img.enabled = false;
    }
}
