using System.Collections.Generic;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class QTEMiniGame : MonoBehaviour
{
    [SerializeField]
    private float buttonTime = 5f;

    [SerializeField]
    private float buttonSpawnCooldown = 2f;

    [SerializeField]
    private float gameTime = 10f;

    [SerializeField]
    private int lives;

    [SerializeField]
    private QTEButton buttonPrefab;

    [SerializeField]
    private GameObject[] hearts;

    [SerializeField]
    private RectTransform buttonContainer;
    
    [SerializeField]
    private Clock timer;

    private float _currentSpawnCooldown = 0f;

    private List<QTEButton> _buttons = new();

    public readonly UnityEvent OnLoose = new();
    public readonly UnityEvent OnWin = new();

    private float _currentTime;

    enum State
    {
        Start,
        WaitForSpawn,
        Idle,
        Spawn,
        Loose,
        Win,
        SpawnEnd
    }

    private State _state = State.Start;

    void Update()
    {
        switch (_state)
        {
            case State.Start:
                _state = State.Spawn;
                _currentSpawnCooldown = buttonSpawnCooldown;
                _currentTime = gameTime;
                UpdateHealthBar();
                break;
            case State.WaitForSpawn:
            case State.Idle:
                break;
            case State.Spawn:
                HandleSpawnState();
                break;
            case State.Loose:
                HandleLooseGame();
                break;
            case State.Win:
                HandleWinGame();
                break;
            case State.SpawnEnd:
                HandleSpawnEndState();
                break;
        }
    }

    private void HandleSpawnEndState()
    {
        UpdateTimer();
        if (_buttons.Count > 0)
        {
            return;
        }

        SetWin();
    }

    void UpdateHealthBar()
    {
        for (var i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < lives);
        }
    }

    void UpdateTimer()
    {
        timer.SetTime(_currentTime);
    }

    private void HandleWinGame()
    {
        OnWin.Invoke();
        ExitGame();
    }

    private void HandleLooseGame()
    {
        OnLoose.Invoke();
        ExitGame();
    }

    private void ExitGame()
    {
        GameObject.Find("CoreGame").SendMessage("Unpause");
        Destroy(gameObject);
    }

    private void HandleSpawnState()
    {
        _currentTime -= Time.deltaTime;

        if (_currentTime <= 0)
        {
            _state = State.SpawnEnd;
            _currentTime = 0;
            return;
        }
        
        UpdateTimer();

        if (_currentSpawnCooldown > 0)
        {
            _currentSpawnCooldown -= Time.deltaTime;
            return;
        }

        SpawnButton();
        _currentSpawnCooldown = buttonSpawnCooldown;
    }

    private Vector2 GenerateButtonPosition()
    {
        var buttonSize = buttonPrefab.Size();

        float x = Random.Range(-buttonContainer.rect.width / 2 + buttonSize.x, buttonContainer.rect.width / 2 - buttonSize.x);
        float y = Random.Range(-buttonContainer.rect.height / 2 + buttonSize.y, buttonContainer.rect.height / 2 - buttonSize.y);

        // Ensure button is not intersect with existing buttons
        foreach (var button in _buttons)
        {
            // Take into account the button size to avoid overlap and ensure it fits within the container
            if (Mathf.Abs(x - button.transform.localPosition.x) < buttonSize.x &&
                Mathf.Abs(y - button.transform.localPosition.y) < buttonSize.y)
            {
                return GenerateButtonPosition(); // Retry if position intersects
            }
        }

        return new Vector2(x, y);
    }

    private void SpawnButton()
    {
        var buttonObject = Instantiate(buttonPrefab, GenerateButtonPosition(), Quaternion.identity);
        buttonObject.transform.SetParent(buttonContainer, false);

        var button = buttonObject.GetComponent<QTEButton>();

        _buttons.Add(button);

        button.timer = buttonTime;

        button.OnTimerEnd.AddListener(() =>
        {
            RemoveButton(button);
            OnButtonMiss();
        });

        button.OnClick.AddListener(() =>
        {
            RemoveButton(button);
        });
    }

    private void RemoveButton(QTEButton button)
    {
        _buttons.Remove(button);
    }

    private void OnButtonMiss()
    {
        lives--;
        if (lives <= 0)
        {
            SetLoose();
        }

        UpdateHealthBar();
    }

    private void SetLoose()
    {
        _state = State.Loose;
    }

    private void SetWin()
    {
        _state = State.Win;
    }
}
