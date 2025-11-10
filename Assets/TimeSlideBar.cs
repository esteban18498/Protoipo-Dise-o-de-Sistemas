using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlideBar : MonoBehaviour
{
    public Image fillImage;
    public float maxTime = 10.0f;
    private float currentTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            fillImage.fillAmount = currentTime / maxTime;
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
