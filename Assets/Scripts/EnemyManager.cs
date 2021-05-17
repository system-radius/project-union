using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public GameObject enemyPrefab;

    private static EnemyManager instance;

    private List<GameObject> enemies;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        enemies = new List<GameObject>();

        enemies.Add(Instantiate(enemyPrefab, transform));
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
