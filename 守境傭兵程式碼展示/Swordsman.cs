using System.Linq;
using Mono.Cecil.Cil;
using Unity.Collections;
using UnityEngine;

public class Swordsman : Character
{
    public override void TakeDamage(int damage)
    {
        if (GetSkill(out PassiveShieldDefenseSkill shieldSkill) && shieldSkill.TryToTrigger())
        {
            Debug.Log("Shield blocked the damage.");
            return;
        }
        base.TakeDamage(damage);
    }
    public override void LevelUp()
    {
        base.LevelUp();
        if (CharacterData.Level == 5)
        {
            LearnSkill(PassiveShieldDefenseSkill.Create());
            Debug.Log("Learned Passive Shield Defense Skill at level 5.");
        }
    }
    [ContextMenu("Add Passive Shield Defense Skill")]
    public void AddPassiveShieldDefenseSkill()
    {
        if (GetSkill(out PassiveShieldDefenseSkill shieldSkill))
        {
            Debug.Log("Already has Passive Shield Defense Skill.");
            return;
        }
        LearnSkill(PassiveShieldDefenseSkill.Create());
        Debug.Log("Added Passive Shield Defense Skill.");
    }
}
