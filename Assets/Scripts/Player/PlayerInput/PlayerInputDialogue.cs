using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputDialogue : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerInputActions _inputActions;

    private void Start()
    {
        playerManager = PlayerManager.Instance;
        if (playerManager.PlayerInputManager.PlayerInputDialogue != null && playerManager.PlayerInputManager.PlayerInputDialogue != this)
        {
            Destroy(this);
        }
        else
        {
            playerManager.PlayerInputManager.PlayerInputDialogue = this;
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
        EnableProgressDialogue();
    }

    private void DisableAllInput()
    {
        DisableMovement();
        DisableProgressDialogue();
    }

    private void EnableMovement()
    {
        _inputActions.Dialogue.Move.performed += MovePerformed;
    }

    private void DisableMovement()
    {
        _inputActions.Dialogue.Move.performed -= MovePerformed;
    }

    public void EnableProgressDialogue()
    {
        _inputActions.Dialogue.ProgressDialogue.performed += ProgressDialoguePerformed;
    }

    public void DisableProgressDialogue()
    {
        _inputActions.Dialogue.ProgressDialogue.performed -= ProgressDialoguePerformed;
    }
    private void MovePerformed(InputAction.CallbackContext obj)
    {
        UIManager.Instance.DialogueManager.HandleDialogueOptionsInput(obj.ReadValue<float>()); 
    }

    public void ProgressDialoguePerformed(InputAction.CallbackContext obj)
    {
        IInteractable currentInteraction = PlayerManager.Instance.GetCurrentInteraction();
        if (playerManager.IsInRangeOfInteraction() && currentInteraction != null)
        {
            currentInteraction.Interact();

        }
    }
}
