using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Sprite emptySprite;

    private EquipmentSlot slot;
    private Equipment equipment;
    private CharacterDetail owner;

    public void Setup(CharacterDetail owner, EquipmentSlot slot, Equipment equipment)
    {
        this.owner = owner;
        this.slot = slot;
        this.equipment = equipment;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (icon == null) return;
        if (equipment != null)
            icon.sprite = equipment.Icon;
        else if (emptySprite != null)
            icon.sprite = emptySprite;
    }

    public void OnClick()
    {
        if (owner == null) return;
        if (equipment != null)
            owner.Unequip(slot);
        else
            owner.EquipFromInventory(slot);
    }
}

