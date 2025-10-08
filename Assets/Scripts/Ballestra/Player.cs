using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] public NovaCharacterController CharacterController;

    private PlayerInput playerInput;
    private InputAction directionalAction;
    private InputAction freezAction;
    private InputAction moveAction;
    private InputAction attackAction;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        directionalAction = playerInput.actions["Directional"];
        directionalAction.performed += DirectionalAction;
        directionalAction.canceled += DirectionalAction;

        freezAction = playerInput.actions["Freez"];
        freezAction.started += Freez;

        moveAction = playerInput.actions["Move"];
        moveAction.started += MoveAction;
        
        attackAction = playerInput.actions["Attack"];
        attackAction.started += AttackAction;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void DirectionalAction(InputAction.CallbackContext context)
    {
        if (CharacterController.combat_state == Combat_state.free_move)
        {

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

    void Freez(InputAction.CallbackContext context)
    {
        CharacterController.RequestFreezState();
    }

    void MoveAction(InputAction.CallbackContext context)
    {
        if (CharacterController.combat_state == Combat_state.freez)
        {
            //check for mods? here?
            CharacterController.actionQueue.EnqueueAction(new Action_Advance(CharacterController));
        }
    }
    
    void AttackAction(InputAction.CallbackContext context)
    {
        if (CharacterController.combat_state == Combat_state.freez)
        {
            //check for mods? here?
            CharacterController.actionQueue.EnqueueAction(new Action_Attack(CharacterController));
        }
    }
}
