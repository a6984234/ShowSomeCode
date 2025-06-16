using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;

public partial class WaveManager : MonoBehaviour
{
    [SerializeField] private List<WaveData> waveList;
    [SerializeField] private float timeBetweenWaves = 10f;
    [SerializeField] private float nextWaveDelay = 15f;
    [SerializeField] TimeBar timeBar;

    private int currentWaveIndex = 0;
    private int currentLevelIndex = 1;
    private bool isFirstWave = true;

    public bool AllWavesCompleted => currentWaveIndex >= waveList.Count;
    void Start()
    {
        Character.OnCharacterDied += OnEnemyDied;
    }
    public async UniTask StartWaves()
    {
        currentWaveIndex = 0;
        await SpawnCurrentWave();
    }

    public async UniTask SpawnCurrentWave()
    {
        if (isFirstWave)
        {
            isFirstWave = false;
            await DOTween.To(() => timeBetweenWaves, x =>
                {
                    timeBar?.UpdateTime(x, timeBetweenWaves);
                }, 0, timeBetweenWaves).SetEase(Ease.Linear).AsyncWaitForCompletion();
        }
        if (AllWavesCompleted)
        {
            Debug.Log("All waves cleared!");
            return;
        }

        WaveData currentWave = waveList[currentWaveIndex];
        foreach (var enemyData in currentWave.EnemyDataList)
        {
            SpawnEnemyManager.Instance.SpawnEnemies(enemyData);
            await UniTask.Delay(System.TimeSpan.FromSeconds(currentWave.spawnInterval));
        }

        currentWaveIndex++;
        await DOTween.To(() => timeBetweenWaves, x =>
        {
            timeBar?.UpdateTime(x, timeBetweenWaves);
        }, 0, timeBetweenWaves).SetEase(Ease.Linear).AsyncWaitForCompletion();
        HandleNextWave();
    }
    private void HandleNextWave()
    {
        if (AllWavesCompleted)
        {
            Debug.Log("All waves cleared!");
            return;
        }

        if (currentWaveIndex < waveList.Count)
        {
            SpawnCurrentWave().Forget();
        }
    }
    private void OnEnemyDied(Character character)
    {
        if (character.CharacterData.Faction == CharacterData.FactionType.Enemy)
        {
            if (SpawnEnemyManager.Instance.GetAllEnemies().Count == 0)
            {
                if (AllWavesCompleted)
                {
                    Debug.Log("All waves cleared!");
                    UniTask.Void(async () =>
                    {
                        await UniTask.Delay(System.TimeSpan.FromSeconds(nextWaveDelay));
                        currentLevelIndex++;
                        currentWaveIndex = 0;
                        waveList = WaveAutoGenerator.GenerateWavesForLevel(currentLevelIndex);
                        await SpawnCurrentWave();
                    });
                }
            }
        }
    }
    [ContextMenu("Test Spawn Wave")]
    public void TestSpawnWave()
    {
        waveList = WaveAutoGenerator.GenerateWavesForLevel(1);
        SpawnCurrentWave().Forget();
    }
}
