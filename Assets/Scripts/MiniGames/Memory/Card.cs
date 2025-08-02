using System.ComponentModel;
using UnityEngine;
using DG.Tweening;
using System;
using JetBrains.Annotations;

public class Card : MonoBehaviour
{
    [SerializeField]
    private float rotateDuration;
    [SerializeField]
    private float scaleDuration;
    [SerializeField]
    private float upscaleValue;

    [SerializeField]
    private Sprite backSprite;
    [SerializeField]
    private Sprite frontSprite;
    private Action<Card> _clickCallback;
    private bool _isFlipping = false;
    private SpriteRenderer _spriteRenderer;
    private DG.Tweening.Sequence _sequence;
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _spriteRenderer.sprite = backSprite;
    }

    void Update()
    {
        
    }

    void OnMouseDown()
    {
        _clickCallback?.Invoke(this);
    }
    
    public Sprite GetFrontSprite() => frontSprite;

    public void SetClickCallback(Action<Card> clickCallback)
    {
        _clickCallback = clickCallback;
    }

    public Tween Flip()
    {
        _isFlipping = true;
        
        _sequence = DOTween.Sequence();

        _sequence.Append(transform.DOScale(transform.localScale + transform.localScale * upscaleValue, scaleDuration));
        _sequence.Join(transform.DORotate(new Vector3(0, 90, 0), rotateDuration, RotateMode.LocalAxisAdd));

        _sequence.AppendCallback(() => _spriteRenderer.sprite = _spriteRenderer.sprite == backSprite ? frontSprite : backSprite);
        _sequence.Append(transform.DOScale(transform.localScale, scaleDuration));
        _sequence.Join(transform.DORotate(new Vector3(0, 90, 0), rotateDuration, RotateMode.LocalAxisAdd));
        _sequence.AppendCallback(() => _isFlipping = false);

        return _sequence;
    }

    [CanBeNull]
    public Tween GetCurrentTween()
    {
        return _sequence;
    }

    public bool IsFlipping()
    {
        return _isFlipping;
    }
}
