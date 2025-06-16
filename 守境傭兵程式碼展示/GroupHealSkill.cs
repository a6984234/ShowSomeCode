using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GroupHealSkill : ActiveSkill
{
    private readonly Healer owner;
    private readonly float healRange;
    private readonly float baseHealAmount;
    private readonly int healCount;

    public GroupHealSkill(Healer owner, float coolDown, float range, float healAmount, int healCount = 3)
        : base("Group Heal", coolDown, 0f)
    {
        this.owner = owner;
        healRange = range;
        baseHealAmount = healAmount;
        this.healCount = healCount;
    }

    public override void Use(Character target)
    {
        var group = owner.CharacterData.Faction == CharacterData.FactionType.Enemy
            ? SpawnEnemyManager.Instance.GetAllEnemies()
            : MercenaryManager.Instance.GetAllMercenaries();

        var targets = group
            .Where(a => Vector3.Distance(owner.transform.position, a.transform.position) <= healRange)
            .OrderBy(a => (float)a.CharacterData.Hp / a.CharacterData.MaxHp)
            .Take(healCount)
            .ToList();
        if (targets.Count < 2)
        {
            return;
        }
        foreach (var ally in targets)
        {
            float healValue = baseHealAmount + Random.Range(0, owner.CharacterData.Atk);
            int healInt = Mathf.RoundToInt(healValue);
            ally.Heal(healInt);
            CombatTextSpawner.Instance.SpawnHealText(ally.transform.position, healInt);
            BattleManager.RegisterHealContribution(owner, ally);
        }
        Debug.Log($"Group Heal used by {owner.CharacterData.Type} on {targets.Count} allies.");
    }
}
