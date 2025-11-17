using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class multicontrol_manager : MonoBehaviour
{
    //singleton 
    public static multicontrol_manager instance;

    PlayerInputManager playerInputManager;

    public PlayerSelectionHud PlayerSelectionHud;

    PlayerInput player1;
    PlayerInput player2;

    List<PlayerSideSelection> Controllers = new List<PlayerSideSelection>();


    // Start is called before the first frame update
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

        if (PlayerAsignation.instance != null)
        {
            PlayerAsignation.instance.ResetPlayers();
        }

        playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.onPlayerJoined += OnPlayerJoined;
        player1 = null;
        player2 = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnPlayerJoined(PlayerInput playerInput)
    {

        playerInput.gameObject.name = "Player_" + (playerInput.playerIndex + 1);

        PlayerSelectionHud.JoinPlayer(playerInput);



    }


    public bool selectP1(PlayerInput player)
    {
        if (player1 == null)
        {
            player1 = player;
            StartCoroutine(CheckToStart(1.0f));
            return true;
        }
        return false;
    }
    public bool selectP2(PlayerInput player)
    {
        if (player2 == null)
        {
            player2 = player;
            StartCoroutine(CheckToStart(1.0f));
            return true;
        }
        return false;
    }


    IEnumerator CheckToStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (player1 != null && player2 != null)
        {
            Debug.Log("Both players selected. Starting game...");
            PlayerAsignation.instance.player1 = player1;
            PlayerAsignation.instance.player2 = player2;

            player1.transform.SetParent(PlayerAsignation.instance.transform);
            player2.transform.SetParent(PlayerAsignation.instance.transform);

            SceneManager.LoadScene("Prototype");
            // Here you can add code to transition to the game scene or start the game logic.
        }

    }
}
