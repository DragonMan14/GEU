using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputState
{
    Openworld,
    BattleSystem,
    Dialogue,
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

        if (CurrentState == InputState.Openworld)
        {
            ToggleActionMap(InputActions.PlayerOverworld);
        }
        else if (CurrentState == InputState.BattleSystem)
        {
            ToggleActionMap(InputActions.PlayerBattleSystem);
        }
    }

    public void SetInputState(InputState state)
    {
        if (CurrentState == state)
        {
            return;
        }
        PreviousState = CurrentState;
        CurrentState = state;
        if (state == InputState.Openworld)
        {
            ToggleActionMap(InputActions.PlayerOverworld);
        }
        else if (state == InputState.BattleSystem)
        {
            ToggleActionMap(InputActions.PlayerBattleSystem);
        }
        else if (state == InputState.Dialogue) {
            ToggleActionMap(InputActions.Dialogue);
        }
    }

    public void ToggleActionMap(InputActionMap actionMap)
    {
        if (actionMap.enabled)
        {
            return;
        }
        InputActions.Disable();
        actionMap.Enable();
        print(actionMap.ToString());
    }
}
