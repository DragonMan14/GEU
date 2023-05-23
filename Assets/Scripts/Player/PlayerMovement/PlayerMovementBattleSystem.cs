using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    // Jumping
    private float _startingJumpHeight = float.NegativeInfinity;
    private readonly float _minJumpHeight = 1.2f;
    private readonly float _jumpBufferTime = 0.25f;
    private int _maxJumps = 1;
    private int _timesJumped = 0;
    // Gravity and Falling
    private readonly float _maxFallSpeed = 50f;
    private readonly float _defaultGravityScale = 1f;
    private readonly float _fallGravityMultiplier = 2f;
    private readonly float _maxGroundedBuffer = 5f;
    private float _groundedBuffer;

    public float NormalPhysAttackAnimLength = 1f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _groundedBuffer = 0f;
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
        print(_groundedBuffer);
        UpdateGrounded();
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
        StartCoroutine(JumpBuffer());
    }

    private IEnumerator JumpBuffer()
    {
        float timer = 0f;
        while (timer < _jumpBufferTime)
        {
            if (IsBufferGrounded() && _timesJumped < _maxJumps)
            {
                _timesJumped++;
                _groundedBuffer = float.PositiveInfinity;
                _startingJumpHeight = _rigidbody.position.y;
                _rigidbody.velocity = new Vector2(_movementChange.x * _moveSpeed, _jumpSpeed);
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void ReduceJumpHeight()
    {
        bool playerIsGoingUp = _rigidbody.velocity.y > 0f;
        bool playerIsFalling = _rigidbody.velocity.y < -0f;
        bool minJumpAchieved = _rigidbody.position.y - _startingJumpHeight > _minJumpHeight;
        
        if (IsCurrentlyGrounded())
        {
            SetGravityScale(_defaultGravityScale);
        }
        else if (playerIsGoingUp && !JumpPressed && minJumpAchieved)
        {
            // The player's jump height depends on how long they hold jump
            _startingJumpHeight = float.NegativeInfinity;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);
        }
        else if (playerIsFalling)
        {
            // The player's fall speed increases while falling
            SetGravityScale(_defaultGravityScale * _fallGravityMultiplier);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Mathf.Max(_rigidbody.velocity.y, -_maxFallSpeed));
        }
    }

    private bool IsCurrentlyGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
    }

    private bool IsBufferGrounded()
    {
        return _groundedBuffer < _maxGroundedBuffer;
    }

    private void UpdateGrounded()
    {
        if (!Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer))
        {
            _groundedBuffer += Time.deltaTime;
        }
        else if (_rigidbody.velocity.y == 0)
        {
            _timesJumped = 0;
            _groundedBuffer = 0;
        }
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
