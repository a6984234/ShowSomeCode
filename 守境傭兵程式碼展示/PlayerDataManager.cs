using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    [SerializeField] private int playerGold = 350;
    private List<CharacterData> ownedMercenaries = new();
    private List<Equipment> ownedEquipments = new();
    private readonly Dictionary<Equipment, CharacterData> equipmentOwners = new();
    public int Gold => playerGold;
    public IReadOnlyList<CharacterData> OwnedMercenaries => ownedMercenaries.AsReadOnly();
    public IReadOnlyList<Equipment> OwnedEquipments => ownedEquipments.AsReadOnly();
    public event System.Action<int> OnGoldChanged;
    public void AddGold(int amount)
    {
        playerGold += amount;
        OnGoldChanged?.Invoke(playerGold);
    }

    public bool SpendGold(int amount)
    {
        if (playerGold >= amount)
        {
            playerGold -= amount;
            OnGoldChanged?.Invoke(playerGold);
            return true;
        }
        Debug.LogWarning("Not enough gold.");
        return false;
    }

    public void AddMercenary(CharacterData merc)
    {
        ownedMercenaries.Add(merc);
    }

    public bool OwnsMercenary(CharacterData merc)
    {
        return ownedMercenaries.Contains(merc); // 可自訂比對條件
    }

    public void AddEquipment(Equipment equipment)
    {
        if (!ownedEquipments.Contains(equipment))
            ownedEquipments.Add(equipment);
    }

    public void RemoveEquipment(Equipment equipment)
    {
        if (ownedEquipments.Remove(equipment))
        {
            equipmentOwners.Remove(equipment);
        }
    }

    public void RegisterEquipmentUse(Equipment equipment, CharacterData owner)
    {
        equipmentOwners[equipment] = owner;
    }

    public void UnregisterEquipmentUse(Equipment equipment)
    {
        equipmentOwners.Remove(equipment);
    }

    public bool IsEquipmentEquipped(Equipment equipment)
    {
        return equipmentOwners.ContainsKey(equipment);
    }

    public bool IsEquipmentEquippedByOther(Equipment equipment, CharacterData character)
    {
        return equipmentOwners.TryGetValue(equipment, out var owner) && owner != character;
    }
}
