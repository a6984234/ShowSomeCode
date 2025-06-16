using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour
{
    public enum StatType
    {
        Attack,
        Defense,
        Health,
        AtkSpeed,
        CriticalChance,
        CriticalDamage,
        Evasion,
        Level,          
        Experience      
    }
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI valueText;
    public void Setup(StatType statType, float value)
    {
        string address = StatDisplayImageAddress.GetAddress(statType);
        var sprite = Addressables.LoadAssetAsync<Sprite>(address);
        image.sprite = sprite.WaitForCompletion();
        if (image.sprite == null)
        {
            Debug.LogWarning($"Failed to load sprite for stat type: {statType}");
        }

        switch (statType)
        {
            case StatType.CriticalChance:
            case StatType.Evasion:
            case StatType.CriticalDamage:
                var percentage = Mathf.RoundToInt(value * 100);
                if (percentage <= 0)
                    Destroy(gameObject);
                valueText.text = $"{percentage}%";
                break;
            case StatType.AtkSpeed:
                valueText.text = $"{value:F2}s";
                break;
            default:
                valueText.text = value.ToString("0");
                break;
        }
    }
}
