using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;

    public bool gameStarted = false;
    public bool gameEnded = false;

    private bool isInsideYellowZone = false;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!gameStarted || gameEnded)
        {
            return;
        }

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos;

        if (!isInsideYellowZone)
        {
            LevelManager.Instance.GameOver();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BlackZone") && !gameEnded)
        {
            LevelManager.Instance.GameOver();
            gameEnded = true;
        }
        else if (other.CompareTag("YellowZone"))
        {
            isInsideYellowZone = true;
        }
        else if (other.CompareTag("RedZone"))
        {
            LevelManager.Instance.Victory();
            gameEnded = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("YellowZone"))
        {
            isInsideYellowZone = false;
        }
    }
}
