using System.Collections;
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

    private bool active = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!active && lineRenderer == null)
        {
            return;
        }

        // Cast the ray always upwards. The direction is corrected by the objects' rotation.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, GridController.SIZE_Y * 2, layerMask);

        if (hit.collider != null) {
            // Display the line renderer.
            lineRenderer.SetPosition(TARGET, hit.collider.transform.position);
        }
    }

    public void Deactivate()
    {
        active = false;
        lineRenderer = null;
        Destroy(line);
    }

    public void Activate()
    {
        active = true;
        line = Instantiate(trailPrefab, transform.position, Quaternion.identity);

        // Cache the trail's line renderer.
        lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(SOURCE, transform.position);

    }
}
