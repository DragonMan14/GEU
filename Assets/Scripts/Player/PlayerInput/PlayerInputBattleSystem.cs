using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInputBattleSystem : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerInputActions _inputActions;

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
        EnableJump();
        EnableNormalPhysicalAttack();
    }

    private void DisableAllInput()
    {
        DisableMovement();
        DisableJump();
        DisableNormalPhysicalAttack();
    }

    private void EnableMovement()
    {
        _inputActions.PlayerBattleSystem.Move.performed += MovePerformed;
        _inputActions.PlayerBattleSystem.Move.canceled += MoveCanceled;
    }

    private void DisableMovement()
    {
        _inputActions.PlayerBattleSystem.Move.performed -= MovePerformed;
        _inputActions.PlayerBattleSystem.Move.canceled -= MoveCanceled;
    }

    public void EnableJump()
    {
        _inputActions.PlayerBattleSystem.Jump.performed += JumpPerformed;
        _inputActions.PlayerBattleSystem.Jump.canceled += JumpCanceled;
    }

    public void DisableJump()
    {
        _inputActions.PlayerBattleSystem.Jump.performed -= JumpPerformed;
        _inputActions.PlayerBattleSystem.Jump.canceled -= JumpCanceled;
    }

    public void EnableNormalPhysicalAttack()
    {
        _inputActions.PlayerBattleSystem.NormalPhysicalAttack.performed += NormalPhysicalAttackPerformed;
    }

    public void DisableNormalPhysicalAttack()
    {
        _inputActions.PlayerBattleSystem.NormalPhysicalAttack.performed -= NormalPhysicalAttackPerformed;
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

    private void NormalPhysicalAttackPerformed(InputAction.CallbackContext obj)
    {
        playerManager.PlayerMovementManager.PlayerMovementBattleSystem.PerformNormalPhysicalAttack();
    }
}
