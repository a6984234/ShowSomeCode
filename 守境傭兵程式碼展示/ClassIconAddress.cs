public static class ClassIconAddress
{
    public static string GetAddress(CharacterData.JobType jobType)
    {
        return jobType switch
        {
            CharacterData.JobType.Swordsman => "ClassIcons[Sword]",
            CharacterData.JobType.Archer    => "ClassIcons[Archer]",
            CharacterData.JobType.Healer    => "ClassIcons[Healer]",
            _ => null
        };
    }
}
