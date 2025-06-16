using System;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardManager : Singleton<GraveyardManager>
{
    private readonly List<CharacterData> deadCharacters = new();
    public event Action<CharacterData> OnCharacterBuried;
    public event Action<CharacterData> OnCharacterRevived;
    void Start()
    {
        Character.OnCharacterDied += OnCharacterDied;
    }
    private void OnCharacterDied(Character character)
    {
        if (character.CharacterData.Faction == CharacterData.FactionType.Ally)
        {
            Bury(character.CharacterData);
        }
    }
    public void Bury(CharacterData character)
    {
        if (!deadCharacters.Contains(character))
        {
            deadCharacters.Add(character);
            OnCharacterBuried?.Invoke(character);
        }
    }
    public void Revive(CharacterData target)
    {
        if (deadCharacters.Contains(target))
        {
            deadCharacters.Remove(target);
            OnCharacterRevived?.Invoke(target);
        }
    }
    public IReadOnlyList<CharacterData> GetDeadCharacters() => deadCharacters.AsReadOnly();
}
