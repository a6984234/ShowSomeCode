using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] float xMinOffset = -40, xMaxOffset = 40;
    public void Setup(DamageTextParams damageTextParams)
    {
        damageText.text = damageTextParams.Damage.ToString("F0");
        damageText.color = damageTextParams.Color;
        PlayAnimation(damageTextParams.OffsetX, damageTextParams.OffsetY);
    }
    private void PlayAnimation(float offsetX, float offsetY)
    {

        RectTransform rect = damageText.GetComponent<RectTransform>();
        rect.localScale = Vector3.zero;
        Vector2 basePos = rect.anchoredPosition;
        float duration = 1f;
        float xOffset = Random.Range(xMinOffset, xMaxOffset);// * (Random.value < 0.5f ? -1f : 1f);
        float yOffset = Random.Range(-40f, 40f);// * (Random.value < 0.5f ? -1f : 1f);
        Vector2 targetPos = new Vector2(basePos.x + offsetX, basePos.y + offsetY);

        Sequence seq = DOTween.Sequence();
        seq.Join(DOTween.To(() => 0f, t =>
        {
            Vector2 linearPos = Vector2.Lerp(basePos, targetPos, t);
            float x = Mathf.Sin(t * Mathf.PI) * xOffset;
            float y = Mathf.Sin(t * Mathf.PI) * yOffset; // 0.5 倍頻率讓 Y 弧度更寬緩
            rect.anchoredPosition = linearPos + new Vector2(x, y);
        }, 1f, duration));
        seq.Join(damageText.DOFade(0f, 1f));
        seq.Join(rect.DOScale(1.5f, 0.2f).SetEase(Ease.OutBack));
        seq.Join(rect.DOScale(1f,0.2f).SetEase(Ease.OutBack).SetDelay(0.2f));


        seq.AppendCallback(() =>
        {
            Destroy(gameObject); 
        });
    }
    public struct DamageTextParams
    {
        public float Damage;
        public Color Color;
        public float OffsetX;
        public float OffsetY;
        public float? CustomXSwing; // 可選項目（進階）
    }
}
