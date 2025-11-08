using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordColliderBind : MonoBehaviour
{
    public Sword sword;
    // Start is called before the first frame update
    void Start()
    {
        if (sword == null)
        {
            sword = this.GetComponentInParent<Sword>();
        }
    }

    // Update is called once per frame

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        sword.triggerColliderEnter(collision);
    }
}
