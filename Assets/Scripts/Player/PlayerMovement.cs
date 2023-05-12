using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerManager playerManager;

    [SerializeField] private float _movementSpeed;
    private Rigidbody2D _rigidbody;

    private Vector2 _movementChange;

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
        MovePlayer(_movementChange);
    }

    private void MovePlayer(Vector2 change)
    {
        change = _movementSpeed * Time.deltaTime * change;
        Vector2 currentPosition = this.transform.position;
        Vector2 newPosition = currentPosition + change;
        _rigidbody.MovePosition(newPosition);
    }

    public void SetMovementChange(Vector2 change)
    {
        _movementChange = change;
    }
}
