using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] public NovaCharacterController CharacterController;

    private PlayerInput playerInput;
    private InputAction moveAction;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];

        moveAction.performed += Move;
        moveAction.canceled += Move;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Move(InputAction.CallbackContext context)
    {

        // check flip side
        // player is allways facing right

        if (context.ReadValue<Vector2>().x < 0)
        {
            CharacterController.MoveToPreviousSpot();
        }
        else if (context.ReadValue<Vector2>().x > 0)
        {
            // normal side
            CharacterController.MoveToNextSpot();
        }
    }
}
