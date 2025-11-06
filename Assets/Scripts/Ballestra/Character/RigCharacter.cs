using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigCharacter : Bone
{

    public Bone Head;
    public Bone Torso;
    public Bone Sword;
    public Bone supportArm;
    public Bone legs;

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
