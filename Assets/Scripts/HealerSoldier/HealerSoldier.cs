using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum HealerSoldierStates
{
    Follow,
    Lost,
    Search,
    Heal,
    Dead,
}

public class HealerSoldier : Agent<HealerSoldierStates>
{
    public Transform originalLeaderTransform;
    public Entity agentToHeal;
    public SquareQuery squareQuery;

    public override void Start()
    {
        base.Start();
        originalLeaderTransform = leaderTransform;

        fsm.AddState(HealerSoldierStates.Follow, new HealerSoldierFollowLeaderState(fsm, this));
        fsm.AddState(HealerSoldierStates.Lost, new HealerSoldierLostState(fsm, this));
        fsm.AddState(HealerSoldierStates.Heal, new HealerSoldierHealState(fsm, this));
        fsm.AddState(HealerSoldierStates.Search, new HealerSoldierSearchState(fsm, this));
        fsm.AddState(HealerSoldierStates.Dead, new HealerSoldierDeadState(fsm, this));

        fsm.ChangeState(HealerSoldierStates.Follow);
    }

    public override void Update()
    {
        base.Update();
        fsm.Update();
    }

    public void Follow(bool healing)
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
        {
            if(healing)
                fsm.ChangeState(HealerSoldierStates.Search);
            else
                fsm.ChangeState(HealerSoldierStates.Lost);
        }
    }

    public void Lost()
    {
        if (!InLineOfSight(transform.position, leaderTransform.position))
        {
            Flocking();
            if (pathToFollow.Count > 0)
                FollowPath();
            else
                CalculateRandomPathFinding();
        }
        else
            fsm.ChangeState(HealerSoldierStates.Follow);
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
            fsm.ChangeState(HealerSoldierStates.Heal);
    }

    public void Heal()
    {
        if (LevelManager.instance.allNpc.Where(x => x != null).Where(x => x.team == team).Any(x => x.life < x.maxLife))
        {

        }

        startingNode = LevelManager.instance.allNodes.Select(x => (Node)x).Where(x => x != null).Where(x => InLineOfSight(x.transform.position, transform.position) == true).OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).FirstOrDefault();
    }

    public override void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);
        if (life <= 0)
            fsm.ChangeState(HealerSoldierStates.Dead);
    }
}
