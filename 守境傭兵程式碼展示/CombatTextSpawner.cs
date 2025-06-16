using NUnit.Framework;
using UnityEngine;

public class CombatTextSpawner : Singleton<CombatTextSpawner>
{
    [SerializeField] private DamageText damageTextPrefab;
    [SerializeField] private HealText healTextPrefab;
    public void SpawnDamageText(Vector3 position, Vector3 attackerPos, float damage, Color color)
    {
        Vector3 direction = (position - attackerPos).normalized;
        Vector3 jitter = new Vector3(0, 0, 0);
        Vector3 finalOffset = (direction + jitter).normalized * 80f;

        var text = Instantiate(damageTextPrefab, position, Quaternion.identity);
        text.Setup(new DamageText.DamageTextParams
        {
            Damage = damage,
            Color = color,
            OffsetX = finalOffset.x,
            OffsetY = finalOffset.y
        });
    }
    public void SpawnHealText(Vector3 position , float healAmount)
    {
        var text = Instantiate(healTextPrefab, position, Quaternion.identity);
        text.Setup(healAmount);
    }
}

