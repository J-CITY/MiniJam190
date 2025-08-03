using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


enum Loot : int
{
    Loot1 = 0,
    Loot2,
}

public class CoreGameController : MonoBehaviour
{
    enum State
    {
        InGame,
        LevelEnd,
        Pause,
        Win
    }

    public Transform playerSpawnPoint;
    public GameObject playerPrefab;
    public GameObject guyPrefab;

    [SerializeField] private int maxGuyOnField = 10;
    [SerializeField] private float levelTimer = 60.0f;
    [SerializeField] private List<GameObject> miniGames = new List<GameObject>();

    private GameObject player;
    private List<GameObject> guys = new List<GameObject>();

    State state = State.InGame;

    public int stressValue = 100;

    public int lootRestoreStress = 10;


    List<Loot> loots = new List<Loot>();

    Loot goal = Loot.Loot1;

    void Start()
    {
        loots.Clear();
        goal = (Loot)UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(Loot)).Length);
        state = State.InGame;
        SpawnPlayer();
        SpawnGuys();
    }

    void SpawnPlayer()
    {
        player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
    }

    void SpawnGuys()
    {
        guys.Clear();
        for (int i = 0; i < maxGuyOnField; ++i)
        {
            GameObject obj = Instantiate(guyPrefab, new Vector3(100, 100), Quaternion.identity);
            Component component = obj.GetComponent("GuyController");
            if (component != null)
            {
                var leftDownField = component.GetType().GetField("leftDown");
                if (leftDownField != null)
                {
                    leftDownField.SetValue(component, GameObject.Find("LeftDown").transform);
                }

                var rightUpField = component.GetType().GetField("rightUp");
                if (rightUpField != null)
                {
                    rightUpField.SetValue(component, GameObject.Find("RightUp").transform);
                }
            }
            guys.Add(obj);
        }
    }

    void Update()
    {
        if (state != State.InGame)
        {
            return;
        }
        Debug.Log(levelTimer);
        if (levelTimer > 0.0f)
        {
            levelTimer -= Time.deltaTime;
        }
        if (levelTimer < 0.0f || stressValue <= 0)
        {
            levelTimer = 0.0f;
            state = State.LevelEnd;

            stressValue = 0;
            Clear();

            LoseGame();
        }
    }

    void Clear()
    {
        Destroy(player);
        foreach (GameObject guy in guys)
        {
            Destroy(guy);
        }
        guys.Clear();
    }

    private void Pause()
    {
        if (state != State.InGame)
        {
            return;
        }
        state = State.Pause;
        Clear();
        //player.BroadcastMessage("Pause");
        //foreach (GameObject guy in guys)
        //{
        //    guy.BroadcastMessage("Pause");
        //}
    }
    private void Unpause()
    {
        if (state != State.Pause)
        {
            return;
        }
        state = State.InGame;
        SpawnPlayer();
        SpawnGuys();
    }

    private void CreateMinigame()
    {
        Instantiate(miniGames[UnityEngine.Random.Range(0, miniGames.Count)], Vector3.zero, Quaternion.identity);
    }
    void AddStress(int v)
    {
        if (state != State.InGame)
        {
            return;
        }
        stressValue += v;
        if (stressValue < 0)
        {
            stressValue = 0;
        }
        if (stressValue > 100)
        {
            stressValue = 100;
        }
    }

    void TakeRewardForMinigame(Loot l)
    {
        if (l == goal)
        {
            state = State.Win;
            WinGame();
        }
        else
        {
            stressValue += lootRestoreStress;
        }
    }

    void LoseGame()
    {

    }

    void WinGame()
    {

    }
}
