using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class QTEButton : MonoBehaviour
{
    private static readonly int FadeAmount = Shader.PropertyToID("_FadeAmount");
    public float timer = 1f;

    [SerializeField]
    private float appearDuration = 0.5f;

    [SerializeField]
    private float clickDisappearDuration = 0.5f;

    [SerializeField] 
    private RectTransform circleTransform;

    [SerializeField]
    private Image uiImage;

    public readonly UnityEvent OnTimerEnd = new();
    public readonly UnityEvent OnClick = new();

    private float currentTime;

    private RectTransform _rectTransform;

    private Material _instanceMaterial;

    public Vector2 Size() => new(100, 100);

    enum AnimState
    {
        Appear,
        Wait,
        Clicked,
        Missed,
        Die
    }

    private AnimState _state = AnimState.Appear;
    private float _progress;
    private float _end;
    
    void Start()
    {
        _state = AnimState.Appear;
        _progress = 0f;
        _end = appearDuration;

        _rectTransform = GetComponent<RectTransform>();

        currentTime = timer;

        // Set initial scale to zero for the appear animation
        transform.localScale = Vector3.zero;

        _instanceMaterial = new Material(uiImage.material);
        uiImage.material = _instanceMaterial;
    }

    void Update()
    {
        currentTime -= Time.deltaTime;

        var progress = 1f - currentTime / timer;

        var scale = EasingFunction.EaseOutCubic(1, 2, progress);

        circleTransform.localScale = new Vector3(scale, scale);

        _instanceMaterial.SetFloat(FadeAmount, EasingFunction.EaseOutSine(0f, 0.4f, progress));

        if (currentTime <= 0 && _state == AnimState.Wait)
        {
            OnTimerEnd.Invoke();
            _state = AnimState.Missed;
        }

        _progress += Time.deltaTime;
        
        if (_progress >= _end)
        {
            _progress = _end;
        }

        switch (_state)
        {
            case AnimState.Appear:
            {
                var t = EasingFunction.EaseOutElastic(0f, 1f, 1f - (_end - _progress) / _end);
                transform.localScale = new Vector3(t, t);
                if (_progress >= _end)
                {
                    _state = AnimState.Wait;
                    _progress = 0f;
                }
            } break;
            case AnimState.Wait:
            {
                
            } break;
            case AnimState.Clicked:
            {
                var t = EasingFunction.EaseOutQuart(0f, 1f, 1f - (_end - _progress) / _end);
                transform.localScale = new Vector3(1 - t, 1 - t);
                if (_progress >= _end)
                {
                    _state = AnimState.Die;
                    _progress = 0f;
                }
            } break;
            case AnimState.Missed:
                _state = AnimState.Die;
                break;
            case AnimState.Die:
                Destroy(gameObject);
                break;
        }
    }

    public void OnClickButton()
    {
        _state = AnimState.Clicked;
        _progress = 0f;
        _end = clickDisappearDuration;
        OnClick.Invoke();
    }
}
