using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInputBattleSystem : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerInputActions inputActions;

    private void Start()
    {
        playerManager = PlayerManager.Instance;
        if (playerManager.PlayerInputManager.PlayerInputBattleSystem != null && playerManager.PlayerInputManager.PlayerInputBattleSystem != this)
        {
            Destroy(this);
        }
        else
        {
            playerManager.PlayerInputManager.PlayerInputBattleSystem = this;
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
        EnableJump();
    }

    private void DisableAllInput()
    {
        DisableMovement();
        DisableJump();
    }

    private void EnableMovement()
    {
        inputActions.PlayerBattleSystem.Move.performed += MovePerformed;
        inputActions.PlayerBattleSystem.Move.canceled += MoveCanceled;
    }

    private void DisableMovement()
    {
        inputActions.PlayerBattleSystem.Move.performed -= MovePerformed;
        inputActions.PlayerBattleSystem.Move.canceled -= MoveCanceled;
    }

    public void EnableJump()
    {
        inputActions.PlayerBattleSystem.Jump.performed += JumpPerformed;
        inputActions.PlayerBattleSystem.Jump.canceled += JumpCanceled;
    }

    public void DisableJump()
    {
        inputActions.PlayerBattleSystem.Jump.performed -= JumpPerformed;
        inputActions.PlayerBattleSystem.Jump.canceled -= JumpCanceled;
    }
    private void MovePerformed(InputAction.CallbackContext obj)
    {
        float direction = obj.ReadValue<float>();
        Vector2 movement = Vector2.zero;
        if (direction < 0)
        {
            movement = Vector2.left;
        }
        else if (direction > 0) 
        {
            movement = Vector2.right;
        }
        playerManager.PlayerMovementManager.PlayerMovementBattleSystem.SetHorizontalMovementChange(movement);
    }

    private void MoveCanceled(InputAction.CallbackContext obj)
    {
        playerManager.PlayerMovementManager.PlayerMovementBattleSystem.SetHorizontalMovementChange(Vector2.zero);
    }

    private void JumpPerformed(InputAction.CallbackContext obj)
    {
        playerManager.PlayerMovementManager.PlayerMovementBattleSystem.JumpPressed = true;
        playerManager.PlayerMovementManager.PlayerMovementBattleSystem.PerformJump();
    }

    private void JumpCanceled(InputAction.CallbackContext obj)
    {
        playerManager.PlayerMovementManager.PlayerMovementBattleSystem.JumpPressed = false;
    }
}
