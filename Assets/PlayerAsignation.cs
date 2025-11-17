using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAsignation : MonoBehaviour
{
    //singleton 
    public static PlayerAsignation instance;

    public PlayerInput player1;
    public PlayerInput player2;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void ResetPlayers()
    {
        player1 = null;
        player2 = null;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
