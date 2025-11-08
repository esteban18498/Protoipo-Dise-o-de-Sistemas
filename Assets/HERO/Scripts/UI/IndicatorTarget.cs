using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorTarget : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Target;
    public float speed = 10f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Target == null) return;
        //lerp
        this.transform.position = Vector3.Lerp(this.transform.position, Target.transform.position, Time.deltaTime * speed);
    }
}
