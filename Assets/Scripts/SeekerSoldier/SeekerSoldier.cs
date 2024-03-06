using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum SeekerSoldierStates
{
    Follow,
    Search,
    Seek,
    Dead,
}

public class SeekerSoldier : Agent<SeekerSoldierStates>
{
    public GameObject flag;
    public Vector3 targetPosition;

    public override void Start()
    {
        base.Start();
        targetPosition = transform.position;

        fsm.AddState(SeekerSoldierStates.Follow, new SeekerSoldierFollowState(fsm, this));
        fsm.AddState(SeekerSoldierStates.Search, new SeekerSoldierSearchState(fsm, this));
        fsm.AddState(SeekerSoldierStates.Seek, new SeekerSoldierSeekState(fsm, this));
        fsm.AddState(SeekerSoldierStates.Dead, new SeekerSoldierDeathState(fsm, this));
        fsm.ChangeState(SeekerSoldierStates.Seek);
    }

    public override void Update()
    {
        base.Update();
        fsm.Update();
    }

    public void Seek()
    {
        if (pathToFollow.Count > 0 || !InLineOfSight(transform.position, targetPosition))
            FollowPath();
        else
            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                AddForce(Arrive(targetPosition));

        velocity.y = 0;
        transform.position += velocity * Time.deltaTime;
        if (velocity != Vector3.zero)
            bodyTransform.forward = velocity;

        AddForce(AvoidObstacles() * 1.3f);
    }

    public void Follow()
    {
        if (InLineOfSight(transform.position, leaderTransform.position))
        {
            if (Vector3.Distance(leaderTransform.position, transform.position) > viewRadius / 2)
            {
                AddForce(Seek(leaderTransform.position));
            }

            Flocking();
        }
        else
            fsm.ChangeState(SeekerSoldierStates.Search);
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
            fsm.ChangeState(SeekerSoldierStates.Follow);
    }

    public void SetNewRandomPosition()
    {
        targetPosition = new Vector3(Random.Range(-20, 20), 0, Random.Range(-10, 10));

        if (!LevelManager.instance.allNodes.Any(x => InLineOfSight(x.transform.position, targetPosition)))
            SetNewRandomPosition();

        if (InLineOfSight(transform.position, targetPosition))
            return;

        startingNode = LevelManager.instance.allNodes.Select(x => (Node)x)            //IA2-LINQ
                                                     .Where(x => InLineOfSight(x.transform.position, transform.position) == true)
                                                     .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                                                     .First();

        goalNode = LevelManager.instance.allNodes.Select(x => (Node)x)
                                                 .Where(x => InLineOfSight(x.transform.position, targetPosition) == true)
                                                 .OrderBy(x => Vector3.Distance(x.transform.position, targetPosition))
                                                 .First();

        //if (startingNode == null || goalNode == null) SetNewRandomPosition();

        SetPath(pathFinding.AStar(startingNode, goalNode));
    }

    public override void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);
        if (life <= 0)
            fsm.ChangeState(SeekerSoldierStates.Dead);
    }
}
