using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class Healer : Character
{
    [SerializeField] private float healRange = 3f;
    [SerializeField] Character healTarget;
    private float healAmount = 3f;
    protected override void TickAI()
    {
        TickAttack();
        TickSkill();
        if (moveTarget == null)
        {
            healTarget = FindHealTarget();
            moveTarget = healTarget?.transform;
            if (moveTarget != null)
                navMeshAgent.SetDestination(moveTarget.position);
        }
        if (moveTarget == null) return;
        if (HasReachedDestination() && canAttack)
            HealAlly(healTarget);
        else
            MoveToTarget();
    }
    private Character FindHealTarget()
    {
        var group = CharacterData.Faction == CharacterData.FactionType.Enemy
            ? SpawnEnemyManager.Instance.GetAllEnemies()
            : MercenaryManager.Instance.GetAllMercenaries();

        Character lowest = null;
        float lowestRatio = 1f;

        foreach (var ally in group)
        {
            if (ally == this) continue;

            float ratio = (float)ally.CharacterData.Hp / ally.CharacterData.MaxHp;
            if (ratio >= 1f) continue; // 不需補血

            float dist = Vector3.Distance(transform.position, ally.transform.position);
            if (dist > healRange) continue;

            if (ratio < lowestRatio)
            {
                lowestRatio = ratio;
                lowest = ally;
            }
        }
        if (lowest == null)
        {
            foreach (var ally in group)
            {
                float ratio = (float)ally.CharacterData.Hp / ally.CharacterData.MaxHp;
                if (ratio < lowestRatio)
                {
                    lowestRatio = ratio;
                    lowest = ally;
                }
            }
        }
        // if (lowest != null)
        //     Debug.Log($"Lowest HP ratio: {lowestRatio} for {lowest?.CharacterData.Type} , obj is {CharacterData.Faction}");
        return lowest;
    }
    private void HealAlly(Character target)
    {
        if (target == null) return;
        atkTimer = 1f / CharacterData.AtkSpd;
        float healValue = healAmount + Random.Range(0, CharacterData.Atk);
        int healInt = Mathf.RoundToInt(healValue);
        target.Heal(healInt);
        CombatTextSpawner.Instance.SpawnHealText(target.transform.position, healInt);
        BattleManager.RegisterHealContribution(this, target);
        ResetMoveTarget();
    }
    private void ResetMoveTarget()
    {
        moveTarget = null;
        healTarget = null;
    }
    public override void LevelUp()
    {
        base.LevelUp();
        if(CharacterData.Level == 5)
        {
            var groupHealSkill = new GroupHealSkill(this, 10f, healRange + 2, 2f);
            LearnSkill(groupHealSkill);
            Debug.Log("Learned Group Heal Skill at level 5.");
        }
    }
}
