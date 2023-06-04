using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputState
{
    Openworld,
    BattleSystem,
    Dialogue
}

public class PlayerInputManager : MonoBehaviour
{
    private PlayerManager playerManager;

    public PlayerInputOverworld PlayerInputOverworld;
    public PlayerInputBattleSystem PlayerInputBattleSystem;
    public PlayerInputDialogue PlayerInputDialogue;

    public InputState PreviousState;
    public InputState CurrentState;

    public PlayerInputActions InputActions;

    private void Awake()
    {
        InputActions = new PlayerInputActions();
    }

    private void Start()
    {
        playerManager = PlayerManager.Instance;
        if (playerManager.PlayerInputManager != null && playerManager.PlayerInputManager != this)
        {
            Destroy(this);
        }
        else
        {
            playerManager.PlayerInputManager = this;
        }
        ToggleActionMap(CurrentState);
    }

    public void SetInputState(InputState state)
    {
        if (CurrentState == state)
        {
            return;
        }
        PreviousState = CurrentState;
        CurrentState = state;
        ToggleActionMap(state);
    }
    
    public void ToggleActionMap(InputState state)
    {
        if (state == InputState.Openworld)
        {
            playerManager.PlayerOverworld.SetActive(true);
            playerManager.PlayerCombat.SetActive(false);
            ToggleActionMap(InputActions.PlayerOverworld);
        }
        else if (state == InputState.BattleSystem)
        {
            playerManager.PlayerOverworld.SetActive(false);
            playerManager.PlayerCombat.SetActive(true);
            ToggleActionMap(InputActions.PlayerBattleSystem);
        }
        else if (state == InputState.Dialogue)
        {
            ToggleActionMap(InputActions.Dialogue);
        }
    }

    public void ToggleActionMap(InputActionMap actionMap)
    {
        StartCoroutine(ToggleActionMapDelay(actionMap));
    }

    private IEnumerator ToggleActionMapDelay(InputActionMap actionMap)
    {
        if (actionMap.enabled)
        {
            yield break;
        }
        yield return new WaitForSeconds(0.25f);
        InputActions.Disable();
        actionMap.Enable();
    }
}
