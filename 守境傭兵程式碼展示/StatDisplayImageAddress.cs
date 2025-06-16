public static class StatDisplayImageAddress
{
    public static string GetAddress(StatDisplay.StatType statType)
    {
        return statType switch
        {
            StatDisplay.StatType.Attack => "StatIcons[Atk]",
            StatDisplay.StatType.Defense => "StatIcons[Def]",
            StatDisplay.StatType.Health => "StatIcons[Hp]",
            StatDisplay.StatType.AtkSpeed => "StatIcons[AtkSpd]",
            StatDisplay.StatType.CriticalChance => "StatIcons[AtkCrit]",
            StatDisplay.StatType.CriticalDamage => "StatIcons[CritDmg]",
            StatDisplay.StatType.Evasion => "StatIcons[Eva]",
            _ => "UnknowIcon"
        };
    }
}
