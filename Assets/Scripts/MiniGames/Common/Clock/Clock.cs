using System;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI text;

    public void SetTime(float seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        text.text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
}
