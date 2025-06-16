using System.Collections.Generic;
using UnityEngine;

public partial class WaveManager
{
    [System.Serializable]
    public class WaveData
    {
        public float spawnInterval;
        [SerializeField] List<CharacterData> enemyDataList = new();
        public List<CharacterData> EnemyDataList => enemyDataList;
        public void AddEnemyData(CharacterData enemyData)
        {
            enemyDataList.Add(enemyData);
        }
    }
}
