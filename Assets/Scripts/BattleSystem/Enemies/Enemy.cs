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
    public Facing Direction;

    [Header("Components")]
    public Rigidbody2D Rigidbody;
    public Animator Animator;

    [Header("Knockback")]
    public float BaseKnockbackForceMultiplier = 1f;
    public float BaseKnockbackDamage = 5f;

    [Header("Ground Check")]
    public Transform _groundCheck;
    public LayerMask _groundLayer;
    public Vector2 _groundCheckDimensions;

    private void Awake()
    {
        Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), PlayerManager.Instance.PlayerCombat.GetComponent<Collider2D>());
        Direction = Facing.right;

        Rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateFacing();
        UpdateCurrentState();
    }

    public abstract void UpdateCurrentState();

    #region Physics

    public void UpdateFacing()
    {
        bool playerOnRight = PlayerManager.Instance.PlayerCombat.transform.position.x > this.gameObject.transform.position.x;
        if (playerOnRight && Direction == Facing.left)
        {
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(_groundCheck.position, _groundCheckDimensions);
    }

    // Basic knockback collision damage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("PlayerCombat"))
        {
            return;
        }
        PlayerManager.Instance.PlayerAttributes.DrainHealth(BaseKnockbackDamage);
        PlayerManager.Instance.PlayerMovementManager.PlayerMovementBattleSystem.ApplyKnockback(Direction, BaseKnockbackForceMultiplier);
    }

    #endregion
}
