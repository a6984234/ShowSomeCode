using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

public enum EquipmentSlot
{
    Weapon,
    Body,
    Leg
}
public enum WeaponType
{
    None,
    Sword, 
    Bow,    
    Staff   
}
[System.Serializable]
public class Equipment
{
    private EquipmentSlot slot;
    private WeaponType weaponType;
    private int atk;
    private float atkSpd;
    private float crit;
    private float critDmg;
    private int def;
    private int maxHp;
    private float eva;
    public EquipmentSlot Slot => slot;
    public WeaponType WeaponType => weaponType;
    public int Atk => atk;
    public float AtkSpd => atkSpd;
    public float Crit => crit;
    public float CritDmg => critDmg;
    public int Def => def;
    public int MaxHp => maxHp;
    public float Eva => eva;
    public Sprite Icon
    {
        get
        {
            Sprite sprite = null;
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    sprite = weaponType switch
                    {
                        WeaponType.Sword => Addressables.LoadAssetAsync<Sprite>("Shop/Shop_sword.png").WaitForCompletion(),
                        WeaponType.Bow => Addressables.LoadAssetAsync<Sprite>("Shop/Shop_bow.png").WaitForCompletion(),
                        WeaponType.Staff => Addressables.LoadAssetAsync<Sprite>("Shop/Shop_staff.png").WaitForCompletion(),
                        _ => null
                    };
                    break;
                case EquipmentSlot.Body:
                    sprite = Addressables.LoadAssetAsync<Sprite>("Shop/Shop_clothes.png").WaitForCompletion();
                    break;
                case EquipmentSlot.Leg:
                    sprite = Addressables.LoadAssetAsync<Sprite>("Shop/Shop_shoes.png").WaitForCompletion();
                    break;
            }
            return sprite;
        }
    }
    public static Equipment Create(int level)
    {
        Equipment equipment = new Equipment();
        equipment.slot = Random.Range(0, 3) switch
        {
            0 => EquipmentSlot.Weapon,
            1 => EquipmentSlot.Body,
            _ => EquipmentSlot.Leg
        };
        switch (equipment.slot)
        {
            case EquipmentSlot.Weapon:
                equipment.slot = EquipmentSlot.Weapon;
                equipment.weaponType = Random.Range(0, 3) switch
                {
                    0 => WeaponType.Sword,
                    1 => WeaponType.Bow,
                    _ => WeaponType.Staff
                };
                equipment.atk = Random.Range(1, 5) * level;
                equipment.atkSpd = Random.Range(0, 0.03f) * level;
                equipment.crit = Random.Range(0, 0.03f) * level;
                equipment.critDmg = Random.Range(0, 0.03f) * level;
                break;
            case EquipmentSlot.Body:
                equipment.slot = EquipmentSlot.Body;
                equipment.def = Random.Range(1, 5) * level;
                equipment.maxHp = Random.Range(1, 10) * level;
                break;
            case EquipmentSlot.Leg:
                equipment.slot = EquipmentSlot.Leg;
                equipment.eva = Random.Range(0, 0.03f) * level;
                equipment.def = Random.Range(1, 3) * level;
                equipment.maxHp = Random.Range(1, 3) * level;
                break;
        }
        Debug.Log("Created Equipment: " + equipment.slot + " | WeaponType: " + equipment.weaponType + " | Atk: " + equipment.atk + " | AtkSpd: " + equipment.atkSpd + " | Crit: " + equipment.crit + " | CritDmg: " + equipment.critDmg + " | Def: " + equipment.def + " | MaxHp: " + equipment.maxHp + " | Eva: " + equipment.eva);
        return equipment;
    }
}
