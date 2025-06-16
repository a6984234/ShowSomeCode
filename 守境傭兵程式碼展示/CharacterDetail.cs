using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDetail : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI jobText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Transform statsParent;
    [SerializeField] private StatDisplay statDisplayPrefab;
    [SerializeField] private EquipmentSlotUI weaponSlot;
    [SerializeField] private EquipmentSlotUI bodySlot;
    [SerializeField] private EquipmentSlotUI legSlot;
    [SerializeField] private Button closeButton;

    private CharacterData characterData;
    private List<StatDisplay> statDisplays = new List<StatDisplay>();
    bool initStatPrefab = false;
    private void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void Setup(CharacterData data)
    {
        characterData = data;
        gameObject.SetActive(true);
        Refresh();
    }

    private void Refresh()
    {
        if (characterData == null) return;
        if (jobText != null)
            jobText.text = characterData.Job.ToString();
        if (levelText != null)
            levelText.text = $"Lv.{characterData.Level}";
        RefreshStats();
        RefreshEquipment();
    }

    private void RefreshStats()
    {
        var statList = new List<(StatDisplay.StatType statType, float value)>
        {
            (StatDisplay.StatType.Attack, characterData.Atk),
            (StatDisplay.StatType.Defense, characterData.Def),
            (StatDisplay.StatType.Health, characterData.MaxHp),
            (StatDisplay.StatType.AtkSpeed, characterData.AtkSpd),
            (StatDisplay.StatType.CriticalChance, characterData.Crit),
            (StatDisplay.StatType.CriticalDamage, characterData.CritDmg),
            (StatDisplay.StatType.Evasion, characterData.Eva),
            (StatDisplay.StatType.Level, characterData.Level),
            (StatDisplay.StatType.Experience, characterData.Exp / characterData.ExpToNextLevel),
        };
        if (!initStatPrefab)
        {
            initStatPrefab = true;
            foreach (var pair in statList)
            {
                var display = Instantiate(statDisplayPrefab, statsParent);
                statDisplays.Add(display);
                display.Setup(pair.statType, pair.value);
            }
        }
        else
            for (int i = 0; i < statDisplays.Count; i++)
            {
                if (i < statList.Count)
                {
                    statDisplays[i].Setup(statList[i].statType, statList[i].value);
                }
            }
    }

    private void RefreshEquipment()
    {
        if (weaponSlot != null)
            weaponSlot.Setup(this, EquipmentSlot.Weapon, characterData.GetEquipment(EquipmentSlot.Weapon));
        if (bodySlot != null)
            bodySlot.Setup(this, EquipmentSlot.Body, characterData.GetEquipment(EquipmentSlot.Body));
        if (legSlot != null)
            legSlot.Setup(this, EquipmentSlot.Leg, characterData.GetEquipment(EquipmentSlot.Leg));
    }

    public void EquipFromInventory(EquipmentSlot slot)
    {
        foreach (var eq in PlayerDataManager.Instance.OwnedEquipments)
        {
            if (eq.Slot != slot) continue;
            if (PlayerDataManager.Instance.IsEquipmentEquippedByOther(eq, characterData)) continue;
            characterData.Equip(eq);
            Refresh();
            return;
        }
        Debug.LogWarning($"No available equipment for slot {slot}");
    }

    public void Unequip(EquipmentSlot slot)
    {
        characterData.Unequip(slot);
        Refresh();
    }
}

