using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    // The Divider prefab to be created.
    public GameObject divider;

    // Called when the scene starts.
    void Awake()
    {
        SpawnDivider();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnDivider()
    {
        Instantiate(divider);
    }
}
