using UnityEngine;

public class BattleManager
{
    public static void RegisterHealContribution(Character healer, Character healedTarget)
    {
        foreach (var enemy in SpawnEnemyManager.Instance.GetAllEnemies())
        {
            if (enemy.TryGetComponent<ContributionTracker>(out ContributionTracker e))
            {
                e.RegisterHeal(healer, healedTarget);
            }
        }
    }
}
