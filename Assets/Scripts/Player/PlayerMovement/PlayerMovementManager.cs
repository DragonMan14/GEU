using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    private PlayerManager playerManager;

    public PlayerMovementOverworld PlayerMovementOverworld;
    public PlayerMovementBattleSystem PlayerMovementBattleSystem;

    private void Start()
    {
        playerManager = PlayerManager.Instance;
        if (playerManager.PlayerMovementManager != null && playerManager.PlayerMovementManager != this)
        {
            Destroy(this);
        }
        else
        {
            playerManager.PlayerMovementManager = this;
        }
    }
}
