using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public enum Facing
{
    left,
    right
}

public abstract class Enemy : MonoBehaviour
{
    public List<EnemyAttack> _attackPool;
    public Facing Direction;
    public bool CurrentlyAttacking;
    public bool CanAttack;

    [Header("Ground Check")]
    public Transform _groundCheck;
    public LayerMask _groundLayer;
    public Vector2 _groundCheckDimensions;

    private void Awake()
    {
        _attackPool = new List<EnemyAttack>();
        InitalizeAttackPool();

        Direction = Facing.right;
        CurrentlyAttacking = false;
        CanAttack = true;

        StartCoroutine(EnemyAI());
    }

    private void Update()
    {
        UpdateFacing();
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

    public abstract IEnumerator EnemyAI();

    public void UpdateFacing()
    {
        bool playerOnRight = PlayerManager.Instance.PlayerCombat.transform.position.x > this.gameObject.transform.position.x;
        if (playerOnRight && Direction == Facing.left) {
            FlipRotation();
        }
        else if (!playerOnRight && Direction == Facing.right)
        {
            FlipRotation();
        }
    }

    public void FlipRotation()
    {
        if (Direction == Facing.left)
        {
            Direction = Facing.right;
        }
        else
        {
            Direction = Facing.left;
        }
        Vector3 flippedScale = this.transform.localScale;
        flippedScale.x *= -1;
        this.transform.localScale = flippedScale;
    }   

    public bool CurrentlyGrounded()
    {
        return Physics2D.OverlapBox(_groundCheck.position, _groundCheckDimensions, 0, _groundLayer);
    }

    public virtual EnemyAttack GetRandomAttack()
    {
        int random = Random.Range(0, _attackPool.Count);
        return _attackPool[random];
    }

    public IEnumerator PerformRandomAttack()
    {
        yield return GetRandomAttack().PerformAttack();
    }
}
