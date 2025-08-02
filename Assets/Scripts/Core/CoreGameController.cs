using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class CoreGameController : MonoBehaviour
{
    enum State
    {
        InGame,
        LevelEnd,
        Pause
    }

    public Transform playerSpawnPoint;
    public GameObject playerPrefab;
    public GameObject guyPrefab;

    [SerializeField] private int maxGuyOnField = 10;
    [SerializeField] private float levelTimer = 60.0f;

    private GameObject player;
    private List<GameObject> guys = new List<GameObject>();

    State state = State.InGame;

    void Start()
    {
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
            GameObject obj = Instantiate(guyPrefab, new Vector3(), Quaternion.identity);
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
        if (levelTimer > 0.0f)
        {
            levelTimer -= Time.deltaTime;
        }
        if (levelTimer < 0.0f)
        {
            levelTimer = 0.0f;
            state = State.LevelEnd;
            Clear();
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
        Clear();

        //player.BroadcastMessage("Pause");
        //foreach (GameObject guy in guys)
        //{
        //    guy.BroadcastMessage("Pause");
        //}
    }
    private void Unpause()
    {
        SpawnPlayer();
        SpawnGuys();
    }
}
