using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class PassiveSkill : ISkill
{
    public string Name { get; set; }
    public float Rarity { get; set; }
    public float Cooldown { get; set; }
    protected bool IsOnCooldown { get; private set; }
    public UseTime TriggerTime { get; set; }
    public bool TryToTrigger()
    {
        var rare = Random.value;
        return !IsOnCooldown && rare <= Rarity;
    }

    protected virtual void StartCooldown()
    {
        UniTask.Void(async () =>
        {
            IsOnCooldown = true;
            await UniTask.Delay(System.TimeSpan.FromSeconds(Cooldown));
            ResetCooldown();
        });
    }
    protected virtual void ResetCooldown()
    {
        IsOnCooldown = false;
    }
    public enum UseTime
    {
        OnAttack,
        OnHit,
        OnDeath,
        OnSpawn,
        OnCooldownEnd
    }
}
