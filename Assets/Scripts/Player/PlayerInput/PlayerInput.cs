using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerManager playerManager;

    #pragma warning disable IDE0051 // Remove unused private members
    private void OnMove(InputValue inputValue)
    {
        if (playerManager.PlayerInputManager.CurrentState == InputState.Dialogue)
        {
            return;
        }
        else if (UIManager.Instance.DialogueOpen)
        {
            return;
        }
        else if (playerManager.PlayerInputManager.CurrentState == InputState.Openworld)
        {
            playerManager.PlayerMovementManager.PlayerMovementOverworld.SetMovementChange(inputValue.Get<Vector2>());
        }
        else if (playerManager.PlayerInputManager.CurrentState == InputState.BattleSystem)
        {
            playerManager.PlayerMovementManager.PlayerMovementBattleSystem.SetHorizontalMovementChange(inputValue.Get<Vector2>());
        }
    }

    private void OnInteract()
    {
        IInteractable currentInteraction = PlayerManager.Instance.GetCurrentInteraction();
        if (playerManager.IsInRangeOfInteraction() && currentInteraction != null) 
        {
            currentInteraction.Interact();
            
        }
    }

    #pragma warning restore IDE0051 // Remove unused private members

}
