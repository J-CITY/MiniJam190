using System.Collections.Generic;
using UnityEngine;

public class QTEMiniGame : MonoBehaviour
{
    [SerializeField]
    private float buttonTime = 5f;

    [SerializeField]
    private float buttonSpawnCooldown = 2f;

    [SerializeField]
    private int lives;

    [SerializeField]
    private QTEButton buttonPrefab;

    [SerializeField]
    private RectTransform buttonContainer;

    private float _currentSpawnCooldown = 0f;

    private List<QTEButton> _buttons = new List<QTEButton>();

    enum State
    {
        Idle,
        Spawn,
    }

    private State _state = State.Spawn;

    void Update()
    {
        switch (_state)
        {
            case State.Idle:
                break;
            case State.Spawn:
                HandleSpawnState();
                break;
        }
    }

    private void HandleSpawnState()
    {
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
            if (Mathf.Abs(x - button.transform.localPosition.x) < buttonSize.x / 2 &&
                Mathf.Abs(y - button.transform.localPosition.y) < buttonSize.y / 2)
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
        Destroy(button.gameObject);
    }

    private void OnButtonMiss()
    {
        lives--;
        if (lives <= 0)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        
    }
}
