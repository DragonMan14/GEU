using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public PlayerMovementManager PlayerMovementManager;
    public PlayerInputManager PlayerInputManager;
    public List<GameObject> interactions;

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
