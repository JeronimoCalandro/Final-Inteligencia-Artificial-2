using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public enum Team
{
    Red,
    Blue,
}
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float boundHeight;
    public float boundWidth;

    public List<Node> allNodes = new List<Node>();

    public List<Entity> allNpc = new List<Entity>();

    public List<GameObject> allCoins = new List<GameObject>();

    public LayerMask wallMask;

    public TMP_Text finishText;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        var rnd = new System.Random();
        var coinsInGame = allCoins.OrderBy(x => rnd.Next()).Take(allCoins.Count / 2);   //IA2-LINQ
        allCoins = coinsInGame.ToList();
        foreach (var coin in allCoins)
        {
            coin.SetActive(true);
        }

        //boundHeight = boundHeight - 9.5f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }
    }

    public void AddBoid(Entity entity)
    {
        if (allNpc.Contains(entity)) return;

        allNpc.Add(entity);
    }

    public void RemoveBoid(Entity entity)
    {
        if (allNpc.Contains(entity))
            allNpc.Remove(entity);

        if (allNpc.All(x => x.team != entity.team)) FinishGame(allNpc[0].team);   //IA2-LINQ

    }

    public void AddNode(Node node)
    {
        if (allNodes.Contains(node)) return;

        allNodes.Add(node);
    }

    public void RemoveCoin(GameObject coin)
    {
        if (allCoins.Contains(coin)) allCoins.Remove(coin);

        coin.GetComponent<MeshRenderer>().enabled = false;
    }

    public void FinishGame(Team team)
    {
        finishText.enabled = true;
        finishText.text = ("Gano el equipo " + team);
        Time.timeScale = 0;
    }

    void OnDrawGizmos()                // Representacion grafica de los limites para el wraparound
    {
        Gizmos.color = Color.red;
        Vector3 topLeft = new Vector3(-boundWidth / 2, 0, boundHeight / 2);
        Vector3 topRight = new Vector3(boundWidth / 2, 0, boundHeight / 2);
        Vector3 botRight = new Vector3(boundWidth / 2, 0, -boundHeight / 2);
        Vector3 botLeft = new Vector3(-boundWidth / 2, 0, -boundHeight / 2);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, botRight);
        Gizmos.DrawLine(botRight, botLeft);
        Gizmos.DrawLine(botLeft, topLeft);
    }
}

public static class CheckBoundsExtension    // Clase estatica para realizar una extension y poder hacer wraparound con los limites del mapa
{
    public static Vector3 CheckBounds(this Vector3 objectPosition)
    {
        if (objectPosition.x > LevelManager.instance.boundWidth / 2) objectPosition.x = -LevelManager.instance.boundWidth / 2;
        if (objectPosition.x < -LevelManager.instance.boundWidth / 2) objectPosition.x = LevelManager.instance.boundWidth / 2;
        if (objectPosition.z < -LevelManager.instance.boundHeight / 2) objectPosition.z = LevelManager.instance.boundHeight / 2; 
        if (objectPosition.z > LevelManager.instance.boundHeight / 2) objectPosition.z = -LevelManager.instance.boundHeight / 2;

        return objectPosition;
    }
}
