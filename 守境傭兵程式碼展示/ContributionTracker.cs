using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContributionTracker : MonoBehaviour
{
    public HashSet<Character> Damagers = new();
    public HashSet<Character> Supporters = new();


    public void RegisterDamage(Character source)
    {
        Damagers.Add(source);
    }

    public void RegisterHeal(Character healer, Character healedTarget)
    {
        if (Damagers.Contains(healedTarget))
            Supporters.Add(healer);
    }

    public List<Character> GetContributors()
    {
        return Damagers.Union(Supporters).ToList();
    }
}
