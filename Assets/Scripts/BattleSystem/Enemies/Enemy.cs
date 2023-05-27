using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    private List<EnemyAttack> _attackPool;
    private Queue<EnemyAttack> _attackQueue;
    private int minAttacksInQueue;
    private int maxAttacksInQueue;
    
    private void Awake()
    {
        _attackPool = new List<EnemyAttack>();
        _attackQueue = new Queue<EnemyAttack>();
        minAttacksInQueue = 2;
        maxAttacksInQueue = 4;
        InitalizeAttackPool();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("PlayerCombat"))
        {
            Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
        }
    }

    public abstract void InitalizeAttackPool();

    public void AddAttackToPool(EnemyAttack attack)
    {
        _attackPool.Add(attack);
    }

    private EnemyAttack GetRandomAttack()
    {
        int random = Random.Range(0, _attackPool.Count - 1);
        return _attackPool[random];
    }

    private void QueueAttacks()
    {
        int numOfAttacks = Random.Range(minAttacksInQueue, maxAttacksInQueue);
        for (int i = 0; i < numOfAttacks; i++)
        {
            _attackQueue.Enqueue(GetRandomAttack());
        }
    }

    private IEnumerator PerformAttacks()
    {
        int numOfAttacks = _attackQueue.Count;
        for (int attack = 0; attack < numOfAttacks; attack++)
        {
            _attackQueue.Dequeue().PerformAttack();
            yield return new WaitForSeconds(2);
        }
    }

    public IEnumerator StartAttackPhase()
    {
        QueueAttacks();
        yield return PerformAttacks();
    }
}
