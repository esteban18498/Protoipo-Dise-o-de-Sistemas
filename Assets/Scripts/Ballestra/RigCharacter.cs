using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigCharacter : Bone
{
    // Start is called before the first frame update


    [SerializeField] public Bone Head;
    [SerializeField] public Bone Torso;
    [SerializeField] public Bone attackArm;
    [SerializeField] public Bone supportArm;
    [SerializeField] public Bone legs;

    void FixedUpdate()
    {
        //Bone Function
        MoveToAnchor();
    }



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
