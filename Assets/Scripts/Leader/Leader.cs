using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum LeaderStates
{
    Move,
    Attack,
    Escape,
    Dead,
}
public class Leader : Entity
{
    public GameObject UIHeal;

    public SquareQuery squareQuery;

    public Vector3 targetPosition;

    FiniteStateMachine<LeaderStates> fsm;

    bool canHeal = true;

    public override void Start()
    {
        base.Start();
        targetPosition = transform.position;

        fsm = new FiniteStateMachine<LeaderStates>();

        fsm.AddState(LeaderStates.Move, new LeaderMovementState(fsm, this));
        fsm.AddState(LeaderStates.Attack, new LeaderAttackState(fsm, this));
        fsm.AddState(LeaderStates.Escape, new LeaderEscapeState(fsm, this));
        fsm.AddState(LeaderStates.Dead, new LeaderDeadState(fsm, this));

        fsm.ChangeState(LeaderStates.Move);
    }

    public override void Update()
    {
        base.Update();
        fsm.Update();
    }

    public void Movement()
    {
        if (Input.GetMouseButtonUp(0) && team == Team.Red)
            SetNewRandomPosition(Vector3.zero);
        else if (Input.GetMouseButtonUp(1) && team == Team.Blue)
            SetNewRandomPosition(Vector3.zero);

        /*if (canHeal)
        {
            if (Input.GetKeyDown(KeyCode.A) && team == Team.Red)
                Heal();
            else if (Input.GetKeyDown(KeyCode.D) && team == Team.Blue)
                Heal();
        }*/
        

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

    public void SetNewRandomPosition(Vector3 position)
    {
        if (position == Vector3.zero)
            targetPosition = new Vector3(Random.Range(-22, 22), 0, Random.Range(-10, 10));
        else targetPosition = position;

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

        if (startingNode == null || goalNode == null) SetNewRandomPosition(Vector3.zero);

        SetPath(pathFinding.AStar(startingNode, goalNode));
    }

    public override void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);
        if (life <= 0)
            fsm.ChangeState(LeaderStates.Dead);
        /*else if (life == 1)
            fsm.ChangeState(LeaderStates.Escape);*/
    }

    public void Heal()  //IA2-LINQ
    {
        var npcs = squareQuery.Query()
                              .Select(x => (Entity)x)
                              .Where(x => x != null)
                              .Where(x => !x.isDead)
                              .Where(x => x.team == team);
                              /*.OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                              .Take(1);*/

        foreach (var npc in npcs)
        {
            var soldier = npc.GetComponent<Entity>();
            soldier.ReceiveHealing(10);
        }
        canHeal = false;
        UIHeal.SetActive(false);
    }

    public void Escape()
    {
        if (pathToFollow.Count > 0)
            FollowPath();
        else
            AddForce(Arrive(goalNode.transform.position));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 lineA = GetVectorFromAngle(viewAngle / 2 + transform.eulerAngles.z);
        Vector3 lineB = GetVectorFromAngle(-viewAngle / 2 + transform.eulerAngles.z);

        Gizmos.DrawLine(transform.position, transform.position + lineA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + lineB * viewRadius);

    }

    Vector3 GetVectorFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
