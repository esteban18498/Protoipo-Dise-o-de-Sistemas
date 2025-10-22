using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceBarUI : MonoBehaviour
{
    [Header("Wiring")]
    [SerializeField] private Image fill;                    // Image con Fill Method = Horizontal
    [SerializeField] private TMP_Text valueLabel;           // Opcional: "75 / 100"
    [SerializeField] private Gradient colorByPercent;       // Opcional: para colorear 0..1
    [SerializeField] private bool useGradient = false;
    [SerializeField] private Color baseColor = Color.white;


    // Fuente de datos (uno u otro)
    private IHealth health;
    private ISpendable spendable; // esto puede ser stamina, como en PlayerHUD.cs

    // Mantener referencias a los handlers para desuscribir
    private Action<float, float> onChangedHandler;

    public void Bind(Component provider)
    {
        Unbind();

        health = provider as IHealth;            // si el component implementa IHealth
        spendable = provider as ISpendable;      // o ISpendable

        if (health != null)
        {
            onChangedHandler = (cur, max) => UpdateBar(cur, max);
            health.OnChanged += onChangedHandler;
            UpdateBar(health.Current, health.Max);
            return;
        }

        if (spendable != null)
        {
            onChangedHandler = (cur, max) => UpdateBar(cur, max);
            spendable.OnChanged += onChangedHandler;
            UpdateBar(spendable.Current, spendable.Max);
            return;
        }

        Debug.LogWarning($"{name}: Bind() recibió un provider que no implementa IHealth ni ISpendable.", this);
    }

    public void Unbind()
    {
        if (health != null && onChangedHandler != null) health.OnChanged -= onChangedHandler;
        if (spendable != null && onChangedHandler != null) spendable.OnChanged -= onChangedHandler;

        health = null;
        spendable = null;
        onChangedHandler = null;
    }

    private void OnDestroy() => Unbind();

    private void UpdateBar(float current, float max)
    {
        float pct = (max <= 0f) ? 0f : current / max;
        if (fill != null)
        {
            fill.fillAmount = Mathf.Clamp01(pct);
            if (useGradient && colorByPercent != null)
                fill.color = colorByPercent.Evaluate(pct);
            else
                fill.color = baseColor; // respeta el color del Image/base
        }

        if (valueLabel != null)
            valueLabel.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }
}
