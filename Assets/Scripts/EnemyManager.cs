using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;

    private List<GameObject> enemies;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        enemies = new List<GameObject>();
    }

    public void Reset()
    {
        foreach (GameObject enemy in enemies)
        {
            // Destroy all enemies
            Destroy(enemy);
        }

        enemies.Clear();
        List<EnemyData> enemyData = EnemySpawnData.GetSpawnList(GameController.GetInstance().GetLevel());

        foreach (EnemyData data in enemyData)
        {
            GameObject prefab = data.GetPrefab();
            int count = data.GetCount();

            for (int i = 0; i < count; i++)
            {
                enemies.Add(Instantiate(prefab, transform));
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i] == null)
            {
                enemies.RemoveAt(i);
            }
        }
    }

    public static int GetEnemyCount()
    {
        return instance.enemies.Count;
    }

    public static void ResetEnemies()
    {
        instance.Reset();
    }

    public static int CountOnPosition(List<Vector2> coords)
    {

        List<GameObject> enemyList = instance.enemies;
        int count = 0;

        foreach (GameObject enemy in enemyList)
        {
            float x = enemy.transform.position.x;
            float y = enemy.transform.position.y;

            count += coords.Contains(DividerUtils.UnitToGridPoint(x, y)) ? 1 : 0;
        }

        Debug.Log("Count on pos: " + count);

        return count;
    }
}
