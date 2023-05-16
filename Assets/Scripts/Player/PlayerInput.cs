using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerManager playerManager;

    [SerializeField] private InputActionReference Movement;
    [SerializeField] private InputActionReference Interaction;

    private void Start()
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
        if (playerManager.PlayerMovement.CurrentState == MovementState.DialogueOptionsMenu)
        {
            UIManager.Instance.HandleDialogueOptionsInput(inputValue.Get<Vector2>());
            return;
        }
        else if (UIManager.Instance.DialogueOpen)
        {
            return;
        }
        else if (playerManager.PlayerMovement.CurrentState == MovementState.Openworld)
        {
            print(inputValue.Get<Vector2>());
            playerManager.PlayerMovement.SetMovementChange(inputValue.Get<Vector2>());
        }
        else if (playerManager.PlayerMovement.CurrentState == MovementState.Battlesystem)
        {
            playerManager.PlayerMovement.SetMovementChange(inputValue.Get<Vector2>());
        }
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

    public bool MovementIsPressed()
    {
        return !Movement.action.ReadValue<Vector2>().Equals(Vector2.zero);
    }
    public bool MovementWasPressedThisFrame()
    {
        return Movement.action.triggered && !Movement.action.ReadValue<Vector2>().Equals(Vector2.zero);
    }

    public bool MovementWasReleasedThisFrame()
    {
        return Movement.action.triggered && Movement.action.ReadValue<Vector2>().Equals(Vector2.zero);
    }
    #pragma warning restore IDE0051 // Remove unused private members

}
