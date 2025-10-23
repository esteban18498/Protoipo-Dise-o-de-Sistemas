// ComboHistoryUI.cs
using UnityEngine;
using UnityEngine.UI;

public class ComboHistoryUI : MonoBehaviour
{
    [SerializeField] private InputIconSet iconSet;
    [SerializeField] private RectTransform container;
    [SerializeField] private GameObject rowPrefab; // contiene 4 Images alineadas
    [SerializeField] private CanvasGroup group; // asigná el de History_Left

    public void ClearAll()
    {
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
    }

    public void AddRow(ComboSnapshot snap)
    {
        var go = Instantiate(rowPrefab, container);
        // Busca el contenedor “Slots” dentro del Row
        var slots = go.transform.Find("Slots");
        var imgs = slots.GetComponentsInChildren<Image>(true); // deben ser 4

        imgs[0].sprite = iconSet.Get(snap.d1);
        imgs[1].sprite = iconSet.Get(snap.d2);
        imgs[2].sprite = iconSet.Get(snap.d3);
        imgs[3].sprite = iconSet.Get(snap.act);
    }

    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}
