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
        if (arenaTimeValue != null)
            arenaTimeValue.text = $"{Mathf.CeilToInt(currentTime)} / {Mathf.CeilToInt(maxTime)}";
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            fillImage.fillAmount = currentTime / maxTime;
            arenaTimeValue.text = $"{Mathf.CeilToInt(currentTime)} / {Mathf.CeilToInt(maxTime)}";
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

    
}
