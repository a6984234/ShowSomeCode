using DG.Tweening;
using TMPro;
using UnityEngine;

public class HealText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healText;
    public void Setup(float healAmount)
    {
        healText.text = healAmount.ToString("0");
        PlayAnimation();
    }
    private void PlayAnimation()
    {
        var seq = DOTween.Sequence();
        var rect = healText.GetComponent<RectTransform>();
        seq.Join(rect.DOAnchorPosY(rect.anchoredPosition.y + 50f, 0.5f).SetEase(Ease.OutCubic));
        seq.Join(healText.DOFade(0f, 1f));
        seq.Join(healText.rectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutBack));
        seq.Join(healText.rectTransform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetDelay(0.2f));
        seq.AppendCallback(() =>
        {
            Destroy(gameObject);
        });
    }
}
