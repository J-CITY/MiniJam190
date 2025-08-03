using System.IO.Pipes;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using DG.Tweening;

public class HowToPlay : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject cat;
    [SerializeField] private GameObject npc;
    void Start()
    {
        var arrowInitialScale = arrow.transform.localScale;
        var catInitialScale = cat.transform.localScale;

        arrow.transform.localScale = Vector3.zero;
        cat.transform.localScale = Vector3.zero;
        
        var seq = DOTween.Sequence();
        
        seq.Append(cat.transform.DOScale(catInitialScale, 0.5f).SetEase(Ease.OutBack));
        seq.Append(arrow.transform.DOScale(arrowInitialScale, 0.5f).SetEase(Ease.OutBack));
        seq.Append(arrow.GetComponent<Image>().DOFade(0f, 0.25f));
        seq.Join(cat.transform.DOMove(npc.transform.position, 1f).SetEase(Ease.InBack));
        seq.Append(cat.GetComponent<Image>().DOFade(0f, 0.2f));
        seq.AppendInterval(2f);

        seq.SetLoops(-1);
    }
}
