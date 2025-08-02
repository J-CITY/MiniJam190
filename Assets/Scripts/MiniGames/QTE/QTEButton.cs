using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class QTEButton : MonoBehaviour
{
    public float timer = 1f;

    [SerializeField]
    private TextMeshProUGUI text;

    public readonly UnityEvent OnTimerEnd = new();
    public readonly UnityEvent OnClick = new();

    public Vector2 Size() => transform.GetComponent<RectTransform>().sizeDelta;

    void Update()
    {
        timer -= Time.deltaTime;

        text.text = timer.ToString("F2");

        if (timer <= 0)
        {
            OnTimerEnd.Invoke();
        }
    }

    public void OnClickButton()
    {
        OnClick.Invoke();
    }
}
