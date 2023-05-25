using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxMana;
    [SerializeField] private float _currentMana;

    // Affects bonus damage for phys and magic attacks.
    public float LuckStat;
    public float PhysicalAttackStat;
    public float MagicAttackStat;
    public float DefenseStat;
    // Boost dodge bonus damage and flee sucsess chance.
    public float AgilityStat;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _currentMana = _maxMana;
    }

    private void Start()
    {
        PlayerManager playerManager = PlayerManager.Instance;
        if (playerManager.PlayerAttributes != null && playerManager.PlayerAttributes != this)
        {
            Destroy(this);
        }
        else
        {
            playerManager.PlayerAttributes = this;
        }
    }

    public void DrainHealth(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0) {
            GameOver();
        }
    }

    public void DrainMana(float mana)
    {
        _currentMana -= mana;
    }

    private void GameOver()
    {
        print("You are dead!");
    }
}

