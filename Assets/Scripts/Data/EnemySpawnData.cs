using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnData : MonoBehaviour
{
    // A static class, responsible for providing data regarding monster spawns.
    private static EnemySpawnData instance;

    public GameObject[] enemyPrefabs;

    // The only method needed from Un   y so that the instance is cached.
    void Awake()
    {
        instance = this;
    }

    public static EnemySpawnData GetInstance()
    {
        return instance;
    }

    public static List<EnemyData> GetSpawnList(int level)
    {
        List<EnemyData> list = new List<EnemyData>();

        GameObject[] prefabs = instance.enemyPrefabs;

        // Get whichever is lower
        int limit = Mathf.Min(level + 1, prefabs.Length);

        for (int i = 0; i < limit; i++)
        {
            list.Add(new EnemyData(prefabs[i], limit));
        }

        return list;
    }
}
