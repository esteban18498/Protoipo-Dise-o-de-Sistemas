using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : MonoBehaviour
{
    public Transform Anchor;
    public float speed = 5f;

    public bool rotates = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (Anchor == null)
        {
            throw new System.Exception("Anchor is not set. Please assign a Transform in the inspector.");
        }
        else
        {
            this.transform.position = Anchor.transform.position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Bone Function
        MoveToAnchor();
    }

    public void MoveToAnchor()
    {
        this.transform.position = Vector3.Lerp(
            this.transform.position,
            Anchor.position,
            Time.deltaTime * speed
        );

        //rotate to anchor
        if (rotates)
        {
            this.transform.rotation = Quaternion.Slerp(
                this.transform.rotation,
                Anchor.rotation,
                Time.deltaTime * speed*1.5f
            );
        }

    }
}
