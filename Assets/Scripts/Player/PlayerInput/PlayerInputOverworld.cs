using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInputOverworld : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerInputActions inputActions;

    private void Start()
    {
        playerManager = PlayerManager.Instance;
        if (playerManager.PlayerInputManager.PlayerInputOverworld != null && playerManager.PlayerInputManager.PlayerInputOverworld != this)
        {
            Destroy(this);
        }
        else
        {
            playerManager.PlayerInputManager.PlayerInputOverworld = this;
        }
        inputActions = playerManager.PlayerInputManager.InputActions;
        EnableAllInput();
    }

    private void OnEnable()
    {
        if (inputActions == null) 
        {
            return;
        }
        EnableAllInput();
    }

    private void OnDisable()
    {
        DisableAllInput();
    }

    private void EnableAllInput()
    {
        EnableMovement();
        EnableInteract();
    }

    private void DisableAllInput()
    {
        DisableMovement();
        DisableInteract();
    }

    private void EnableMovement()
    {
        inputActions.PlayerOverworld.Move.performed += MovePerformed;
        inputActions.PlayerOverworld.Move.canceled += MoveCanceled;
    }

    private void DisableMovement()
    {
        inputActions.PlayerOverworld.Move.performed -= MovePerformed;
        inputActions.PlayerOverworld.Move.canceled -= MoveCanceled;
    }

    public void EnableInteract()
    {
        inputActions.PlayerOverworld.Interact.performed += InteractPerformed;
    }

    public void DisableInteract()
    {
        inputActions.PlayerOverworld.Interact.performed -= InteractPerformed;
    }
    private void MovePerformed(InputAction.CallbackContext obj)
    {
        Vector2 movement = obj.ReadValue<Vector2>();
        playerManager.PlayerMovementManager.PlayerMovementOverworld.SetMovementChange(movement);
    }

    private void MoveCanceled(InputAction.CallbackContext obj)
    {
        playerManager.PlayerMovementManager.PlayerMovementOverworld.SetMovementChange(Vector2.zero);
    }

    private void InteractPerformed(InputAction.CallbackContext obj)
    {
        IInteractable currentInteraction = PlayerManager.Instance.GetCurrentInteraction();
        if (playerManager.IsInRangeOfInteraction() && currentInteraction != null)
        {
            currentInteraction.Interact();
        }
    }
}
