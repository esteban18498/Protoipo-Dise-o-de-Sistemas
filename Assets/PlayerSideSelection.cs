using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSideSelection : MonoBehaviour
{
    public Color playerColor;
    public Image playerIndicator;

    public PlayerInput playerInput;
    private InputAction directionalAction;
    private InputAction confirmAction;


    private RectTransform rectTransform;
    private float distanceFromCenter = 300f;

    public enum SelectedSide
    {
        None,
        Left,
        Right
    }

    public SelectedSide selectedSide = SelectedSide.None;

    void Start()
    {

        directionalAction = playerInput.actions["Directional"];
        directionalAction.started += DirectionalAction;

        confirmAction = playerInput.actions["Attack1"];
        confirmAction.started += ConfirmAction;

        playerIndicator.color = playerColor;




        rectTransform = GetComponent<RectTransform>();


    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnDestroy()
    {
        // Always unsubscribe
        directionalAction.started -= DirectionalAction;
        confirmAction.started -= ConfirmAction;
    }

    void DirectionalAction(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();

        if (inputVector.x < 0)
        {
            if (selectedSide == SelectedSide.Right)
            {
                selectedSide = SelectedSide.None;
            }
            else
            {
                selectedSide = SelectedSide.Left;
            }
        }
        else if (inputVector.x > 0)
        {
            if (selectedSide == SelectedSide.Left)
            {
                selectedSide = SelectedSide.None;
            }
            else
            {
                selectedSide = SelectedSide.Right;
            }
        }

        switch (selectedSide)
        {

            case SelectedSide.None:
                rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
                break;
            case SelectedSide.Left:
                rectTransform.anchoredPosition = new Vector2(-distanceFromCenter, rectTransform.anchoredPosition.y);
                break;
            case SelectedSide.Right:
                rectTransform.anchoredPosition = new Vector2(distanceFromCenter, rectTransform.anchoredPosition.y);
                break;

        }

    }

    void ConfirmAction(InputAction.CallbackContext context)
    {
        if (selectedSide == SelectedSide.None)
        {
            return;
        }

        bool selectionSuccess = false;
        if (selectedSide == SelectedSide.Left)
        {
            selectionSuccess = multicontrol_manager.instance.selectP1(this.playerInput);
        }
        else if (selectedSide == SelectedSide.Right)
        {
            selectionSuccess = multicontrol_manager.instance.selectP2(this.playerInput);
        }

        if (selectionSuccess)
        {
            playerIndicator.color = playerColor * 0.5f + Color.black * 0.5f;
            // Disable further input
            directionalAction.started -= DirectionalAction;
            confirmAction.started -= ConfirmAction;
        }
        else
        {
            Debug.Log("Side already taken. Player " + playerInput.playerIndex + " could not select side: " + selectedSide);
        }

    }


}
