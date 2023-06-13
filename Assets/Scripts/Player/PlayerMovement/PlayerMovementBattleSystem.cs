using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum BattleSystemDirection
{
    left,
    right
}

public class PlayerMovementBattleSystem : MonoBehaviour
{
    private enum PlayerState
    {
        Moving, // Walking or jumping
        Staggered
    }
    private PlayerState _currentState;

    private PlayerManager playerManager;

    [Header("Components")]
    private Rigidbody2D _rigidbody;
    private Animator _animator;

    [Header("General")]
    private List<string> _statusEffectSources;

    [Header("Walking")]
    public BattleSystemDirection CurrentDirection = BattleSystemDirection.left;
    [SerializeField] private float _baseMoveSpeed; // Base speed no changes
    private float _moveSpeedFlatChange; // A flat change in move speed
    private float _moveSpeedMultiplier; 
    private float _moveSpeedExplicitlySet; // If this value is set, use this for speed, else calculate speed using the flat change and multiplier
    private Vector2 _movementChange; // Detects player input for what direction the player should move
   
    [Header("Knockback")]
    private readonly Vector2 _baseKnockbackForce = new Vector2(10f, 5f);
    private Vector2 finalKnockbackForce;
    private readonly float _knockbackDuration = 0.25f;
    private float _currentKnockbackTime;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Vector2 _groundCheckDimensions;

    [Header("Jumping")]
    [SerializeField] private float _baseJumpSpeed;
    private float _jumpSpeedFlatChange;
    private float _jumpSpeedMultiplier;
    private float _jumpSpeedExplicitlySet; // If this value is set, use this for jump speed, else calculate speed using the flat change and multiplier
    public bool JumpPressed;
    private float _startingJumpHeight = float.NegativeInfinity;
    private readonly float _minJumpHeight = 1.2f;
    private readonly float _jumpBufferTime = 0.25f;
    private float _coyoteBufferTime;
    private readonly float _maxCoyoteBufferTime = .25f;
    private int _timesJumped = 0;
    private readonly int _maxJumps = 1;

    [Header("Gravity")]
    private readonly float _maxFallSpeed = 50f;
    private readonly float _defaultGravityScale = 1f;
    private readonly float _fallGravityMultiplier = 2f;
   

    public float NormalPhysAttackAnimLength = 1f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _statusEffectSources = new List<string>();

        _moveSpeedFlatChange = 0f;
        _moveSpeedMultiplier = 1f;
        _moveSpeedExplicitlySet = float.NegativeInfinity;

        _jumpSpeedFlatChange = 0f;
        _jumpSpeedMultiplier = 1f;
        _jumpSpeedExplicitlySet = float.NegativeInfinity;
        _coyoteBufferTime = 0f;
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
        FixedUpdateCurrentState();
    }

    private void Update()
    {
        UpdateCurrentState();        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(_groundCheck.position, _groundCheckDimensions);
    }

    #region StatusEffect

    public void AddStatusEffectSource(string effectName)
    {
        _statusEffectSources.Add(effectName);
    }

    public void RemoveStatusEffectSource(string effectName)
    {
        _statusEffectSources.Remove(effectName);
    }

    public bool HasMoreThanOneStatusEffectSource(string effectName)
    {
        return _statusEffectSources.Count(name => name.Equals(effectName)) > 1;
    }

    public bool HasStatusEffectSource(string effectName)
    {
        return _statusEffectSources.Contains(effectName);
    }

    #endregion

    #region OnGettingAttacked
    public void ApplyKnockback(Facing direction, float force)
    {
        finalKnockbackForce = _baseKnockbackForce;
        if (direction == Facing.Right)
        {
            finalKnockbackForce.x *= force;
        }
        else
        {
            finalKnockbackForce.x *= -force;
        }
        SetState(PlayerState.Staggered);
    }
    #endregion

    #region Walking
    public void CalculateHorizontalMovement(Vector2 change)
    {
        _rigidbody.velocity = new Vector2(change.x * CalculateMoveSpeed(), _rigidbody.velocity.y);
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

    public void AdjustMovementSpeedFlatChange(float change)
    {
        _moveSpeedFlatChange += change;
    }

    public void AdjustMovementSpeedMultiplier(float multiplier)
    {
        _moveSpeedMultiplier += multiplier;
    }
    
    public void SetMovementSpeedExplicitly(float speed)
    {
        _moveSpeedExplicitlySet = speed;
    }

    public void ResetExplicitMovementSpeed()
    {
        _moveSpeedExplicitlySet = float.NegativeInfinity;
    }

    public float CalculateMoveSpeed()
    {
        // If explicit speed is not 0, that is the speed, else calculate it using the formula
        float speed = _moveSpeedExplicitlySet != float.NegativeInfinity ? _moveSpeedExplicitlySet : (_baseMoveSpeed + _moveSpeedFlatChange) * _moveSpeedMultiplier;
        return speed;
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
            if (HasCoyoteTime() && _timesJumped < _maxJumps)
            {
                _timesJumped++;
                _coyoteBufferTime = float.PositiveInfinity;
                _startingJumpHeight = _rigidbody.position.y;
                _rigidbody.velocity = new Vector2(_movementChange.x * CalculateMoveSpeed(), CalculateJumpSpeed());
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
        
        if (CurrentlyGrounded())
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

    public void AdjustJumpSpeedFlatChange(float change)
    {
        _jumpSpeedFlatChange += change;
    }

    public void AdjustJumpSpeedMultiplier(float multiplier)
    {
        _jumpSpeedMultiplier += multiplier;
    }

    public void SetJumpSpeedExplicitly(float speed)
    {
        _jumpSpeedExplicitlySet = speed;
    }

    public void ResetExplicitJumpSpeed()
    {
        _jumpSpeedExplicitlySet = float.NegativeInfinity;
    }

    public float CalculateJumpSpeed()
    {
        // If explicit speed is not 0, that is the speed, else calculate it using the formula
        float speed = _jumpSpeedExplicitlySet != float.NegativeInfinity ? _jumpSpeedExplicitlySet : (_baseJumpSpeed + _jumpSpeedFlatChange) * _jumpSpeedMultiplier;
        return speed;
    }

    private bool CurrentlyGrounded()
    {
        return Physics2D.OverlapBox(_groundCheck.position, _groundCheckDimensions, 0, _groundLayer);
    }

    private bool HasCoyoteTime()
    {
        return _coyoteBufferTime < _maxCoyoteBufferTime;
    }

    private void UpdateCoyoteTime()
    {
        if (!Physics2D.OverlapBox(_groundCheck.position, _groundCheckDimensions, 0, _groundLayer))
        {
            _coyoteBufferTime += Time.deltaTime;
        }
        else if (_rigidbody.velocity.y == 0)
        {
            _timesJumped = 0;
            _coyoteBufferTime = 0;
        }
    }

    private void SetGravityScale(float gravityScale)
    {
        _rigidbody.gravityScale = gravityScale;
    }
    #endregion

    #region NormalPhysicalAttack
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

    #region MovingState

    private void EnterMovingState()
    {

    }

    private void UpdateMovingState()
    {

    }

    private void FixedUpdateMovingState()
    {
        if (playerManager.PlayerInputManager.CurrentState == InputState.BattleSystem)
        {
            CalculateHorizontalMovement(_movementChange);
        }
        UpdateCoyoteTime();
        ReduceJumpHeight();
    }

    private void ExitMovingState()
    {

    }

    #endregion

    #region StaggeredState

    private void EnterStaggeredState()
    {
        _currentKnockbackTime = 0;
    }

    private void UpdateStaggeredState()
    {

    }

    private void FixedUpdateStaggeredState()
    {
        if (_currentKnockbackTime < _knockbackDuration)
        {
            _rigidbody.velocity = finalKnockbackForce;
            _currentKnockbackTime += Time.deltaTime;
        }
        else
        {
            SetState(PlayerState.Moving);
        }
    }

    private void ExitStaggeredState()
    {

    }

    #endregion

    #region StateMachine

    private void SetState(PlayerState newState)
    {
        // Exit the current state
        switch (_currentState)
        {
            case PlayerState.Moving:
                ExitMovingState();
                break;
            case PlayerState.Staggered:
                ExitStaggeredState();
                break;
        }
        // Enter the new state
        switch (newState)
        {
            case PlayerState.Moving:
                EnterMovingState();
                break;
            case PlayerState.Staggered:
                EnterStaggeredState();
                break;
        }
        _currentState = newState;
    }

    private void UpdateCurrentState()
    {
        // Exit the current state
        switch (_currentState)
        {
            case PlayerState.Moving:
                UpdateMovingState();
                break;
            case PlayerState.Staggered:
                UpdateStaggeredState();
                break;
        }
    }

    private void FixedUpdateCurrentState()
    {
        // Exit the current state
        switch (_currentState)
        {
            case PlayerState.Moving:
                FixedUpdateMovingState();
                break;
            case PlayerState.Staggered:
                FixedUpdateStaggeredState();
                break;
        }
    }
    #endregion
}
