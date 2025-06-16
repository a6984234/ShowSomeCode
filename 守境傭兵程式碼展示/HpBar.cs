using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Image hpBarImage; 
    public void SetHpBar(float hp)
    {
        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = hp;
        }
        else
        {
            Debug.LogWarning("HpBarImage is not assigned in the inspector.");
        }
    }
}
