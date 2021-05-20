using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData
{
    private GameObject enemyPrefab;

    private int enemyCount;

    public EnemyData(GameObject enemyPrefab, int enemyCount)
    {
        this.enemyPrefab = enemyPrefab;
        this.enemyCount = enemyCount;
    }

    public GameObject GetPrefab()
    {
        return enemyPrefab;
    }

    public int GetCount()
    {
        return enemyCount;
    }
}