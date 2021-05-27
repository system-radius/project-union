using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A static class responsible for handling/monitoring
 * the enemies on the level.
 */
public class EnemyManager
{

    private static EnemyManager instance;

    // The array of available enemies to be spawned.
    private GameObject[] enemyPrefabs;

    // The container for the enemies.
    private GameObject enemyContainer;

    // The list of the enemy data to be spawned.
    private List<EnemyData> enemyData;

    public static void CreateInstance(GameObject[] enemyPrefabs)
    {
        if (instance != null)
        {
            // No need to create another instance.
            return;
        }

        instance = new EnemyManager(enemyPrefabs);
    }

    private EnemyManager(GameObject[] enemyPrefabs)
    {
        // Set the enemy prefabrication objects.
        this.enemyPrefabs = enemyPrefabs;

        // Create the list for the enemy data.
        enemyData = new List<EnemyData>();

        // Create the object to contain all enemies.
        enemyContainer = new GameObject("EnemyContainer");
    }

    /**
     * Create the enemies list.
     */
    public static List<EnemyData> CreateEnemyList(int level)
    {
        // Begin by clearing the enemy data list.
        instance.enemyData.Clear();

        List<EnemyData> list = new List<EnemyData>();

        GameObject[] prefabs = instance.enemyPrefabs;

        // Get whichever is lower
        int limit = Mathf.Min(level + 1, prefabs.Length);
        int spawnLimit = Mathf.Max(1, level - 2);

        for (int i = 0; i < limit; i++)
        {
            list.Add(new EnemyData(prefabs[i], spawnLimit));
        }

        instance.enemyData.AddRange(list);

        return list;
    }

    public static List<EnemyData> GetEnemyData()
    {
        return instance.enemyData;
    }

    public static void CreateEnemies()
    {
        List<EnemyData> data = instance.enemyData;

        foreach (EnemyData enemyData in data)
        {
            for (int i = 0; i < enemyData.GetCount(); i++) {
                GameObject.Instantiate(enemyData.GetPrefab(), instance.enemyContainer.transform);
            }
        }
    }

    public static void DestroyEnemies()
    {
        foreach (Transform child in instance.enemyContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static void ResetEnemies(int currentLevel = -1)
    {
        if (currentLevel >= 0)
        {
            CreateEnemyList(currentLevel);
        }

        DestroyEnemies();
        CreateEnemies();
    }

    public static int GetEnemyCount()
    {
        return instance.enemyContainer.transform.childCount;
    }

    public static int CountOnPosition(List<Vector2> coords)
    {
        int count = 0;

        foreach (Transform enemy in instance.enemyContainer.transform)
        {
            float x = enemy.transform.position.x;
            float y = enemy.transform.position.y;

            count += coords.Contains(DividerUtils.UnitToGridPoint(x, y)) ? 1 : 0;
        }

        Debug.Log("Count on pos: " + count);

        return count;
    }
}
