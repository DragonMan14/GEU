using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private List<GameObject> _enemyPrefabs;
    public List<GameObject> testing;

    private void Awake()
    {
        _enemyPrefabs = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            print("Testing");
            SetEnemies(testing);
            StartCoroutine(StartBattle());
        }

    }

    public void SetEnemies(List<GameObject> enemies)
    {
        _enemyPrefabs = new List<GameObject>();
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<Enemy>() == null)
            {
                throw new ArgumentException("Object is not an enemy");
            }
            _enemyPrefabs.Add(enemy);
        }
    }

    public IEnumerator StartBattle()
    {
        // Spawn enemies in
        List<GameObject> enemies = new List<GameObject>();
        for (int i = 0; i < _enemyPrefabs.Count; i++)
        {
            enemies.Add(Instantiate(_enemyPrefabs[i]));
        }
        // Perform all of their attacks
        List<Coroutine> runningCoroutines = new List<Coroutine>();
        foreach (GameObject obj in enemies)
        {
            Enemy enemy = obj.GetComponent<Enemy>();
            runningCoroutines.Add(StartCoroutine(enemy.StartAttackPhase()));
        }
        foreach (Coroutine coroutine in runningCoroutines)
        {
            yield return coroutine;
        }
        // After all attacks are performed, set the state back to battle system menu
        PlayerManager.Instance.PlayerInputManager.SetInputState(InputState.BatleSystemMenu);
    }
}
