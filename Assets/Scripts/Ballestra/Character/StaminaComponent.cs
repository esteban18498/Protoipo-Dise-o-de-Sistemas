using UnityEngine;
using System;

public interface ISpendable
{
    float Current { get; }
    float Max { get; }
    event Action<float,float> OnChanged;
    bool CanSpend(float cost);
    bool TrySpend(float cost);
    void Refill(float amount);
}


[DisallowMultipleComponent]
public class StaminaComponent : MonoBehaviour, ISpendable
{
    [SerializeField] private float max = 100;
    [SerializeField] private float current = 100;
    [SerializeField] private float regenPerSecond = 15f;

    public float Current => current;
    public float Max => max;
    public event Action<float, float> OnChanged;

    public bool CanSpend(float cost) => current >= cost;
    public bool TrySpend(float cost)
    {
        if (!CanSpend(cost)) return false;
        current -= cost;
        OnChanged?.Invoke(current, max);
        return true;
    }

    public void Refill(float amount)
    {
        current = Mathf.Min(current + amount, max);
        OnChanged?.Invoke(current, max);
    }

    void Update()
    {
        // Opcional: limitar regen por estado de tu FSM si lo deseás
        if (regenPerSecond > 0 && current < max)
        {
            current = Mathf.Min(current + regenPerSecond * Time.deltaTime, max);
            OnChanged?.Invoke(current, max);
        }
    }
}