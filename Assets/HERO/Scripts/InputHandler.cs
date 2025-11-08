using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public HERO hero;
    private SequenceManager sequenceManager;


    private PlayerInput playerInput;
    private InputAction directionalAction;
    private InputAction attack1Action;
    private InputAction attack2Action;
    private InputAction blockAction;
    private InputAction pauseAction;


    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        directionalAction = playerInput.actions["Directional"];
        directionalAction.performed += DirectionalAction;
        directionalAction.canceled += DirectionalAction;

        attack1Action = playerInput.actions["Attack1"];
        attack1Action.started += Attack1Action;

        attack2Action = playerInput.actions["Attack2"];
        attack2Action.started += Attack2Action;

        blockAction = playerInput.actions["Block"];
        blockAction.started += BlockAction;

        pauseAction = playerInput.actions["Pause"];
        pauseAction.started += OnPausePressed;
    }

    void Start()
    {
        if (hero == null)
        {
            Debug.LogError("hero reference not set in InputHandler.");
            this.enabled = false;
            return;
        } else
        {
            sequenceManager = hero.sequenceManager;
        }

    }

    private void OnDestroy()
    {
        // Always unsubscribe
        directionalAction.performed -= DirectionalAction;
        directionalAction.canceled -= DirectionalAction;
        attack1Action.started -= Attack1Action;
        attack2Action.started -= Attack2Action;
        blockAction.started -= BlockAction;
        if (pauseAction != null) pauseAction.started -= OnPausePressed;
    }

    void DirectionalAction(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().x < 0)
        {
            sequenceManager.HandlePlayerInput(InputStep.LEFT);
        }
        if (context.ReadValue<Vector2>().x > 0)
        {
            sequenceManager.HandlePlayerInput(InputStep.RIGHT);
        }
        if (context.ReadValue<Vector2>().y < 0)
        {
            sequenceManager.HandlePlayerInput(InputStep.DOWN);
        }
        if (context.ReadValue<Vector2>().y > 0)
        {
            sequenceManager.HandlePlayerInput(InputStep.UP);
        }
    }

    void Attack1Action(InputAction.CallbackContext context)
    {
        sequenceManager.HandlePlayerInput(InputStep.ATK1);

    }

    void Attack2Action(InputAction.CallbackContext context)
    {
        sequenceManager.HandlePlayerInput(InputStep.ATK2);
    }

    void BlockAction(InputAction.CallbackContext context)
    {
        hero.BlockAnimation();
       // Debug.Log("block buton");
    }

    void OnPausePressed(InputAction.CallbackContext context)
    {
        // Handle pause action if needed
        // scene manager?
    }
}