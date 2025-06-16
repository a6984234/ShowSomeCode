using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image equipmentImage;
    [SerializeField] Transform statsLayout;
    [SerializeField] StatDisplay statDisplayPrefab;
    [SerializeField] TextMeshProUGUI priceText;
    private Equipment equipment;
    private int price;
    public void Setup(int level)
    {
        equipment = Equipment.Create(level);
        equipmentImage.sprite = equipment.Icon;

        price = CalculatePrice(level);
        if (priceText != null)
            priceText.text = price.ToString();

        var statMap = new Dictionary<StatDisplay.StatType, float>
        {
            { StatDisplay.StatType.Attack, equipment.Atk },
            { StatDisplay.StatType.Defense, equipment.Def },
            { StatDisplay.StatType.Health, equipment.MaxHp },
            { StatDisplay.StatType.AtkSpeed, equipment.AtkSpd },
            { StatDisplay.StatType.CriticalChance, equipment.Crit },
            { StatDisplay.StatType.CriticalDamage, equipment.CritDmg },
            { StatDisplay.StatType.Evasion, equipment.Eva }
        };

        foreach (var pair in statMap)
        {
            if (pair.Value > 0)
            {
                var statDisplay = Instantiate(statDisplayPrefab, statsLayout);
                statDisplay.Setup(pair.Key, pair.Value);
            }
        }
    }

    private int CalculatePrice(int level)
    {
        return 100 * level;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!PlayerDataManager.Instance.SpendGold(price))
        {
            Debug.Log("Not enough gold");
            return;
        }

        PlayerDataManager.Instance.AddEquipment(equipment);
        Destroy(gameObject);
    }
}
