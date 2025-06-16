using System.Collections.Generic;
using UnityEngine;

public static class WaveAutoGenerator
{
    public static List<WaveManager.WaveData> GenerateWavesForLevel(int levelIndex)
    {
        int waveCount = GetWaveCount(levelIndex);
        List<WaveManager.WaveData> waveList = new();

        for (int wave = 0; wave < waveCount; wave++)
        {
            var data = GenerateWaveData(levelIndex);
            waveList.Add(data);
        }

        return waveList;
    }

    private static int GetWaveCount(int levelIndex)
    {
        return 3 + levelIndex / 5;
    }

    private static WaveManager.WaveData GenerateWaveData(int levelIndex)
    {
        var data = new WaveManager.WaveData();
        data.spawnInterval = Mathf.Clamp(1.5f - levelIndex * 0.05f, 0.5f, 1.5f);

        int enemiesInThisWave = Mathf.RoundToInt(4 + levelIndex * 1.2f);
        for (int i = 0; i < enemiesInThisWave; i++)
        {
            CharacterData enemy = GenerateEnemy(levelIndex);
            data.AddEnemyData(enemy);
        }

        return data;
    }

    private static CharacterData GenerateEnemy(int levelIndex)
    {
        CharacterData enemy = CharacterData.Create(
            faction: CharacterData.FactionType.Enemy,
            job: GetRandomJob(),
            type: CharacterData.CharacterType.Goblin,
            isRandomized: true
        );

        int levelBias = Random.Range(-1, 3); // -1 ~ +2
        int enemyLevel = Mathf.Max(1, levelIndex + levelBias);

        for (int lv = 1; lv < enemyLevel; lv++)
        {
            enemy.LevelUp();
        }

        return enemy;
    }

    private static CharacterData.JobType GetRandomJob()
    {
        var values = (CharacterData.JobType[])System.Enum.GetValues(typeof(CharacterData.JobType));
        return values[Random.Range(0, values.Length)];
    }

    private static CharacterData.CharacterType GetRandomType()
    {
        var values = (CharacterData.CharacterType[])System.Enum.GetValues(typeof(CharacterData.CharacterType));
        return values[Random.Range(0, values.Length)];
    }
}
