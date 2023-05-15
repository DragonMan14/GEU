using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerManager playerManager;
    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.Instance;
        if (playerManager.PlayerInput != null && playerManager.PlayerInput != this)
        {
            Destroy(this);
        }
        else
        {
            playerManager.SetPlayerInput(this);
        }
    }

    #pragma warning disable IDE0051 // Remove unused private members
    private void OnMove(InputValue inputValue)
    {
        if (UIManager.Instance.DialogueOptionsOpen)
        {
            UIManager.Instance.HandleDialogueOptionsInput(inputValue.Get<Vector2>());
            return;
        }
        if (UIManager.Instance.DialogueOpen)
        {
            return;
        }
        playerManager.PlayerMovement.SetMovementChange(inputValue.Get<Vector2>());
    }

    private void OnInteract()
    {
        IInteractable currentInteraction = PlayerManager.Instance.GetCurrentInteraction();
        if (playerManager.IsInRangeOfInteraction() && currentInteraction != null) 
        {
            currentInteraction.Interact();
            playerManager.PlayerMovement.SetMovementChange(Vector2.zero);
        }
    }
    #pragma warning restore IDE0051 // Remove unused private members

}
