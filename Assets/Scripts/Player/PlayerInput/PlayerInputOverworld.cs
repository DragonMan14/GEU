using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInputOverworld : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerInputActions _inputActions;

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
        _inputActions = playerManager.PlayerInputManager.InputActions;
        EnableAllInput();
    }

    private void OnEnable()
    {
        if (_inputActions == null) 
        {
            return;
        }
        EnableAllInput();
    }

    private void OnDisable()
    {
        if (_inputActions == null)
        {
            return;
        }
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
        _inputActions.PlayerOverworld.Move.performed += MovePerformed;
        _inputActions.PlayerOverworld.Move.canceled += MoveCanceled;
    }

    private void DisableMovement()
    {
        _inputActions.PlayerOverworld.Move.performed -= MovePerformed;
        _inputActions.PlayerOverworld.Move.canceled -= MoveCanceled;
    }

    public void EnableInteract()
    {
        _inputActions.PlayerOverworld.Interact.performed += InteractPerformed;
    }

    public void DisableInteract()
    {
        _inputActions.PlayerOverworld.Interact.performed -= InteractPerformed;
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
