using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private List<EnemyAttack> _attackPool;
    private Queue<EnemyAttack> _attackQueue;
    private int minAttacksInQueue;
    private int maxAttacksInQueue;

    private void Awake()
    {
        _attackPool = new List<EnemyAttack>();
        AddAttackToPool(new Attack1());
        AddAttackToPool(new Attack2());
        AddAttackToPool(new Attack3());

        _attackQueue = new Queue<EnemyAttack>();
        minAttacksInQueue = 2;
        maxAttacksInQueue = 4;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H)) 
        {
            print("test");
            StartCoroutine(StartAttackPhase());
        }
    }

    #region Attacks
    public class Attack1 : EnemyAttack
    {
        public override void PerformAttack()
        {
            print("attack1");
        }
    }

    public class Attack2 : EnemyAttack
    {
        public override void PerformAttack()
        {
            print("attack2");
        }
    }

    public class Attack3 : EnemyAttack
    {
        public override void PerformAttack()
        {
            print("attack3");
        }
    }
    #endregion

    private void AddAttackToPool(EnemyAttack attack)
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
