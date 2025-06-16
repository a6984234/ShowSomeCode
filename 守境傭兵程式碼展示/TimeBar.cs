using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    [SerializeField] Image fillImage;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] float originalWidth;
    void Start()
    {
        if (fillImage == null || timeText == null)
        {
            Debug.LogError("Fill Image or Time Text is not assigned in the TimeBar component.");
            return;
        }
        originalWidth= fillImage.rectTransform.sizeDelta.x;
    }
    public void UpdateTime(float currentTime, float maxTime)
    {
        if (fillImage == null || timeText == null)
        {
            Debug.LogError("Fill Image or Time Text is not assigned in the TimeBar component.");
            return;
        }

        var size = fillImage.rectTransform.sizeDelta;
        size.x = currentTime / maxTime * originalWidth;
        fillImage.rectTransform.sizeDelta = size;

        timeText.text = $"{currentTime:F2}";
    }
}
