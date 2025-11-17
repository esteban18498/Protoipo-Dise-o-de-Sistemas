using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlideBar : MonoBehaviour
{
    public Image fillImage;
    [SerializeField] private TMP_Text arenaTimeValue;
    public float maxTime = 10.0f;
    private float currentTime;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = maxTime;
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (currentTime < 0)
                currentTime = 0;

            fillImage.fillAmount = currentTime / maxTime;

            UpdateText();
        }   
    }

    public void StartTimer()
    {
        currentTime = maxTime;

    }
    public void StartTimer(float time)
    {
        maxTime = time;
        currentTime = maxTime;

    }

    private void UpdateText()
    {
        // Whole seconds left
        int seconds = Mathf.FloorToInt(currentTime);

        // Decimal part (hundredths)
        int decimals = Mathf.FloorToInt((currentTime - seconds) * 100f);

        // Format: 00:00
        arenaTimeValue.text = $"{seconds:00}:{decimals:00}";
    }

}
