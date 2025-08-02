using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public Canvas mainCanvas;
    public List<GameObject> levels;

    public GameObject startButtonPrefab;
    public GameObject winButtonPrefab;
    public GameObject loseButtonPrefab;

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
        ShowButton(winButtonPrefab, RestartLevel, player.transform.position);
    }

    public void GameOver()
    {
        Cursor.visible = true;

        player.gameEnded = true;
        ShowButton(loseButtonPrefab, RestartLevel, player.transform.position);
    }

    void RestartLevel()
    {
        Destroy(currentButton);
        Destroy(currentLevel);
        LaunchLevel();
    }

    void ShowButton(GameObject prefab, UnityEngine.Events.UnityAction onClick, Vector3 worldPosition)
    {
        if (currentButton != null)
        {
            Destroy(currentButton);
        }

        currentButton = Instantiate(prefab, mainCanvas.transform);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        currentButton.GetComponent<RectTransform>().position = screenPos;

        Button btn = currentButton.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(onClick);
    }
}
