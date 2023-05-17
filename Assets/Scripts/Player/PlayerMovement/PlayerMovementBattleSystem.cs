using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBattleSystem : MonoBehaviour
{
    private PlayerManager playerManager;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpSpeed;
    private Rigidbody2D _rigidbody;

    private Vector2 _movementChange;

    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;

    public bool JumpPressed;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
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

    public void CalculateHorizontalMovement(Vector2 change)
    {
        _rigidbody.velocity = new Vector2(change.x * _moveSpeed, _rigidbody.velocity.y);
    }

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
        if (!JumpPressed && _rigidbody.velocity.y > 0)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
    }

    public void SetHorizontalMovementChange(Vector2 change)
    {
        _movementChange.x = change.x;
    }
}
