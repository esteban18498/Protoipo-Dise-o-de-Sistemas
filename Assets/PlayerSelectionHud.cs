using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSelectionHud : MonoBehaviour
{

    public GameObject PlayerControllerPrefab;

    public RectTransform Slot1;
    public RectTransform Slot2;
    public RectTransform Slot3;

    public int count=0;

    public void JoinPlayer(PlayerInput playerInput)
    {
        RectTransform slot = null;
        Color color = Color.white;
        if (count == 0)
        {
            slot = Slot1;
            color = Color.red;
        }
        else if (count == 1)
        {
            slot = Slot2;
            color = Color.blue;
        }
        else if (count == 2)
        {
            slot = Slot3;
            color = Color.green;
        }
        else
        {
            Debug.Log("Maximum players reached.");
            return;
        }

        PlayerSideSelection player = Instantiate(PlayerControllerPrefab, slot, false)
                                        .GetComponent<PlayerSideSelection>();
        player.playerInput = playerInput;
        player.playerColor = color;
        player.gameObject.SetActive(true);
        count++;
    }   

}
