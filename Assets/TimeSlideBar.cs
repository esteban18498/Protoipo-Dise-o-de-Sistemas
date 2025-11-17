using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlideBar : MonoBehaviour
{
    [Header("Wiring")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text arenaTimeValue;

    [Header("Timer Settings")]
    [Tooltip("Current maximum time of the arena timer (seconds).")]
    [SerializeField] private float maxTime = 10.0f;
    private float currentTime;

    [Header("Dynamic Time (HP-based)")]
    [Tooltip("Minimum arena time when players are low HP.")]
    [SerializeField] private float minArenaTime = 2f;
    [Tooltip("Maximum arena time when players are high HP.")]
    [SerializeField] private float maxArenaTime = 5f;

    // Thresholds expressed as 0–1 of total HP
    private const float UpperHpThreshold = 0.9f; // >= 90% → 5s
    private const float LowerHpThreshold = 0.3f; // <= 30% → 2s

    [Header("Fill Colors")]
    [Tooltip("Color when there is more than 50% time left.")]
    [SerializeField] private Color highTimeColor = Color.white;
    [Tooltip("Color when there is between 50% and 25% time left.")]
    [SerializeField] private Color midTimeColor = new Color(243f / 255f, 194f / 255f, 60f / 255f); // #F3C23C
    [Tooltip("Color when there is 25% or less time left.")]
    [SerializeField] private Color lowTimeColor = new Color(230f / 255f, 39f / 255f, 103f / 255f);  // #E62767

    private void Start()
    {
        currentTime = maxTime;
        UpdateFillVisuals();
        UpdateText();
    }

    private void Update()
    {
        if (currentTime > 0f)
        {
            currentTime -= Time.deltaTime;

            if (currentTime < 0f)
                currentTime = 0f;

            UpdateFillVisuals();
            UpdateText();
        }
    }

    /// <summary>
    /// Starts the timer using the current maxTime.
    /// </summary>
    public void StartTimer()
    {
        currentTime = maxTime;
        UpdateFillVisuals();
        UpdateText();
    }

    /// <summary>
    /// Starts the timer with an explicit duration in seconds.
    /// </summary>
    public void StartTimer(float time)
    {
        maxTime = time;
        currentTime = maxTime;
        UpdateFillVisuals();
        UpdateText();
    }

    /// <summary>
    /// Pure-ish function that maps the sum of both heroes' HP to a turn time.
    ///  - >= 90% total HP  => maxArenaTime (default 5s)
    ///  - <= 30% total HP  => minArenaTime (default 2s)
    ///  - Linear interpolation between those thresholds.
    /// </summary>
    /// <param name="currentHpSum">Sum of current HP of both players.</param>
    /// <param name="maxHpSum">Sum of max HP of both players.</param>
    public float CalculateArenaTimeFromHealth(float currentHpSum, float maxHpSum)
    {
        if (maxHpSum <= 0f)
            return maxArenaTime; // fallback: avoid divide by zero

        float hpRatio = Mathf.Clamp01(currentHpSum / maxHpSum); // 0..1

        // Ceiling: very healthy fight → longest time
        if (hpRatio >= UpperHpThreshold)
            return maxArenaTime;

        // Floor: both are almost dead → shortest time
        if (hpRatio <= LowerHpThreshold)
            return minArenaTime;

        // Between 90% and 30% → lerp 5s → 2s linearly
        float t = Mathf.InverseLerp(UpperHpThreshold, LowerHpThreshold, hpRatio);
        return Mathf.Lerp(maxArenaTime, minArenaTime, t);
    }

    /// <summary>
    /// Updates the UI image fill and color based on current time.
    /// </summary>
    private void UpdateFillVisuals()
    {
        if (fillImage == null || maxTime <= 0f)
            return;

        float pct = Mathf.Clamp01(currentTime / maxTime);
        fillImage.fillAmount = pct;

        if (pct <= 0.25f)
            fillImage.color = lowTimeColor;      // Red at <= 25%
        else if (pct <= 0.5f)
            fillImage.color = midTimeColor;      // Yellow between 25–50%
        else
            fillImage.color = highTimeColor;     // Default above 50%
    }

    /// <summary>
    /// Updates the SS:DD label.
    /// </summary>
    private void UpdateText()
    {
        // Whole seconds left
        int seconds = Mathf.FloorToInt(currentTime);

        // Decimal part (hundredths)
        int decimals = Mathf.FloorToInt((currentTime - seconds) * 100f);

        // Format: 00:00 (SS:DD)
        if (arenaTimeValue != null)
            arenaTimeValue.text = $"{seconds:00}:{decimals:00}";
    }
}
