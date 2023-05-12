using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerInput PlayerInput { get; private set; }

    public List<GameObject> interactions;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        interactions = new List<GameObject>();
    }

    public void SetPlayerMovement(PlayerMovement playerMovement)
    {
        this.PlayerMovement = playerMovement;
    }
    public void SetPlayerInput(PlayerInput playerInput)
    {
        this.PlayerInput = playerInput;
    }

    #region TriggerRegisterer
    public void AddInteractable(GameObject trigger)
    {
        interactions.Add(trigger);
    }

    public void RemoveInteractable(GameObject trigger)
    {
        interactions.Remove(trigger);
    }

    public bool IsInRangeOfInteraction()
    {
        return interactions.Count > 0;
    }

    public IInteractable GetCurrentInteraction()
    {
        if (!IsInRangeOfInteraction())
        {
            return null;
        }
        return interactions[^1].GetComponent<IInteractable>();
    }
    #endregion
}
