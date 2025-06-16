using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public enum FactionType
    {
        Ally,
        Enemy,
        Neutral
    }

    public enum JobType
    {
        Swordsman,
        Archer,
        Healer,
    }

    public enum CharacterType
    {
        Human,
        Goblin,
        Orc,
        Beast,
        Robot
    }

    [SerializeField] private FactionType faction;
    [SerializeField] private JobType job;
    [SerializeField] private CharacterType type;
    [SerializeField] private int atk;
    [SerializeField] private int def;
    [SerializeField] private int hp;
    [SerializeField] private int maxHp;
    [SerializeField] private float atkSpd;
    [SerializeField] private float crit;
    [SerializeField] private float critDmg;
    [SerializeField] private float eva;
    [SerializeField] private int level;
    [SerializeField] private float exp;

    private readonly Dictionary<EquipmentSlot, Equipment> equippedItems = new();

    // Public getters
    public FactionType Faction => faction;
    public JobType Job => job;
    public CharacterType Type => type;
    public int BaseAtk => atk;
    public int Atk => atk + SumEquipmentInt(e => e.Atk);
    public int BaseDef => def;
    public int Def => def + SumEquipmentInt(e => e.Def);
    public int Hp => hp;
    public int BaseMaxHp => maxHp;
    public int MaxHp => maxHp + SumEquipmentInt(e => e.MaxHp);
    public float BaseAtkSpd => atkSpd;
    public float AtkSpd => atkSpd + SumEquipmentFloat(e => e.AtkSpd);
    public float BaseCrit => crit;
    public float Crit => crit + SumEquipmentFloat(e => e.Crit);
    public float BaseCritDmg => critDmg;
    public float CritDmg => critDmg + SumEquipmentFloat(e => e.CritDmg);
    public float BaseEva => eva;
    public float Eva => eva + SumEquipmentFloat(e => e.Eva);
    public int Level => level;
    public float Exp => exp;
    public float ExpToNextLevel => 10 + level * 15 + Mathf.Floor(level / 5f) * 20;
    public int ExpRewardOnDeath => Mathf.CeilToInt(level * 5 + Mathf.Sqrt(level) * 2);
    public int DropMoney => 10 + Mathf.CeilToInt(level * level / 10f);

    private int SumEquipmentInt(System.Func<Equipment, int> selector)
    {
        int sum = 0;
        foreach (var item in equippedItems.Values)
            sum += selector(item);
        return sum;
    }

    private float SumEquipmentFloat(System.Func<Equipment, float> selector)
    {
        float sum = 0f;
        foreach (var item in equippedItems.Values)
            sum += selector(item);
        return sum;
    }

    public void Equip(Equipment equipment)
    {
        if (equipment == null) return;
        if (PlayerDataManager.Instance != null &&
            PlayerDataManager.Instance.IsEquipmentEquippedByOther(equipment, this))
        {
            Debug.LogWarning("Equipment is already equipped by another character.");
            return;
        }
        equippedItems[equipment.Slot] = equipment;
        PlayerDataManager.Instance?.RegisterEquipmentUse(equipment, this);
    }

    public void Unequip(EquipmentSlot slot)
    {
        if (equippedItems.TryGetValue(slot, out var equipment))
        {
            equippedItems.Remove(slot);
            PlayerDataManager.Instance?.UnregisterEquipmentUse(equipment);
        }
    }

    public Equipment GetEquipment(EquipmentSlot slot)
    {
        equippedItems.TryGetValue(slot, out var item);
        return item;
    }
    public CharacterData(FactionType faction, JobType job, CharacterType characterType, int atk, int def, int hp, int maxHp, float atkSpd, float crit, float critDmg, float eva)
    {
        this.faction = faction;
        this.job = job;
        this.type = characterType;
        this.level = 1;
        this.exp = 0;
        this.atk = atk;
        this.def = def;
        this.hp = hp;
        this.maxHp = maxHp;
        this.atkSpd = atkSpd;
        this.crit = crit;
        this.critDmg = critDmg;
        this.eva = eva;

    }

    public static CharacterData Create(FactionType faction, JobType job, CharacterType type, bool isRandomized = false)
    {
        int atk = 0;
        int def = 0;
        int hp = 0;
        float atkSpd = 1f;
        float crit = 0.1f;
        float critDmg = 1.5f;
        float eva = 0.05f;

        switch (job)
        {
            case JobType.Swordsman:
                atk = 10; def = 5; hp = 100;
                break;
            case JobType.Archer:
                atk = 8; def = 3; hp = 80; atkSpd = 1.5f;
                break;
            case JobType.Healer:
                atk = 12; def = 2; hp = 70; crit = 0.2f; critDmg = 2f;
                break;
        }

        switch (type)
        {
            case CharacterType.Goblin:
                hp -= 50;
                atk -= 2;
                def -= 2;
                break;
        }
        

        if (isRandomized)
        {
            atk += Mathf.RoundToInt(Random.Range(-2f, 2f));
            def += Mathf.RoundToInt(Random.Range(-2f, 2f));
            hp += Mathf.RoundToInt(Random.Range(-10f, 10f));
        }

        return new CharacterData(faction, job, type, atk, def, hp, hp, atkSpd, crit, critDmg, eva);
    }

    public void TakeDamage(int damage)
    {
        hp = Mathf.Max(hp - damage, 0);
    }

    public void Heal(int amount)
    {
        hp = Mathf.Min(hp + amount, MaxHp);
    }
    public void LevelUp()
    {
        level++;
        int pointsToDistribute = 3; // 每次升級分配3點屬性

        for (int i = 0; i < pointsToDistribute; i++)
        {
            int statIndex = Random.Range(0, 5); // 決定要升哪一項

            switch (statIndex)
            {
                case 0:
                    atk += Random.Range(1, 3); // +1 ~ +2
                    break;
                case 1:
                    def += Random.Range(1, 3);
                    break;
                case 2:
                    maxHp += Random.Range(5, 11); // +5 ~ +10
                    hp = maxHp; // 順便全補
                    break;
                case 3:
                    crit += 0.01f;
                    break;
                case 4:
                    eva += 0.01f;
                    break;
            }
        }

        //Debug.Log($"角色升級到 Lv.{level}！ATK:{atk}, DEF:{def}, HP:{maxHp}, Crit:{crit:P0}, EVA:{eva:P0}");
    }
    public void GainExp(float amount)
    {
        exp += amount;
        while (exp >= ExpToNextLevel)
        {
            exp -= ExpToNextLevel;
            LevelUp();
        }
    }
    public int SoulPrice()
    {
        return Mathf.CeilToInt(Level * Level / 10f);
    }
    public void ResetHp()
    {
        hp = MaxHp;
    }
}
