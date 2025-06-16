using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnEnemyManager : Singleton<SpawnEnemyManager>
{
    [Header("Enemy Prefabs")]
    [SerializeField] private List<Character> enemyPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnCenter;
    [SerializeField] private float spawnMaxRadius = 5f;
    [SerializeField] private float spawnMinRadius = 4f;
    [SerializeField] private int spawnCount = 1;
    [SerializeField] private List<Character> activeEnemies = new();
    public void TestSpawnCount(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Character prefab = GetRandomEnemyPrefab();
            Vector3 spawnPos = GetRandomPositionAroundCenter();
            Character enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
            activeEnemies.Add(enemy);
            enemy.SetupCharacter(CharacterData.Create(
                faction: CharacterData.FactionType.Enemy,
                job: CharacterData.JobType.Swordsman,
                type: CharacterData.CharacterType.Goblin));
        }
    }
    public void SpawnEnemies(CharacterData enemyData)
    {

        Character prefab = enemyPrefabs.Find(x => x.CharacterData.Type == enemyData.Type && 
            x.CharacterData.Job == enemyData.Job);
        if (prefab == null)
        {
            Debug.LogError($"No prefab found for {enemyData.Type} of job {enemyData.Job}");
            return;
        }   
        
        Vector3 spawnPos = GetRandomPositionAroundCenter();
        Character enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        activeEnemies.Add(enemy);
        enemy.SetupCharacter(enemyData);
    }
    public void SpawnEnemies(CharacterData enemyData , Transform spawnTransform)
    {
        Character prefab = enemyPrefabs.Find(x => x.CharacterData.Type == enemyData.Type &&
            x.CharacterData.Job == enemyData.Job);
        if (prefab == null)
        {
            Debug.LogError($"No prefab found for {enemyData.Type} of job {enemyData.Job}");
            return;
        }

        Vector3 spawnPos = spawnTransform.position;
        Character enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        activeEnemies.Add(enemy);
        enemy.SetupCharacter(enemyData);
    }
    private Character GetRandomEnemyPrefab()
    {
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogError("No enemy prefabs assigned!");
            return null;
        }
        int index = Random.Range(0, enemyPrefabs.Count);
        return enemyPrefabs[index];
    }

    private Vector3 GetRandomPositionAroundCenter()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized; 
        Vector2 offset = randomDirection * Random.Range(spawnMinRadius, spawnMaxRadius);
        return spawnCenter.position + new Vector3(offset.x, offset.y, 0);
    }

    public void ClearAllEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy.gameObject);
        }
        activeEnemies.Clear();
    }
    public Character GetClosestEnemy(Vector3 position)
    {
        Character closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (var enemy in activeEnemies)
        {
            if (enemy == null) continue; // Skip if the enemy is null

            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            return closestEnemy;
        }
        return null;
    }
    public List<Character> GetAllEnemies()
    {
        return activeEnemies;
    }
    public void RemoveEnemy(Character enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }
    #region Test Methods
    [ContextMenu("Spawn 5 Enemies")]
    private void TestSpawn()
    {
        TestSpawnCount(spawnCount);
    }
    #endregion
}
