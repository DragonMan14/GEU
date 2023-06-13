using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BasicSpider : Enemy
{
    private enum State
    {
        Patrolling,
        Angry,
        Jumping,
        WebShot,
    }

    private enum Orientation
    {
        Upright,
        UpsideDown,
        ClimbingRight,
        ClimbingLeft,
    }

    private State _currentState;
    private State _previousState;
    private int _consecutiveSameAttackCounter;
    private readonly int _maxStall = 3;


    // Custom implementation of gravity
    [Header("Gravity")]
    private Vector2 _gravityDirection;
    private readonly float _gravityScale = 9.81f;
    private readonly float _gravityStrength = 1.0f;

    [Header("Rotation")]
    private Orientation _orientation;
    private readonly float _rotationDuration = 0.01f;
    private float _rotationCooldown; // After a rotating around an edge, wait the cooldown until you can rotate again

    [Header("Patrolling")]
    [SerializeField] private float _patrollingMoveSpeed;
    [SerializeField] private List<GameObject> _patrolPoints;
    private int _currentPatrolPoint;

    [Header("Angry")]
    [SerializeField] private float _angryMoveSpeed;
    [SerializeField] private float _attackRadius;

    [Header("Jumping")]
    [SerializeField] private float _jumpForce;

    [Header("WebShot")]
    [SerializeField] private GameObject _webShot;

    // Start is called before the first frame update
    void Start()
    {
        _orientation = Orientation.Upright;

        Rigidbody.gravityScale = 0;
        _gravityDirection = Vector2.down;

        _rotationCooldown = _rotationDuration;

        SetState(State.Patrolling);
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    public override void CustomUpdate()
    {
        _rotationCooldown -= Time.deltaTime;
    }

    public override void UpdateFacing()
    {
        if (_currentState == State.Patrolling)
        {
            return;
        }
        bool playerOnRight = PlayerManager.Instance.PlayerCombat.transform.position.x > this.gameObject.transform.position.x;
        if (playerOnRight && Direction == Facing.Left)
        {
            FlipRotation();
        }
        else if (!playerOnRight && Direction == Facing.Right)
        {
            FlipRotation();
        }
    }

    private void SetVelocity()
    {
        float speed = _currentState == State.Patrolling ? _patrollingMoveSpeed : _angryMoveSpeed;
        if (_orientation == Orientation.Upright)
        {
            Rigidbody.velocity = Direction == Facing.Right ? new Vector2(speed, 0) : new Vector2(-speed, 0);
        }
        else if (_orientation == Orientation.UpsideDown)
        {
            Rigidbody.velocity = Direction == Facing.Right ? new Vector2(-speed, 0) : new Vector2(speed, 0);
        }
        else if (_orientation == Orientation.ClimbingRight)
        {
            Rigidbody.velocity = Direction == Facing.Right ? new Vector2(0, speed) : new Vector2(0, -speed);
        }
        else if (_orientation == Orientation.ClimbingLeft)
        {
            Rigidbody.velocity = Direction == Facing.Right ? new Vector2(0, -speed) : new Vector2(0, speed);
        }
    }

    #region Orientation

    private void IncreaseOrientation(string mode)
    {
        print("increase");
        // If rotation is on cooldown, don't rotate
        if (_rotationCooldown > 0)
        {
            return;
        }
        _rotationCooldown = _rotationDuration;

        Rigidbody.velocity = Vector2.zero;
        if (mode == "Wall")
        {
            this.transform.Rotate(new Vector3(0, 0, 90));
        }
        else if (mode == "Edge")
        {
            this.transform.RotateAround(GroundCheck.position, Vector3.forward, 90);
        }
        switch (_orientation)
        {
            case Orientation.Upright:
                _orientation = Orientation.ClimbingRight;
                _gravityDirection = Vector2.right;
                break;
            case Orientation.ClimbingRight:
                _orientation = Orientation.UpsideDown;
                _gravityDirection = Vector2.up;
                break;
            case Orientation.UpsideDown:
                _orientation = Orientation.ClimbingLeft;
                _gravityDirection = Vector2.left;
                break;
            case Orientation.ClimbingLeft:
                _orientation = Orientation.Upright;
                _gravityDirection = Vector2.down;
                break;
        }
        SnapToGround();
    }

    private void DecreaseOrientation(string mode)
    {
        print("decrease");
        // If rotation is on cooldown, don't rotate
        if (_rotationCooldown > 0)
        {
            return;
        }
        _rotationCooldown = _rotationDuration;


        Rigidbody.velocity = Vector2.zero;
        if (mode == "Wall")
        {
            this.transform.Rotate(new Vector3(0, 0, -90));
        }
        else if (mode == "Edge")
        {
            this.transform.RotateAround(GroundCheck.position, Vector3.forward, -90);
        }
        switch (_orientation)
        {
            case Orientation.Upright:
                _orientation = Orientation.ClimbingLeft;
                _gravityDirection = Vector2.left;
                break;
            case Orientation.ClimbingLeft:
                _orientation = Orientation.UpsideDown;
                _gravityDirection = Vector2.up;
                break;
            case Orientation.UpsideDown:
                _orientation = Orientation.ClimbingRight;
                _gravityDirection = Vector2.right;
                break;
            case Orientation.ClimbingRight:
                _orientation = Orientation.Upright;
                _gravityDirection = Vector2.down;
                break;
        }
        SnapToGround();
    }

    private void SnapToGround()
    {
        float snappingOffset = 0.1f; // The offset from the groundcheck position where the raycast is cast
        // Raycast from the bottom of the spider to the nearest ground it can stand on and find the distance
        Vector3 snapPosition = GroundCheck.position;

        if (_orientation == Orientation.Upright && Direction == Facing.Right || _orientation == Orientation.UpsideDown && Direction == Facing.Left)
        {
            snapPosition.x += snappingOffset;
        }
        else if (_orientation == Orientation.UpsideDown && Direction == Facing.Right || _orientation == Orientation.Upright && Direction == Facing.Left)
        {
            snapPosition.x -= snappingOffset;
        }
        else if (_orientation == Orientation.ClimbingLeft && Direction == Facing.Left || _orientation == Orientation.ClimbingRight && Direction == Facing.Right)
        {
            snapPosition.y += snappingOffset;
        }
        else if (_orientation == Orientation.ClimbingRight && Direction == Facing.Left || _orientation == Orientation.ClimbingLeft && Direction == Facing.Right)
        {
            snapPosition.y -= snappingOffset;
        }

        float distance = Physics2D.Raycast(snapPosition, _gravityDirection, 1f, EnvironmentLayer).distance;
        // Adjust the spider's positon by the distance
        Vector3 position = this.transform.position;
        if (_orientation == Orientation.Upright)
        {
            position.y -= distance;
        }
        else if (_orientation == Orientation.UpsideDown)
        {
            position.y += distance;
        }
        else if (_orientation == Orientation.ClimbingLeft)
        {
            position.x -= distance;
        }
        else if (_orientation == Orientation.ClimbingRight)
        {
            position.x += distance;
        }
        this.transform.position = position;
    }

    private void ApplyGravity()
    {
        Rigidbody.AddForce(2 * _gravityScale * _gravityStrength * _gravityDirection, ForceMode2D.Force);
    }

    #endregion

    #region Patrolling

    private void EnterPatrollingState()
    {
        _currentPatrolPoint = 0;
        // Move to the nearest patrol point
    }

    private void UpdatePatrollingState()
    {
        // If the list of patrol points is empty, move along the current platform
        // If the spider hits a wall or reaches the end of the platform, turn around
        if (_patrolPoints.Count == 0)
        {
            SetVelocity();
            if (IsCollidingWithWall() || IsAtEdgeOfGround())
            {
                FlipRotation();
            }
        }
        else
        {
            // Else if the spider has set patrol points assigned in the inspector
            // For each patrol point, attempt to move form the current position to it
            SetVelocity();
            // If it reaches a wall, climb up the wall
            if (IsCollidingWithWall())
            {
                // If facing right, rotate the z axis by 90
                if (Direction == Facing.Right) 
                {
                    IncreaseOrientation("Wall");
                }
                // If facing left, rotate the z axis by -90
                else if (Direction == Facing.Left)
                {
                    DecreaseOrientation("Wall");
                }
            }
            // If it reaches the edge of the platform, climb down the side
            if (IsCenteredAtEdgeOfGround())
            {
                // If facing right, rotate the z axis by -90
                if (Direction == Facing.Right)
                {
                    DecreaseOrientation("Edge");
                }
                // If facing left, rotate the z axis by 90
                else if (Direction == Facing.Left)
                {
                    IncreaseOrientation("Edge");
                }
            }
            // When it reaches the patrol point, move on to the next patrol point
            if (Vector2.Distance(this.transform.position, _patrolPoints[_currentPatrolPoint].transform.position) < 0.4f)
            {
                print("Reached patrol point");
                _currentPatrolPoint++;
                _currentPatrolPoint %= _patrolPoints.Count;
                // If the next patrol point is in the opposite direction the spider is facing, flip the spider
                Facing patrolDirection = this.transform.position.x > _patrolPoints[_currentPatrolPoint].transform.position.x ? Facing.Left : Facing.Right;
                if (patrolDirection != Direction)
                {
                    FlipRotation();
                }
            }
            // Go to the starting patrol point and restart the whole cycle
        }
        float distanceToPlayer = Vector2.Distance(PlayerManager.Instance.PlayerCombat.transform.position, this.transform.position);
        if (distanceToPlayer <= _attackRadius)
        {
            SetState(State.Angry);
        }
    }

    private void ExitPatrollingState()
    {

    }

    #endregion

    #region Angry

    private void EnterAngryState()
    {

    }

    private void UpdateAngryState()
    {
        // Pathfind to the player
        SetVelocity();
        // If it encounters a wall, climb over it
        

    }

    private void ExitAngryState()
    {

    }

    #endregion

    #region Jumping

    private void EnterJumpingState()
    {

    }

    private void UpdateJumpingState()
    {

    }

    private void ExitJumpingState()
    {

    }

    #endregion

    #region WebShot

    private void EnterWebShotState()
    {

    }

    private void UpdateWebShotState()
    {

    }

    private void ExitWebShotState()
    {

    }

    #endregion

    #region StateMachine

    private void SetState(State newState)
    {
        // Exit the current state
        switch(_currentState)
        {
            case State.Patrolling:
                ExitPatrollingState(); 
                break;
            case State.Angry:
                ExitAngryState();
                break;
            case State.Jumping:
                ExitJumpingState();
                break;
            case State.WebShot:
                ExitWebShotState();
                break;
        }
        // Enter the new state
        switch(newState)
        {
            case State.Patrolling:
                EnterPatrollingState();
                break;
            case State.Angry:
                EnterAngryState();
                break;
            case State.Jumping:
                EnterJumpingState();
                break;
            case State.WebShot:
                EnterWebShotState();
                break;
        }
        _previousState = _currentState;
        _currentState = newState;
    }

    public override void UpdateCurrentState()
    {
        switch (_currentState)
        {
            case State.Patrolling:
                UpdatePatrollingState();
                break;
            case State.Angry:
                UpdateAngryState();
                break;
            case State.Jumping:
                UpdateJumpingState();
                break;
            case State.WebShot:
                UpdateWebShotState();
                break;
        }
    }

    #endregion
}
