using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BasicSlimeNew : Enemy
{
    private enum State
    {
        Idle,
        Jumping,
        SlimeShot,
    }

    [SerializeField] private GameObject _slimeball;

    private State _currentState;

    [Header("Idle")]
    private readonly float _minEndlag = 1f;
    private readonly float _maxEndlag = 2f;
    private float _currentEndlagDuration;
    private float _currentEndlagTimer = 0f;

    [Header("Jumping")]
    private readonly float _minJumpDistance = 4f;
    private readonly float _maxJumpDistanceClose = 6f;
    private readonly float _maxJumpDistanceFar = 12f;

    private void Start()
    {
        SetState(State.Idle);
    }

    private void Update()
    {
        UpdateFacing();
        UpdateCurrentState();   
    }

    public override IEnumerator EnemyAI()
    {
        yield return null;
    }

    public override void InitalizeAttackPool()
    {
        
    }

    #region Idle

    private void EnterIdleState()
    {
        // Randomize the endlag duration
        _currentEndlagDuration = Random.Range(_minEndlag, _maxEndlag);
    }

    private void UpdateIdleState()
    {
        // Increase the endlag timer
        _currentEndlagTimer += Time.deltaTime;
        // If the timer ends, switch the state
        if (_currentEndlagTimer >= _currentEndlagDuration)
        {
            int newState = Random.Range(0, 2);
            switch(newState)
            {
                case 0:
                    SetState(State.Jumping); 
                    break;
                case 1:
                    SetState(State.SlimeShot);
                    break;
            }
        }
    }

    private void ExitIdleState()
    {
        // Reset the endlag timer
        _currentEndlagTimer = 0;
    }

    #endregion

    #region SlimeShot
    private void EnterSlimeShotState()
    {
        Vector3 spawnPosition;
        if (Direction == Facing.right)
        {
            spawnPosition = this.transform.position + new Vector3(0.75f, 0, 0);
        }
        else
        {
            spawnPosition = this.transform.position + new Vector3(-0.75f, 0, 0);
        }
        SlimeShot shot = Instantiate(_slimeball, spawnPosition, Quaternion.identity).GetComponent<SlimeShot>();
        shot.Damage = 5f;
        shot.Pew(Direction);
    }

    private void UpdateSlimeShotState()
    {
        // Do nothing and immediately switch the state back to idle
        SetState(State.Idle);
    }

    private void ExitSlimeShotState()
    {

    }
    #endregion

    #region Jumping

    private void EnterJumpingState()
    {
        // Get the distance between the player and the slime
        Vector2 distance = PlayerManager.Instance.PlayerCombat.transform.position - this.transform.position; // fun line
        float absoluteXDistance = Mathf.Min(Mathf.Abs(distance.x), 6f); // XDist is player.x - slime.x, max is 6
        float absoluteYDistance = Mathf.Min(Mathf.Abs(distance.y), 6f); // Same with YDist
        float jumpForce;

        if (absoluteXDistance < 2)
        {
            float jumpDistance = Mathf.Pow(absoluteXDistance, -1f) + 2;
            jumpForce = Mathf.Clamp(jumpDistance, _minJumpDistance, _maxJumpDistanceClose);
        }
        else
        {
            float jumpDistance = Mathf.Pow(absoluteXDistance, 0.85f) - (Mathf.Pow(2, 0.85f) - 0.5f);
            jumpForce = Mathf.Clamp(jumpDistance, _minJumpDistance, _maxJumpDistanceFar);
        }

        // If there is a height difference between the player and slime, increase jump force proportional to that distance
        if (absoluteYDistance > 1)
        {
            jumpForce += absoluteYDistance * 1.25f;
        }

        // Uses the jump force and x distance to calculate the velocity
        Vector2 velocityForce;
        if (Direction == Facing.left)
        {
            velocityForce = new Vector2(-absoluteXDistance, jumpForce);
        }
        else
        {
            velocityForce = new Vector2(absoluteXDistance, jumpForce);
        }

        this.GetComponent<Rigidbody2D>().AddForce(velocityForce, ForceMode2D.Impulse); // Makes the slime jump

    }

    private void UpdateJumpingState()
    {
        // When the slime lands on the ground, set it's state back to idle
        if (CurrentlyGrounded() && this.GetComponent<Rigidbody2D>().velocity.y == 0)
        {
            SetState(State.Idle);
        }
    }

    private void ExitJumpingState()
    {
        // When the slime lands, set the velocity to zero
        this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
    #endregion

    #region StateMachine
    private void SetState(State newState)
    {
        // Exit the current state
        switch (_currentState)
        {
            case State.Idle:
                ExitIdleState();
                break;
            case State.Jumping:
                ExitJumpingState();
                break;
            case State.SlimeShot:
                ExitSlimeShotState();
                break;
        }
        // Enter the new state
        switch (newState)
        {
            case State.Idle:
                EnterIdleState();
                break;
            case State.Jumping:
                EnterJumpingState();
                break;
            case State.SlimeShot:
                EnterSlimeShotState();
                break;
        }
        _currentState = newState;
    }

    private void UpdateCurrentState()
    {
        switch (_currentState)
        {
            case State.Idle:
                UpdateIdleState();
                break;
            case State.Jumping:
                UpdateJumpingState();
                break;
            case State.SlimeShot:
                UpdateSlimeShotState();
                break;
        }
    }

    #endregion
}
