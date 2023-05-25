using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputBattleSystemMenu : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerInputActions _inputActions;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.Instance;
        if (playerManager.PlayerInputManager.PlayerInputBattleSystemMenu != null && playerManager.PlayerInputManager.PlayerInputBattleSystemMenu != this)
        {
            Destroy(this);
        }
        else
        {
            playerManager.PlayerInputManager.PlayerInputBattleSystemMenu = this;
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
        EnableSelection();
    }

    private void DisableAllInput()
    {
        DisableMovement();
        DisableSelection();
    }

    private void EnableMovement()
    {
        _inputActions.PlayerBattleSystemMenu.Move.performed += MovementPerformed;
    }

    private void DisableMovement()
    {
        _inputActions.PlayerBattleSystemMenu.Move.performed -= MovementPerformed;
    }

    private void EnableSelection()
    {
        _inputActions.PlayerBattleSystemMenu.Select.performed += SelectionPerformed;
    }

    private void DisableSelection()
    {
        _inputActions.PlayerBattleSystemMenu.Select.performed -= SelectionPerformed;
    }

    private void MovementPerformed(InputAction.CallbackContext obj)
    {
        float input = obj.ReadValue<float>();
        UIManager.Instance.UIBattleSystem.HandleBattleOptionsInput(input);
    }

    private void SelectionPerformed(InputAction.CallbackContext obj)
    {
        print("Selection");
        UIManager.Instance.UIBattleSystem.TestInteract();
    }
}

