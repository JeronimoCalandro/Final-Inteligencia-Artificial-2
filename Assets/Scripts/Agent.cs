using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Agent<T> : Entity
{
    [Header("Leader")]
    public Transform leaderTransform;

    [Header("Flocking")]
    [Range(0f, 3f)]
    public float separationWeight = 1;
    [Range(0f, 2f)]
    public float alignmentWeight = 1;
    [Range(0f, 2f)]
    public float cohesionWeight = 1;

    public Rigidbody rigidbody;

    protected FiniteStateMachine<T> fsm;

    float originalSeparationWeight, originalAlignmentWeight, originalCohesionWeight;

    public override void Start()
    {
        base.Start();

        fsm = new FiniteStateMachine<T>();
        originalSeparationWeight = separationWeight;
        originalCohesionWeight = cohesionWeight;
        originalAlignmentWeight = alignmentWeight;
    }

    public override void Update()
    {
        base.Update();
    }

    public void Escape()
    {
        velocity.y = 0;
        transform.position += velocity * Time.deltaTime;
        if (velocity != Vector3.zero)
            bodyTransform.forward = velocity;

        if (pathToFollow.Count > 0)
            FollowPath();
        else
            AddForce(Arrive(goalNode.transform.position));
    }

    public void CalculatePathFindingToLeader()
    {
        startingNode = LevelManager.instance.allNodes.Select(x => (Node)x)
                                                     .Where(x => x != null)
                                                     .Where(x => InLineOfSight(x.transform.position, transform.position) == true)
                                                     .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                                                     .FirstOrDefault();

        goalNode = LevelManager.instance.allNodes.Select(x => (Node)x).Where(x => x != null).Where(x => InLineOfSight(x.transform.position, leaderTransform.position) == true).OrderBy(x => Vector3.Distance(x.transform.position, leaderTransform.position)).FirstOrDefault();

        SetPath(pathFinding.AStar(startingNode, goalNode));
    }

    public void CalculateRandomPathFinding()
    {
        startingNode = LevelManager.instance.allNodes.Select(x => (Node)x).Where(x => x != null).Where(x => InLineOfSight(x.transform.position, transform.position) == true).OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).FirstOrDefault();

        var randomGoalNodes = LevelManager.instance.allNodes.Select(x => (Node)x).Where(x => x != null).Where(x => !InLineOfSight(x.transform.position, transform.position) == true).OrderByDescending(x => Vector3.Distance(x.transform.position, leaderTransform.position)).Take(5).ToList();

        goalNode = randomGoalNodes[Random.Range(0, randomGoalNodes.Count)];

        /*if (startingNode == null || goalNode == null)
            CalculateRandomPathFinding();
        else*/
            SetPath(pathFinding.AStar(startingNode, goalNode));
    }

    public void Flocking()
    {
        //AddForce(AvoidObstacles() * 1.3f);
        if(WallSeparation() != Vector3.zero)
        {
            separationWeight = 0;
            cohesionWeight = 0;
            alignmentWeight = 0;
        }
        else
        {
            separationWeight = originalSeparationWeight;
            cohesionWeight = originalCohesionWeight;
            alignmentWeight = originalAlignmentWeight;
        }
        AddForce(WallSeparation() * 10);
        AddForce(Separation() * separationWeight);
        AddForce(Cohesion() * cohesionWeight);
        AddForce(Alignment() * alignmentWeight);

        velocity.y = 0;
        transform.position += velocity * Time.deltaTime;
        if (velocity != Vector3.zero)
            bodyTransform.forward = velocity;
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
