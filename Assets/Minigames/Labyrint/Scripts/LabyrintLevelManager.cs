using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public List<GameObject> levels;

    public GameObject startButtonPrefab;
    public GameObject winButtonPrefab;
    public GameObject loseButtonPrefab;
    public bool isDebug = false;

    private GameObject currentLevel;
    private PlayerController player;
    private GameObject currentButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LaunchLevel();
    }

    void LaunchLevel()
    {
        int index = UnityEngine.Random.Range(0, levels.Count);
        currentLevel = Instantiate(levels[index]);

        player = currentLevel.GetComponentInChildren<PlayerController>();
        player.gameStarted = false;
        player.gameEnded = false;

        ShowButton(startButtonPrefab, StartGame, player.transform.position);
    }

    void StartGame()
    {
        Cursor.visible = false;

        Destroy(currentButton);
        player.gameStarted = true;
    }

    public void Victory()
    {
        Cursor.visible = true;

        player.gameEnded = true;
        if (isDebug)
        {
            ShowButton(winButtonPrefab, RestartLevel, player.transform.position);
        } else
        {
            GameObject.Find("CoreGame").SendMessage("Unpause");
            GameObject.Find("CoreGame").SendMessage("TakeRewardForMinigame");
            DestroyLabyrintGame();
        }
    }

    public void GameOver()
    {
        Cursor.visible = true;

        player.gameEnded = true;
        if (isDebug)
        {
            ShowButton(loseButtonPrefab, RestartLevel, player.transform.position);
        }
        else
        {
            GameObject.Find("CoreGame").SendMessage("Unpause");
            GameObject.Find("CoreGame").SendMessage("AddStress", -15);
            DestroyLabyrintGame();
        }
    }

    void RestartLevel()
    {
        Destroy(currentButton);
        Destroy(currentLevel);
        LaunchLevel();
    }

    void DestroyLabyrintGame()
    {
        Destroy(currentButton);
        Destroy(currentLevel);
        Destroy(gameObject);
    }

    void ShowButton(GameObject prefab, UnityEngine.Events.UnityAction onClick, Vector3 worldPosition)
    {
        if (currentButton != null)
        {
            Destroy(currentButton);
        }
        Canvas canvas = FindObjectOfType<Canvas>();
        currentButton = Instantiate(prefab, canvas.transform);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        currentButton.GetComponent<RectTransform>().position = screenPos;

        Button btn = currentButton.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(onClick);
    }
}
