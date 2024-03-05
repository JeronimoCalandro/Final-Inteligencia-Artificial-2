using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    public List<Node> neighbors = new List<Node>();
    public bool used = false;
    public int cost = 1;
    public LayerMask wallLayer;

    private void Start()
    {
        LevelManager.instance.AddNode(this);
    }

    public List<Node> GetNeighbors()
    {
        if (neighbors.Count == 0)
        {
            foreach (var item in LevelManager.instance.allNodes)
            {
                if (InLineOfSight(transform.position, item.transform.position))
                {
                    if(item != this) neighbors.Add(item);
                }
            }
        }
        return neighbors;
    }
    bool InLineOfSight(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        return !Physics.Raycast(start, dir, dir.magnitude, wallLayer);
    }

}
