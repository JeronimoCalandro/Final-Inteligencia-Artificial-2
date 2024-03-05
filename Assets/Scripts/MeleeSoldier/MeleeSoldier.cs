using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum MeleeSoldierStates
{
    Follow, 
    Search,
    Attack,
    Escape,
    Dead,
}

public class MeleeSoldier : Agent<MeleeSoldierStates>
{
    public override void Start()
    {
        base.Start();

        fsm.AddState(MeleeSoldierStates.Follow, new MeleeSoldierFollowLeaderState(fsm, this));
        fsm.AddState(MeleeSoldierStates.Search, new MeleeSoldierSearchLeaderState(fsm, this));
        fsm.AddState(MeleeSoldierStates.Attack, new MeleeSoldierAttackState(fsm, this));
        fsm.AddState(MeleeSoldierStates.Escape, new MeleeSoldierEscapeState(fsm, this));
        fsm.AddState(MeleeSoldierStates.Dead, new MeleeSoldierDeadState(fsm, this));
        fsm.ChangeState(MeleeSoldierStates.Follow);
    }

    public override void Update()
    {
        base.Update();
        fsm.Update();
    }

    public void Follow()
    {
        if (InLineOfSight(transform.position, leaderTransform.position))
        {
            if (Vector3.Distance(leaderTransform.position, transform.position) > viewRadius / 2)
            {
                //AddForce(Arrive(leader.position));
                AddForce(Seek(leaderTransform.position));
            }

            Flocking();
        }
        else
            fsm.ChangeState(MeleeSoldierStates.Search);
    }

    public void Search()
    {
        if (!InLineOfSight(transform.position, leaderTransform.position))
        {
            Flocking();
            if (pathToFollow.Count > 0)
                FollowPath();
            else
                CalculatePathFindingToLeader();
        }
        else
            fsm.ChangeState(MeleeSoldierStates.Follow);
    }

    public override void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);
        if (life <= 0)
            fsm.ChangeState(MeleeSoldierStates.Dead);
        else if (life == 1)
            fsm.ChangeState(MeleeSoldierStates.Escape);
    }
}
