using UnityEngine;

public class QTESpawner : MonoBehaviour
{
    public GameObject qtePrefab;

    void Start()
    {
        var UI = GameObject.Find("UI");
        Instantiate(qtePrefab, UI.transform);
        Destroy(gameObject);
    }
}
