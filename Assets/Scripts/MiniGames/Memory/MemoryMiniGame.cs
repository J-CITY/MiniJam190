using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class MemoryMiniGame : MonoBehaviour
{
    [SerializeField]
    private int rows;
    [SerializeField]
    private int columns;
    [SerializeField]
    private float offsetX;
    [SerializeField]
    private float offsetY;

    [SerializeField] 
    private float unflipDelay;
    [SerializeField]
    private List<GameObject> cardPrefabs;

    [SerializeField] 
    private int turnCount;

    private List<Card> _flippedCards = new List<Card>();
    private List<Card> _matchedCards = new List<Card>();
    private List<Card> _discardedCards = new List<Card>();
    void Start()
    {
        Debug.Assert(cardPrefabs.Count >= (columns * rows) / 2, $"Card prefabs count < (columns * rows) / 2! ");
        
        List<GameObject> cards = new List<GameObject>();

        for (int i = 0; i < (rows * columns) / 2; i++)
        {
            cards.Add(cardPrefabs[i]);
            cards.Add(cardPrefabs[i]);
        }
        
        Shuffle(cards);
        
        var spriteSize = cardPrefabs[0].GetComponent<Card>().backSprite.bounds.size;
        
        for (var row = 0; row < rows; ++row)
        {
            for (var column = 0; column < columns; ++column)
            {
                var gameObject = Instantiate(cards[row * columns + column], new Vector3((offsetX + spriteSize.x) * row, (offsetY + spriteSize.y) * column, 0), Quaternion.identity, this.transform);

                var card = gameObject.GetComponent<Card>();
                
                card.SetClickCallback(new Action<Card>((Card self) =>
                {
                    if (_discardedCards.Contains(self) || _flippedCards.Contains(self) || _matchedCards.Contains(self) || _flippedCards.Count >= 2 || self.IsFlipping())
                    {
                        return;
                    }
                    _flippedCards.Add(self);

                    self.Flip();
                }));
            }
        }

        var size = new Vector2(columns * (spriteSize.x + offsetX), rows * (spriteSize.y + offsetY));
        
        Debug.Log(spriteSize);
        Debug.Log(size);
        
        transform.position = new Vector3((size.x - offsetX - spriteSize.x) * -0.5f, (size.y - offsetY - spriteSize.y) * -0.5f, 0);
    }

    void Update()
    {
        if (turnCount == 0 || _matchedCards.Count == columns * rows)
        {
            GameObject.Find("CoreGame").SendMessage("Unpause");
            Destroy(gameObject);
        }
        
        if (_flippedCards.Count >= 2)
        {
            var currentPair = _flippedCards.GetRange(0, 2);

            if (currentPair[0].IsFlipping() || currentPair[1].IsFlipping())
            {
                return;
            }
            
            --turnCount;
            
            if (currentPair[0].GetFrontSprite() == currentPair[1].GetFrontSprite())
            {
                _matchedCards.AddRange(currentPair);
            }
            else
            {
                var Seq = DOTween.Sequence();
                
                _discardedCards.AddRange(currentPair);

                Seq.AppendInterval(unflipDelay);
                Seq.Append(currentPair[0].Flip());
                Seq.Join(currentPair[1].Flip());
                Seq.AppendCallback(() =>
                {
                    _discardedCards.Remove(currentPair[0]);
                    _discardedCards.Remove(currentPair[1]);
                });
            }

            _flippedCards.RemoveRange(0, 2);
        }
    }
    
    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }
}
