using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum BattleSystemDirection
{
    left,
    right
}

public class PlayerMovementBattleSystem : MonoBehaviour
{
    private PlayerManager playerManager;

    private Animator _animator;
    public BattleSystemDirection CurrentDirection = BattleSystemDirection.left;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpSpeed;
    private Rigidbody2D _rigidbody;

    private Vector2 _movementChange;

    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    public bool JumpPressed;

    private readonly float _maxFallSpeed = 50f;
    private readonly float _defaultGravityScale = 1f;
    private readonly float _fallGravityMultiplier = 2f;

    public float NormalPhysAttackAnimLength = 1f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        playerManager = PlayerManager.Instance;
        if (playerManager.PlayerMovementManager.PlayerMovementBattleSystem != null && playerManager.PlayerMovementManager.PlayerMovementOverworld != this)
        {
            Destroy(this);
        }
        else
        {
            playerManager.PlayerMovementManager.PlayerMovementBattleSystem = this;
        }
    }

    private void FixedUpdate()
    {
        if (playerManager.PlayerInputManager.CurrentState == InputState.BattleSystem)
        {
            CalculateHorizontalMovement(_movementChange);
        }
        ReduceJumpHeight();
    }

    #region Walking
    public void CalculateHorizontalMovement(Vector2 change)
    {
        _rigidbody.velocity = new Vector2(change.x * _moveSpeed, _rigidbody.velocity.y);
        if (change.x < 0)
        {
            CurrentDirection = BattleSystemDirection.left;
            _animator.SetFloat("DirectionX", -1f);

        }
        else if (change.x > 0)
        {
            CurrentDirection = BattleSystemDirection.right;
            _animator.SetFloat("DirectionX", 1f);
        }
    }

    public void SetHorizontalMovementChange(Vector2 change)
    {
        _movementChange.x = change.x;
    }
    #endregion

    #region Jumping
    public void PerformJump()
    {
        if (!IsGrounded())
        {
            return;
        }
        _rigidbody.velocity = new Vector2(_movementChange.x * _moveSpeed, _jumpSpeed);
    }

    private void ReduceJumpHeight()
    {
        bool playerIsGoingUp = _rigidbody.velocity.y > 0f;
        bool playerIsFalling = _rigidbody.velocity.y < -0f;

        if (IsGrounded())
        {
            SetGravityScale(_defaultGravityScale);
        }
        else if (playerIsGoingUp && !JumpPressed)
        {
            // The player's jump height depends on how long they hold jump
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);
        }
        else if (playerIsFalling)
        {
            // The player's fall speed increases while falling
            SetGravityScale(_defaultGravityScale * _fallGravityMultiplier);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Mathf.Max(_rigidbody.velocity.y, -_maxFallSpeed));
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
    }

    private void SetGravityScale(float gravityScale)
    {
        _rigidbody.gravityScale = gravityScale;
    }
    #endregion

    #region Normal Physical Attack
    public void PerformNormalPhysicalAttack()
    {
        StartCoroutine(NormalPhysicalAttackCo());
    }

    public IEnumerator NormalPhysicalAttackCo()
    {
        _animator.SetBool("NormalPhysicalAttacking", true);
        yield return new WaitForSeconds(NormalPhysAttackAnimLength);
        _animator.SetBool("NormalPhysicalAttacking", false);
    }
    #endregion
}
