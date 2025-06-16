using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class MercenaryManager : Singleton<MercenaryManager>
{
    [SerializeField] private List<Character> mercenaryPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<Character> activeMercenaries = new();
    void Start()
    {
        GraveyardManager.Instance.OnCharacterRevived += OnCharacterRevived;
    }

    private void OnCharacterRevived(CharacterData data)
    {
        Hire(data);
        Debug.Log($"Revived!");
    }

    private void AddToTeam(Character merc)
    {
        activeMercenaries.Add(merc);
        merc.transform.position = spawnPoint.position;
    }
    public void Hire(CharacterData characterData)
    {
        switch (characterData.Job)
        {
            case CharacterData.JobType.Swordsman:
                var swordsman = SpawnMercenary<Swordsman>();
                if (swordsman != null)
                {
                    swordsman.SetupCharacter(characterData);
                    AddToTeam(swordsman);
                }
                break;
            case CharacterData.JobType.Archer:
                var archer = SpawnMercenary<Archer>();
                if (archer != null)
                {
                    archer.SetupCharacter(characterData);
                    AddToTeam(archer);
                }
                break;
            case CharacterData.JobType.Healer:
                var healer = SpawnMercenary<Healer>();
                if (healer != null)
                {
                    healer.SetupCharacter(characterData);
                    AddToTeam(healer);
                }
                break;
            default:
                Debug.LogError($"No prefab of type {characterData.Job} found.");
                break;
        }
    }

    public void Dismiss(Character merc)
    {
        activeMercenaries.Remove(merc);
    }

    public List<Character> GetAllMercenaries()
    {
        return activeMercenaries;
    }
    private T SpawnMercenary<T>() where T : Character
    {
        var prefab = mercenaryPrefabs.FirstOrDefault(p => p is T);
        if (prefab == null)
        {
            Debug.LogError($"No prefab of type {typeof(T)} found.");
            return null;
        }

        var instance = Instantiate(prefab);
        return instance as T;
    }
    public Character GetClosestMercenary(Vector3 position)
    {
        Character closestMercenary = null;
        float closestDistance = float.MaxValue;

        foreach (var merc in activeMercenaries)
        {
            float distance = Vector3.Distance(position, merc.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestMercenary = merc;
            }
        }
        if (closestMercenary != null)
        {
            return closestMercenary;
        }
        Debug.LogWarning("No mercenaries available.");
        return null;
    }
    #region Test Methods
    [ContextMenu("Test Hire")]
    private void TestHire()
    {
        var merc = SpawnMercenary<Swordsman>();
        if (merc != null)
        {
            AddToTeam(merc);
            Debug.Log($"Hired {merc.name}");
        }
    }
    [ContextMenu("Spawn Mercenary and Enemy")]
    private void TestSpawnMercenaryAndEnemy()
    {
        Hire(CharacterData.Create(CharacterData.FactionType.Ally, CharacterData.JobType.Swordsman, CharacterData.CharacterType.Human));
        SpawnEnemyManager.Instance.TestSpawnCount(1);
    }
    #endregion
}