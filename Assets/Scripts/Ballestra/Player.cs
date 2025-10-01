using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] public NovaCharacterController CharacterController;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction freezAction;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        
        moveAction = playerInput.actions["Move"];
        moveAction.performed += Move;
        moveAction.canceled += Move;

        freezAction = playerInput.actions["Freez"];
        freezAction.started += Freez;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Move(InputAction.CallbackContext context)
    {
        if (CharacterController.combat_state == Combat_state.free_move) {

            if (context.ReadValue<Vector2>().x < 0)
            {
                CharacterController.MoveToPreviousSpot();
            }
            else if (context.ReadValue<Vector2>().x > 0)
            {
                CharacterController.MoveToNextSpot();
            }
        }

        if (CharacterController.combat_state == Combat_state.freez)
        {
            // load combo/action modificators
        }
    }

    void Freez(InputAction.CallbackContext context) { 
        CharacterController.RequestFreezState();
    }
}
