using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public enum Team
{
    Rojo,
    Azul,
}
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float boundHeight;
    public float boundWidth;

    public List<Node> allNodes = new List<Node>();

    public List<Entity> allNpc = new List<Entity>();

    public List<GameObject> allCoins = new List<GameObject>();

    public List<Transform> redSpawns = new List<Transform>();
    public List<Transform> blueSpawns = new List<Transform>();

    public LayerMask wallMask;

    public TMP_Text finishText;

    public TMP_Text redText;
    public TMP_Text blueText;
    public TMP_Text timeText;

    int redSoldiersNumber = 7;
    int blueSoldiersNumber = 7;
    int gameTime = 200;

    bool isPlaying = false;

    System.Random rnd;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        rnd = new System.Random();
        var coinsInGame = allCoins.OrderBy(x => rnd.Next()).Take(allCoins.Count / 2);   //IA2-LINQ
        allCoins = coinsInGame.ToList();
        foreach (var coin in allCoins)
        {
            coin.SetActive(true);
        }

        isPlaying = true;
        StartCoroutine(RefresGameTime());
        //boundHeight = boundHeight - 9.5f;
    }

    private void Start()
    {
        var redNpcs = allNpc.Where(x => x.team == Team.Rojo).ToList();
        var blueNpcs = allNpc.Where(x => x.team == Team.Azul).ToList();
        var redRandomSpawns = redSpawns.OrderBy(x => rnd.Next()).ToList();
        var blueRandomSpawns = blueSpawns.OrderBy(x => rnd.Next()).ToList();

        var newNpcs = redNpcs.Concat(blueNpcs);
        var newSpawns = redRandomSpawns.Concat(blueRandomSpawns);

        var npcsSpawns = newNpcs.Zip(newSpawns, (npc, spawn) => npc.gameObject.name + " spawneo en el Spawn: " + spawn.gameObject.name);

        for (int i = 0; i < redNpcs.Count; i++)
        {
            redNpcs[i].transform.position = redRandomSpawns[i].position;
            blueNpcs[i].transform.position = blueRandomSpawns[i].position;
        }

        foreach (var npc in npcsSpawns)
        {
            Debug.Log(npc);
        }
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
        RefreshTeamText(entity.team);

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

    public void RefreshTeamText(Team team)
    {
        if(team == Team.Rojo)
        {
            redSoldiersNumber--;
            redText.text = redSoldiersNumber + " / 7";
            if (redSoldiersNumber <= 0) FinishGame(Team.Azul);
        }
        else if(team == Team.Azul)
        {
            blueSoldiersNumber--;
            blueText.text = blueSoldiersNumber + " / 7";
            if (blueSoldiersNumber <= 0) FinishGame(Team.Rojo);
        }
    }

    public void FinishGame(Team team)
    {
        //var resultado = allNpc.Zip(allNodes, (npc, node) => npc.name + node.name);

        finishText.enabled = true;
        finishText.text = ("Gano el equipo " + team);
        isPlaying = false;
        Time.timeScale = 0;
    }

    IEnumerator RefresGameTime()
    {
        yield return new WaitForSeconds(1);
        gameTime--;
        timeText.text = gameTime.ToString();

        if (gameTime <= 0)
        {
            if (redSoldiersNumber > blueSoldiersNumber) FinishGame(Team.Rojo);
            else if (blueSoldiersNumber > redSoldiersNumber) FinishGame(Team.Azul);
        }
        else if(isPlaying) StartCoroutine(RefresGameTime());
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
