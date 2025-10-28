using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] public NovaCharacterController CharacterController;

    private PlayerInput playerInput;
    private InputAction directionalAction;
    private InputAction freezAction;
    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction blockAction;
    private InputAction pauseAction;

    public ListKey<Combat_Action_mod> Mods;
    public Action OnModsUpdated;

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

        blockAction = playerInput.actions["Block"];
        blockAction.started += BlockAction;

        pauseAction = playerInput.actions["Pause"];
        pauseAction.started += OnPausePressed;

        Mods = new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>());

    }

    void Start()
    {
        CharacterController.OnFreezEnd += OnFreezEnd;
    }

    #region pause
    // THIS SHOULD BE CHANGED ONCE WE GET A REAL PAUSE MENU
    private void OnDestroy()
    {
        // Always unsubscribe
        directionalAction.performed -= DirectionalAction;
        directionalAction.canceled -= DirectionalAction;
        freezAction.started -= Freez;
        moveAction.started -= MoveAction;
        attackAction.started -= AttackAction;
        blockAction.started -= BlockAction;
        if (pauseAction != null) pauseAction.started -= OnPausePressed;
    }
    #endregion

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
            // e.g., if context.ReadValue<Vector2>().y > 0 then add Combat_Action_mod.Up to the current action's mod list

            if (context.ReadValue<Vector2>().x < 0)
            {
                if (CharacterController.transform.localScale.x > 0)// facing right
                {
                    Mods.add(Combat_Action_mod.Back);
                }
                else
                {
                    Mods.add(Combat_Action_mod.Front);
                }
            }
            else if (context.ReadValue<Vector2>().x > 0)
            {
                if (CharacterController.transform.localScale.x > 0)// facing right
                {
                    Mods.add(Combat_Action_mod.Front);
                }
                else
                {
                    Mods.add(Combat_Action_mod.Back);
                }
            }

            if (context.ReadValue<Vector2>().y < 0)
            {
                Mods.add(Combat_Action_mod.Down);
            }
            else if (context.ReadValue<Vector2>().y > 0)
            {
                Mods.add(Combat_Action_mod.Up);
            }

            OnModsUpdated?.Invoke();

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
            ICombatAction action = CharacterController.CombatActionDictionary.GetCombatAction(Mods, Combat_Action_Type.Move);
            CharacterController.actionQueue.EnqueueAction(action);

            Mods = new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>()); // reset mods after enqueuing action
            OnModsUpdated?.Invoke();
        }
    }

    void AttackAction(InputAction.CallbackContext context)
    {
        if (CharacterController.combat_state == Combat_state.freez)
        {
            ICombatAction action = CharacterController.CombatActionDictionary.GetCombatAction(Mods, Combat_Action_Type.Attack);
            CharacterController.actionQueue.EnqueueAction(action);

            Mods = new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>()); // reset mods after enqueuing action
            OnModsUpdated?.Invoke();
        }
    }

    void BlockAction(InputAction.CallbackContext context)
    {
        if (CharacterController.combat_state == Combat_state.freez)
        {
            ICombatAction action = CharacterController.CombatActionDictionary.GetCombatAction(Mods, Combat_Action_Type.Block);
            CharacterController.actionQueue.EnqueueAction(action);

            Mods = new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>()); // reset mods after enqueuing action
            OnModsUpdated?.Invoke();
        }
    }


    public void OnFreezEnd()
    {
        Mods = new ListKey<Combat_Action_mod>(new List<Combat_Action_mod>());
        OnModsUpdated?.Invoke();
    }

    // -------------------- PAUSE MENU --------------------

    private void OnPausePressed(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene("MainMenu");
    }
}
