using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class Entity : MonoBehaviour, IGridEntity
{
    public event Action<IGridEntity> OnMove;

    public Team team;
    public int life;
    public float maxSpeed;
    public float maxForce;
    public float viewRadius;
    public float viewAngle;

    public Image lifeBar;
    public TMP_Text text;
    public Transform bodyTransform;

    public Entity enemy;
    public bool isDead = false;

    [SerializeField] LayerMask obstacleMask;
    [SerializeField] LayerMask wallLayer;

    public Node startingNode;
    public Node goalNode;

    protected List<Node> pathToFollow = new List<Node>();

    protected List<Node> pathToPatrol = new List<Node>();

    protected Pathfinding pathFinding = new Pathfinding();

    public Vector3 velocity;

    public int maxLife;

    GameObject[] agents;

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    private void Awake()
    {
        agents = GameObject.FindGameObjectsWithTag("Wall");
    }
    public virtual void Start()
    {
        maxLife = life;
        LevelManager.instance.AddBoid(this);
        AddForce(CalculateSteering(bodyTransform.forward));
    }

    public virtual void Update()
    {
        transform.position = transform.position.CheckBounds();
    }

    public void SetPath(List<Node> path)
    {
        pathToFollow = path;
    }

    public void FollowPath()
    {
        if (pathToFollow.Count > 0)
            AddForce(Seek(pathToFollow[0].transform.position));
        else return;

        if (Vector3.Distance(transform.position, pathToFollow[0].transform.position) < 0.1f)
        {
            pathToFollow[0].neighbors.Clear();
            pathToFollow.RemoveAt(0);
        }
    }

    public void AddForce(Vector3 force)
    {
        velocity = Vector3.ClampMagnitude(velocity + force, maxSpeed);
    }

    public Vector3 CalculateSteering(Vector3 desired)
    {
        return Vector3.ClampMagnitude((desired.normalized * maxSpeed) - velocity, maxForce);
    }

    public Vector3 Seek(Vector3 target)
    {
        return CalculateSteering(target - transform.position);
    }

    public Vector3 AvoidObstacles()
    {
        var obstacles = Physics.OverlapSphere(transform.position, viewRadius, obstacleMask);
        if (obstacles.Length == 0) return Vector3.zero;

        foreach (var item in obstacles)
        {
            if (Vector3.Angle(transform.forward, item.transform.position - transform.position) <= 40)
                return CalculateSteering(transform.up);
        }
        return Vector3.zero;
    }

    public Vector3 WallSeparation()
    {
        Vector3 desired = Vector3.zero;

        foreach (var boid in agents)
        {
            Vector3 dist = boid.transform.position - transform.position;
            if (dist.magnitude <= viewRadius)
                desired += dist;
        }
        if (desired == Vector3.zero) return desired;
        desired = -desired;

        return CalculateSteering(desired);
    }

    public Vector3 Separation()
    {
        Vector3 desired = Vector3.zero;

        foreach (var item in LevelManager.instance.allNpc)
        {
            Vector3 dist = item.transform.position - transform.position;
            if (dist.magnitude <= viewRadius)
                desired += dist;
        }
        if (desired == Vector3.zero) return desired;
        desired = -desired;

        return CalculateSteering(desired);
    }

    public Vector3 Alignment()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;
        foreach (var item in LevelManager.instance.allNpc)
        {
            if (item == this) continue;
            if (Vector3.Distance(item.transform.position, transform.position) <= viewRadius)
            {
                desired += item.velocity;
                count++;
            }
        }
        if (count == 0) return desired;
        desired /= count;

        return CalculateSteering(desired);
    }

    public Vector3 Cohesion()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;
        foreach (var boid in LevelManager.instance.allNpc)
        {
            if (boid == this) continue;
            if (Vector3.Distance(transform.position, boid.transform.position) <= viewRadius)
            {
                desired += boid.transform.position;
                count++;
            }
        }
        if (count == 0) return desired;
        desired /= count;
        desired -= transform.position;

        return CalculateSteering(desired);
    }

    public Vector3 Arrive(Vector3 targetPos)
    {
        Vector3 desired = targetPos - transform.position;
        float dist = desired.magnitude;
        desired.Normalize();
        if (dist <= viewRadius)
            desired *= maxSpeed * (dist / viewRadius);
        else
            desired *= maxSpeed;

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);

        return steering;
    }

    public bool InFOV(GameObject obj)
    {
        if (Vector3.Distance(transform.position, obj.transform.position) > viewRadius) return false;

        if (Vector3.Angle(bodyTransform.forward, obj.transform.position - transform.position) > (viewAngle / 2)) return false;

        return InLineOfSight(transform.position, obj.transform.position);

    }

    public bool InLineOfSight(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        return !Physics.Raycast(start, dir, dir.magnitude, wallLayer);
    }

    public virtual void ReceiveDamage(int damage)
    {
        life -= damage;
        lifeBar.fillAmount = (float)life / (float)maxLife;
    }

    public virtual void ReceiveHealing(int healing)
    {
        life += healing;
        if (life > maxLife) life = maxLife;
        lifeBar.fillAmount = (float)life / (float)maxLife;
    }

    public void CalculatePathFindingToEscape()
    {
        startingNode = LevelManager.instance.allNodes.Select(x => (Node)x)
                                                     .Where(x => x != null)
                                                     .Where(x => InLineOfSight(x.transform.position, transform.position) == true)
                                                     .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                                                     .First();

        goalNode = LevelManager.instance.allNodes.Select(x => (Node)x)   //IA2-LINQ
            .Where(x => x != null)
            .Where(x =>
            {
                foreach (var npc in LevelManager.instance.allNpc)
                {
                    if (InLineOfSight(x.transform.position, npc.transform.position) || x.used) return false;
                }
                return true;
            })
            .OrderByDescending(x => Vector3.Distance(x.transform.position, transform.position))
            .First();

        /*if (startingNode == null || goalNode == null)
            CalculatePathFindingToEscape();
        else
        {*/
            goalNode.used = true;
            SetPath(pathFinding.AStar(startingNode, goalNode));
        //}
    }

    public bool ReturnToCombat()
    {
        if (pathToFollow.Count == 0 && life == maxLife) return true;
        else return false;
    }

    /*public Vector3 Persuit(Transform agentToPersuit)
    {
        //Esto es la base de FuturePos, la manera correcta es la que mejor se aplique a su proyecto
        var agentToPersuitVelocity = agentToPersuit.gameObject.GetComponent<Entity>().velocity;

        Vector3 futurePos = agentToPersuit.position + agentToPersuitVelocity * Time.deltaTime;//* (agent.transform.position - transform.position).magnitude;
        Vector3 desired = (futurePos - transform.position);
        desired.Normalize();
        desired *= 1;

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);

        Debug.Log("PERSUIT");
        return steering;
    }*/

    public void Persuit()
    {
        if(enemy != null)
        {
            /*if (Vector3.Distance(transform.position, enemy.transform.position) > 1)
            {
                AddForce(enemy.transform.position);

                velocity.y = 0;
                transform.position += velocity * Time.deltaTime;
                if (velocity != Vector3.zero)
                    bodyTransform.forward = velocity;
            }*/
            if (Vector3.Distance(transform.position, enemy.transform.position) > 1)
            {
                transform.position = Vector3.MoveTowards(transform.position, enemy.transform.position, 1 * Time.deltaTime);
                bodyTransform.LookAt(enemy.transform);
            }
        }  
    }
}
