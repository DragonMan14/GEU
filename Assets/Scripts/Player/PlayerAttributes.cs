using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    [SerializeField] private float _maxHealth;

    [SerializeField] private float _maxMana;

    public float CurrentHealth { get; private set; }
    public float CurrentMana { get; private set; }

    // Affects bonus damage for phys and magic attacks.
    public float LuckStat;
    public float PhysicalAttackStat;
    public float MagicAttackStat;
    public float DefenseStat;
    // Boost dodge bonus damage and flee sucsess chance.
    public float AgilityStat;

    private void Awake()
    {
        CurrentHealth = _maxHealth;

    }

    public float GetMaxHealth()

    {
        return _maxHealth;
    }
}
