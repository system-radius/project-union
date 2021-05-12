using System;
using System.Collections.Generic;
using UnityEngine;

public class DivideSource : MonoBehaviour
{
    public GameObject trailPrefab;

    [SerializeField] private LayerMask layerMask;

    private const int SOURCE = 0;

    private const int TARGET = 1;

    private GameObject line;

    private LineRenderer lineRenderer;

    private Transform targetTransform;

    private CapsuleCollider2D collider;

    private bool active = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!active || line == null || targetTransform == null)
        {
            return;
        }

        lineRenderer.SetPosition(TARGET, targetTransform.position);
        
        collider.size = new Vector2(0.5f, (targetTransform.position - transform.position).magnitude);
        collider.transform.position = transform.position + (targetTransform.position - transform.position) / 2;
        
    }

    public void Deactivate()
    {
        if (!active)
        {
            return;
        }

        active = false;
        collider = null;
        lineRenderer = null;
        Destroy(line);
    }

    public void Activate(GameObject bullet)
    {
        if (active) {
            return;
        }

        active = true;
        line = Instantiate(trailPrefab, transform.position, Quaternion.identity);

        // Cache the trail's line renderer.
        lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(SOURCE, transform.position);

        // Create a capsule collider attached to the line renderer.
        collider = line.GetComponent<CapsuleCollider2D>();

        targetTransform = bullet.transform;
        lineRenderer.SetPosition(TARGET, targetTransform.position);

        collider.transform.rotation = transform.rotation;
    }
}
