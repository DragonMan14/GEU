using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

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
    private readonly int _maxStall = 3;

    [Header("Idle")]
    [SerializeField] private float _attackRange;
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

    [Header("SlimeResidue")]
    [SerializeField] private GameObject _slimeResidue;
    [SerializeField] private GameObject _slimeResidueSpriteMask;
    private readonly float leftEdgeOffset = -0.665f;
    private readonly float rightEdgeOffset = 0.6475f;

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
        // Check if player is in range to attack
        if (PlayerManager.Instance.PlayerCombat.transform.position.x > _attackRange)
        {
            return;
        }

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
        InstantiateSlimeResidue();
    }

    private void InstantiateSlimeResidue()
    {
        // Spawn residue underneath slime
        Instantiate(_slimeResidue, GroundCheck.position, Quaternion.identity);
        // Get the left bottom edge of the slime
        Vector2 leftEdge = new Vector2(this.transform.position.x + leftEdgeOffset, GroundCheck.position.y);
        // List of tuples to store raycast, (x offset and hit ground or not)
        List<(float, bool)> groundChecks = new List<(float, bool)>();
        // Do raycasts starting from the left ending at the right
        for (float xOffset = 0; xOffset < rightEdgeOffset - leftEdgeOffset; xOffset += 0.1f)
        {
            float raycastXCoordinate = leftEdge.x + xOffset;
            Vector2 raycastPoint = new Vector2(raycastXCoordinate, leftEdge.y);
            RaycastHit2D raycastGroundCheck = Physics2D.Raycast(raycastPoint, Vector2.down, 5f, GroundLayer);
            bool hitGround = raycastGroundCheck.distance < 0.1f ? true : false;
            // Note down the sections where ground ends and ground starts
            groundChecks.Add(new(raycastXCoordinate, hitGround));
            Debug.DrawRay(raycastPoint, Vector2.down);
        }
        // In between the sections where the ground starts and ends
        // Spawn in a sprite mask with of that size with a box collider
        (float, bool) startingGroundCheck = groundChecks[0];
        foreach ((float, bool) groundCheck in groundChecks)
        {
            // If the starting ground check and the new ground both hit the ground or both haven't hit ground, continue
            if (startingGroundCheck.Item2 == groundCheck.Item2)
            {
                continue;
            }
            else if (startingGroundCheck.Item2)
            {
                // If the starting ground check hit ground and the new one didn't, spawn a sprite mask
                //Spawn it centered between starting ground check and new ground check
                float distance = groundCheck.Item1 - startingGroundCheck.Item1;
                Vector2 spawnPosition = new Vector2(startingGroundCheck.Item1 + distance / 2, GroundCheck.position.y);
                GameObject spriteMask = Instantiate(_slimeResidueSpriteMask, spawnPosition, Quaternion.identity);
                // Set the scale to be the distance between the two groundchecks
                spriteMask.transform.localScale = new Vector2(distance, 1f);
                // Update starting ground check to the new ground check
                startingGroundCheck = groundCheck;
            }
            else if (!startingGroundCheck.Item2)
            {
                // If the starting ground check didn't hit ground and the new one did, update starting ground check
                startingGroundCheck = groundCheck;
            }
        }

        // If the starting ground is still touching the ground, you need to draw the sprite mask from starting ground to the last ground check
        if (startingGroundCheck.Item2)
        {
            float distance = groundChecks[groundChecks.Count - 1].Item1 - startingGroundCheck.Item1;
            Vector2 spawnPosition = new Vector2(startingGroundCheck.Item1 + distance / 2, GroundCheck.position.y);
            GameObject spriteMask = Instantiate(_slimeResidueSpriteMask, spawnPosition, Quaternion.identity);
            // Set the scale to be the distance between the two groundchecks
            spriteMask.transform.localScale = new Vector2(distance, 1f);
        }

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
