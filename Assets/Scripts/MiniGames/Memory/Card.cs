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

    public Sprite backSprite;
    public Sprite frontSprite;
    private Action<Card> _clickCallback;
    private bool _isFlipping = false;
    private SpriteRenderer _spriteRenderer;
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
        
        var seq = DOTween.Sequence();

        seq.Append(transform.DOScale(transform.localScale + transform.localScale * upscaleValue, scaleDuration));
        seq.Join(transform.DORotate(new Vector3(0, 90, 0), rotateDuration, RotateMode.LocalAxisAdd));

        seq.AppendCallback(() => _spriteRenderer.sprite = _spriteRenderer.sprite == backSprite ? frontSprite : backSprite);
        seq.Append(transform.DOScale(transform.localScale, scaleDuration));
        seq.Join(transform.DORotate(new Vector3(0, 90, 0), rotateDuration, RotateMode.LocalAxisAdd));
        seq.AppendCallback(() => _isFlipping = false);

        return seq;
    }

    public bool IsFlipping()
    {
        return _isFlipping;
    }
}
