using UnityEngine;

public class PassiveShieldDefenseSkill : PassiveSkill
{
    public static PassiveShieldDefenseSkill Create()
    {
        var skill = new PassiveShieldDefenseSkill();
        skill.Name = "Passive Shield Defense";
        skill.Rarity = 0.1f;
        skill.Cooldown = 0;
        skill.TriggerTime = UseTime.OnHit;
        return skill;
    }
}
