using System;
using System.Collections;
using UnityEngine;
public interface IHealth
{
    float Current { get; }
    float Max { get; }

    bool IsDead { get; }

    event Action<float,float> OnChanged;   // current, max
    event Action OnDeath;

    void ApplyDamage(float amount);
    void Heal(float amount);
    void Kill();       // utilidad p/test, trampas, etc.
}

[DisallowMultipleComponent]
public class HealthComponent : MonoBehaviour, IHealth
{
    [SerializeField] private float max = 100f;
    [SerializeField] private float current = 100f;

    public float Current => current;
    public float Max => max;
    public bool IsDead => current <= 0f;

    public event Action<float,float> OnChanged;   // current, max
    public event Action OnDeath;

    public void ApplyDamage(float amount)
    {
        if (IsDead) return;

        current = Mathf.Max(0f, current - amount);
        OnChanged?.Invoke(amount, max);

        if (current <= 0f) OnDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || IsDead) return;

        float before = current;
        current += amount;
        current = Mathf.Min(current, max); // clamp to max

        float healed = current - before;
        OnChanged?.Invoke(current, max);
    }

    public void Kill()
    {
        if (IsDead) return;
        current = 0f;
        OnChanged?.Invoke(current, max);
        OnDeath?.Invoke();
    }
}