using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ZenUtils;

public class InputController : MonoBehaviour
{
    private PlayerController_to_deprecrate playerController;

    private PlayerInput playerInput;
    private InputAction comboAction;
    private InputAction modComboAction;
    private InputAction moveAction;
    private InputAction guardAction;

    private InputAction atack1Action;
    private InputAction atack2Action;
    private InputAction atack3Action;

    // Start is called before the first frame update
    void Awake()
    {
        playerController = GetComponent<PlayerController_to_deprecrate>();
        playerInput = GetComponent<PlayerInput>();

        comboAction = playerInput.actions["Combo"];
        modComboAction = playerInput.actions["ComboMod"];

        moveAction = playerInput.actions["Move"];

        guardAction = playerInput.actions["Guard"];

        atack1Action = playerInput.actions["Atack1"];
        atack2Action = playerInput.actions["Atack2"];
        atack3Action = playerInput.actions["Atack3"];

        atack1Action.started +=Atack1;
        atack2Action.started +=Combo;
        atack3Action.started +=Combo;

        comboAction.started += Combo;

        modComboAction.started += ModCombo;

        moveAction.performed += Move;
        moveAction.canceled += Move;

        guardAction.performed += Guard;
        guardAction.canceled += Guard;
    }

    // Update is called once per frame
    void Combo(InputAction.CallbackContext context)
    {
        //Debug.Log(context.control.shortDisplayName);

        Combo inputCombo = new Combo(Binding(context.control.shortDisplayName));
        //Debug.Log(inputCombo.id);

        playerController.Combo(inputCombo);
    }

    void ModCombo(InputAction.CallbackContext context)
    {
        //Debug.Log(context.control.shortDisplayName);
        ComboMod inputMod = new ComboMod(Binding(context.control.shortDisplayName));
        //Debug.Log(inputMod.id);

        playerController.ModCombo(inputMod);
    }

    void Guard(InputAction.CallbackContext context)
    {
        playerController.Fire(context.ReadValue<float>());
    }

    comboType Binding(string n)
    {
        switch (n)
        {
            case "RB":
                return comboType.atack1;
            case "X":
                return comboType.atack2;
            case "Y":
                return comboType.atack3;
            case "B":
                return comboType.buff;
            case "A":
                return comboType.dodge;
            case "D-Pad Left":
                return comboType.mod1;
            case "D-Pad Down":
                return comboType.mod2;
            case "D-Pad Up":
                return comboType.mod3;
            case "D-Pad Right":
                return comboType.mod4;
            default:
                return comboType.missClick;
        }
    }


    void Move(InputAction.CallbackContext context)
    {
        playerController.Move(context.ReadValue<Vector2>());
    }

    void Atack1(InputAction.CallbackContext context){
        playerController.atack1();
    }


}
