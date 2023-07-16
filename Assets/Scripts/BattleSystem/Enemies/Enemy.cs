using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public enum Facing
{
    Left,
    Right
}

public abstract class Enemy : MonoBehaviour, IPathfindingRestrictor
{
    public Facing Direction;

    [Header("Components")]
    [HideInInspector] public Rigidbody2D Rigidbody;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public SpriteRenderer SpriteRenderer;

    [Header("Pathfinding")]
    public Path CurrentPath; // The path from the current node to goal node
    public Node CurrentNode; // The node the enemy is currently at
    public Node NextNode; // The next node in our path
    public Node GoalNode; // The node we want to end up at
    public List<NodeType> validNodeTypes;
    public List<EdgeType> validEdgeTypes;
    public Transform NodeCheck;

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

        CurrentPath = new Path(new Stack<Edge>());
    }

    public virtual void Update()
    {
        CurrentNode = PathfindingManager.Instance.CurrentGrid.GetNodeClosestTo(NodeCheck.position, validNodeTypes);
        UpdateFacing();
        UpdateCurrentState();
        CustomUpdate();
    }

    public virtual void CustomUpdate()
    {

    }

    public virtual bool IsValidPathAvailable(Edge edge)
    {
        return true;
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
        if (NodeCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(NodeCheck.position, this.transform.rotation, this.transform.lossyScale);
            Gizmos.DrawSphere(Vector2.zero, 0.1f);
        }
        Gizmos.matrix = tempMatrix;

        Gizmos.color = Color.blue;
        if (CurrentPath != null && CurrentPath.Count != 0)
        {
            foreach (Edge next in CurrentPath)
            {
                Gizmos.DrawLine(next.GetNode1().Coordinates, next.GetNode2().Coordinates);
            }
        }

        if (CurrentNode != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(CurrentNode.Coordinates, 0.2f);
        }
        if (NextNode != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(NextNode.Coordinates, 0.2f);
        }
        CustomOnDrawGizmos();
    }

    public virtual void CustomOnDrawGizmos()
    {

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
