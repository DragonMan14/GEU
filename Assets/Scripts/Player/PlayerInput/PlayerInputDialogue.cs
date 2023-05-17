using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputDialogue : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerInputActions inputActions;

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
        EnableProgressDialogue();
    }

    private void DisableAllInput()
    {
        DisableMovement();
        DisableProgressDialogue();
    }

    private void EnableMovement()
    {
        inputActions.Dialogue.Move.performed += MovePerformed;
    }

    private void DisableMovement()
    {
        inputActions.Dialogue.Move.performed -= MovePerformed;
    }

    public void EnableProgressDialogue()
    {
        inputActions.Dialogue.ProgressDialogue.performed += ProgressDialoguePerformed;
    }

    public void DisableProgressDialogue()
    {
        inputActions.Dialogue.ProgressDialogue.performed -= ProgressDialoguePerformed;
    }
    private void MovePerformed(InputAction.CallbackContext obj)
    {
        print("test");
        if (UIManager.Instance.DialogueOptionsOpen)
        {
            UIManager.Instance.HandleDialogueOptionsInput(obj.ReadValue<float>());
        }
    }

    public void ProgressDialoguePerformed(InputAction.CallbackContext obj)
    {
        print("Hi");
        IInteractable currentInteraction = PlayerManager.Instance.GetCurrentInteraction();
        if (playerManager.IsInRangeOfInteraction() && currentInteraction != null)
        {
            currentInteraction.Interact();

        }
    }
}
