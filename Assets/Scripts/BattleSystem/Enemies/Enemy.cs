using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public enum Facing
{
    Left,
    Right
}

public abstract class Enemy : MonoBehaviour
{
    public Facing Direction;

    [Header("Components")]
    [HideInInspector] public Rigidbody2D Rigidbody;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public SpriteRenderer SpriteRenderer;

    [Header("Knockback")]
    public float BaseKnockbackForceMultiplier = 1f;
    public float BaseKnockbackDamage = 5f;

    [Header("Physics")]
    public LayerMask EnvironmentLayer;

    [Header("Ground Check")]
    public Transform GroundCheck;
    public Vector2 GroundCheckDimensions;
    public Transform EdgeOfGroundCheck;
    public Vector2 EdgeOfGroundCheckDimensions;

    [Header("Wall Check")]
    public Transform WallCheck;
    public Vector2 WallCheckDimensions;

    private void Awake()
    {
        Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), PlayerManager.Instance.PlayerCombat.GetComponent<Collider2D>());
        Direction = Facing.Right;

        Rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void Update()
    {
        UpdateFacing();
        UpdateCurrentState();
        CustomUpdate();
    }

    public virtual void CustomUpdate()
    {

    }

    public abstract void UpdateCurrentState();

    #region Physics

    public virtual void UpdateFacing()
    {
        bool playerOnRight = PlayerManager.Instance.PlayerCombat.transform.position.x > this.gameObject.transform.position.x;
        if (playerOnRight && Direction == Facing.Left)
        {
            FlipRotation();
        }
        else if (!playerOnRight && Direction == Facing.Right)
        {
            FlipRotation();
        }
    }

    public void FlipRotation()
    {
        if (Direction == Facing.Left)
        {
            Direction = Facing.Right;
        }
        else
        {
            Direction = Facing.Left;
        }
        Vector3 flippedScale = this.transform.localScale;
        flippedScale.x *= -1;
        this.transform.localScale = flippedScale;
    }

    public bool IsCurrentlyGrounded()
    {
        return Physics2D.OverlapBox(GroundCheck.position, GroundCheckDimensions, this.transform.eulerAngles.z, EnvironmentLayer);
    }

    public bool IsCollidingWithWall()
    {
        return Physics2D.OverlapBox(WallCheck.position, WallCheckDimensions, this.transform.eulerAngles.z, EnvironmentLayer);
    }

    public bool IsAtEdgeOfGround()
    {
        return !Physics2D.OverlapBox(EdgeOfGroundCheck.position, EdgeOfGroundCheckDimensions, this.transform.eulerAngles.z, EnvironmentLayer);
    }

    public bool IsCenteredAtEdgeOfGround()
    {
        return !Physics2D.OverlapBox(GroundCheck.position, new Vector2(0.1f, 0.1f), this.transform.eulerAngles.z, EnvironmentLayer);
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Matrix4x4 tempMatrix = Gizmos.matrix;
        if (GroundCheck != null)
        {
            Gizmos.matrix = Matrix4x4.TRS(GroundCheck.position, this.transform.rotation, this.transform.lossyScale);
            Gizmos.DrawCube(Vector2.zero, GroundCheckDimensions);
        }
        if (WallCheck != null)
        {
            Gizmos.matrix = Matrix4x4.TRS(WallCheck.position, this.transform.rotation, this.transform.lossyScale);
            Gizmos.DrawCube(Vector2.zero, WallCheckDimensions);
        }
        if (EdgeOfGroundCheck != null)
        {
            Gizmos.matrix = Matrix4x4.TRS(EdgeOfGroundCheck.position, this.transform.rotation, this.transform.lossyScale);
            Gizmos.DrawCube(Vector2.zero, EdgeOfGroundCheckDimensions);
        }
        Gizmos.matrix = tempMatrix;
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
