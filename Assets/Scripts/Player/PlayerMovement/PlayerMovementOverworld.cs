using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementOverworld : MonoBehaviour
{
    private PlayerManager playerManager;

    [SerializeField] private float _moveSpeed;
    private Rigidbody2D _rigidbody;
    private Vector2 _movementChange;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        playerManager = PlayerManager.Instance;
        if (playerManager.PlayerMovementManager.PlayerMovementOverworld != null && playerManager.PlayerMovementManager.PlayerMovementOverworld != this)
        {
            Destroy(this);
        }
        else
        {
            playerManager.PlayerMovementManager.PlayerMovementOverworld = this;
        }
    }

    private void FixedUpdate()
    {
        if (playerManager.PlayerInputManager.CurrentState == InputState.Openworld)
        {
            CalculateMovement(_movementChange);
        }
    }

    public void CalculateMovement(Vector2 change)
    {
        change = _moveSpeed * Time.deltaTime * change;
        Vector2 currentPosition = this.transform.position;
        Vector2 newPosition = currentPosition + change;
        _rigidbody.MovePosition(newPosition);
    }

    public void SetMovementChange(Vector2 change)
    {
        _movementChange = change;
    }
}
