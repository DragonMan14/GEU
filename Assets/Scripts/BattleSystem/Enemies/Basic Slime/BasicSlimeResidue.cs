using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSlimeResidue : MonoBehaviour
{
    private PlayerMovementBattleSystem PlayerMovementBattleSystem;

    [SerializeField] private float _playerSpeedMultiplier = -0.5f;
    [SerializeField] private float _explicitJumpSpeed = 1f;

    private void Start()
    {
        PlayerMovementBattleSystem = PlayerManager.Instance.PlayerMovementManager.PlayerMovementBattleSystem;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("PlayerCombat"))
        {
            return;
        }

        PlayerMovementBattleSystem.AddStatusEffectSource("BasicSlimeResidue");
        if (PlayerMovementBattleSystem.HasMoreThanOneStatusEffectSource("BasicSlimeResidue"))
        {
            return;
        }
        PlayerMovementBattleSystem.AdjustMovementSpeedMultiplier(_playerSpeedMultiplier);
        PlayerMovementBattleSystem.SetJumpSpeedExplicitly(_explicitJumpSpeed);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("PlayerCombat"))
        {
            return;
        }
        PlayerMovementBattleSystem.RemoveStatusEffectSource("BasicSlimeResidue");
        if (PlayerMovementBattleSystem.HasStatusEffectSource("BasicSlimeResidue"))
        {
            return;
        }
        PlayerMovementBattleSystem.AdjustMovementSpeedMultiplier(-_playerSpeedMultiplier);
        PlayerMovementBattleSystem.ResetExplicitJumpSpeed();
    }
}
