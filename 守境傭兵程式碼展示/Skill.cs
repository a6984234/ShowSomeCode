using Cysharp.Threading.Tasks;

public abstract class ActiveSkill : ISkill
{
    public string Name { get; set; }
    public float Cooldown { get; set; }
    public float Duration { get; set; }
    public bool IsOnCooldown { get; private set; }
    public ActiveSkill(string name, float cooldown, float duration)
    {
        Name = name;
        Cooldown = cooldown;
        Duration = duration;
        IsOnCooldown = false;
    }
    public void TryUse(Character target)
    {
        if (!CanUse()) return;
        Use(target);
        StartCooldown();
    }

    public abstract void Use(Character target);
    public virtual bool CanUse()
    {
        return !IsOnCooldown;
    }

    public void StartCooldown()
    {
        UniTask.Void(async () =>
        {
            IsOnCooldown = true;
            await UniTask.Delay(System.TimeSpan.FromSeconds(Cooldown));
            ResetCooldown();
        });
    }

    public void ResetCooldown()
    {
        IsOnCooldown = false;
    }
}