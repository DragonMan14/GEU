using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSpider : Enemy
{
    private enum State
    {
        Patrolling,
        Jumping,
        WebShot,
    }
    // Start is called before the first frame update
    void Start()
    {
        SetState(State.Patrolling);
    }

    #region Patrolling

    private void EnterPatrollingState()
    {

    }

    #endregion

    #region Jumping
    #endregion

    #region WebShot
    #endregion

    private void SetState(State newState)
    {
       
    }

    public override void UpdateCurrentState()
    {
       
    }
}
