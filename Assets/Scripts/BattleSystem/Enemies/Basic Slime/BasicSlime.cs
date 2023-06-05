using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BasicSlime : Enemy
{
    private enum State
    {
        Idle,
        Jumping,
        SlimeShot,
    }

    private State _currentState;
    private State _lastAttack;
    private int _consecutiveSameAttackCounter;
    private int _maxStall;

    [Header("Idle")]
    private readonly float _minEndlag = 1f;
    private readonly float _maxEndlag = 2f;
    private float _currentEndlagDuration;
    private float _currentEndlagTimer = 0f;

    [Header("Jumping")]
    private readonly float _minJumpDistance = 4f;
    private readonly float _maxJumpDistanceClose = 6f;
    private readonly float _maxJumpDistanceFar = 12f;

    [Header("SlimeShot")]
    [SerializeField] private GameObject _slimeball;

    private void Start()
    {
        SetState(State.Idle);
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
            // 1 == Jump, 2 == Slimeshot
            int newState = Random.Range(1, 3);

            if (((int)_lastAttack) != newState)
            {
                _consecutiveSameAttackCounter = 1;
            }
            else if ((int)_lastAttack == newState && _consecutiveSameAttackCounter < _maxStall)
            {
                _consecutiveSameAttackCounter++;
            } 
            else
            {
                newState = newState == 1 ? 2 : 1;
            }

            switch(newState)
            {
                case 1:
                    SetState(State.Jumping); 
                    break;
                case 2:
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
        // Play animation
        Animator.SetTrigger("Shoot");
    }

    private void UpdateSlimeShotState()
    {
        // If the animation is still playing, wait
        if (Animator.GetCurrentAnimatorStateInfo(0).IsName("BasicSlime_Shooting"))
        {
            return;
        }
        // Set the state back to idle
        SetState(State.Idle);
    }

    private void ExitSlimeShotState()
    {

    }

    private void InstantiateSlimeShot()
    {
        Vector3 spawnPosition;
        if (Direction == Facing.right)
        {
            spawnPosition = this.transform.position + new Vector3(0.60f, 0, 0);
        }
        else
        {
            spawnPosition = this.transform.position + new Vector3(-0.60f, 0, 0);
        }
        SlimeShot shot = Instantiate(_slimeball, spawnPosition, Quaternion.identity).GetComponent<SlimeShot>();
        shot.Damage = 5f;
        shot.Pew(Direction);
    }

    #endregion

    #region Jumping

    private void EnterJumpingState()
    {
        // Play animation
        Animator.SetTrigger("Jump");
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

        Rigidbody.AddForce(velocityForce, ForceMode2D.Impulse); // Makes the slime jump

    }

    private void UpdateJumpingState()
    {
        // When the slime lands on the ground, set it's state back to idle
        if (CurrentlyGrounded() && Rigidbody.velocity.y == 0)
        {
            SetState(State.Idle);
        }
    }

    private void ExitJumpingState()
    {
        Animator.SetTrigger("Landing");
        // When the slime lands, set the velocity to zero
        Rigidbody.velocity = Vector2.zero;
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
        _lastAttack = _currentState;
        _currentState = newState;
    }

    public override void UpdateCurrentState()
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
