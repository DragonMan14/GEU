using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public enum MovementState
{
    Openworld,
    Battlesystem,
    DialogueOptionsMenu,
}

public class PlayerMovement : MonoBehaviour
{
    private PlayerManager playerManager;
    public MovementState PreviousState { get; private set; }
    public MovementState CurrentState;

    [SerializeField] private float _overworldMoveSpeed;
    private Rigidbody2D _rigidbody;
   
    private Vector2 _movementChange;

    [SerializeField] private float _battleSystemMoveSpeed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;

    // Start is called before the first frame update
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();   
    }

    private void Start()
    {
        playerManager = PlayerManager.Instance;
        if (playerManager.PlayerMovement != null && playerManager.PlayerMovement != this)
        {
            Destroy(this);
        }
        else
        {
            playerManager.SetPlayerMovement(this);
        }
    }

    private void FixedUpdate()
    {
        if (CurrentState == MovementState.Openworld)
        {
            CalculateOverworldMovement(_movementChange);
        }
        else if (CurrentState == MovementState.Battlesystem)
        {
            CalculateBattleSystemHorizontalMovement(_movementChange);
        }
    }
    private void Update()
    {
        CalculateBattleSystemVerticalMovement(_movementChange);
    }

    public void CalculateOverworldMovement(Vector2 change)
    {
        change = _overworldMoveSpeed * Time.deltaTime * change;
        Vector2 currentPosition = this.transform.position;
        Vector2 newPosition = currentPosition + change;
        _rigidbody.MovePosition(newPosition);
    }

    public void CalculateBattleSystemHorizontalMovement(Vector2 change)
    {
        _rigidbody.velocity = new Vector2(change.x * _battleSystemMoveSpeed, _rigidbody.velocity.y);
    }

    public void CalculateBattleSystemVerticalMovement(Vector2 change)
    {
        bool jumpPressedInFrame = playerManager.PlayerInput.MovementWasPressedThisFrame() && _movementChange.y > 0;
        if (jumpPressedInFrame && IsGrounded())
        {
            _rigidbody.velocity = new Vector2(change.x * _battleSystemMoveSpeed, _jumpSpeed);
        }
        else if (_movementChange.y == 0 && _rigidbody.velocity.y > 0)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
    }

    public void SetMovementChange(Vector2 change)
    {
        _movementChange = change;
    }

    public void SetMovementState(MovementState state)
    {
        PreviousState = CurrentState;
        CurrentState = state;
    }
}
