using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCollitionDetection : MonoBehaviour
{

    [SerializeField] private NovaCharacterController character;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MoveCollider"))
        {
            character.MoveBackToLastSpot();
        }
    }
}
