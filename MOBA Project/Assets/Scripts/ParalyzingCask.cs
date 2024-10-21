using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalyzingCask : MonoBehaviour
{
    private Color32 COLOR_RED = new Color32(231, 76, 60, 255);
    private Color32 COLOR_VIOLET = new Color32(155, 89, 182, 255);

    [SerializeField] private List<Enemy> enemyList;
    [SerializeField] private Enemy enemyTarget;
    [SerializeField] private Transform enemyTargetTransform;
    [SerializeField] private float baseSpeed; // Base speed, speed multiplier
    [SerializeField] private float arcHeight = 2f;  // Height of the arc

    [SerializeField] private float stunDuration;

    private Vector3 startPosition;
    private float journeyLength;
    private bool isLaunched = false;
    private float time;
    private float speed; // Dynamically adjusted speed based on distance

    private void Start()
    {
        enemyList = new List<Enemy>(FindObjectsOfType<Enemy>());
        InvokeRepeating(nameof(SetTarget), 1, 1);
    }

    private void Update()
    {
        if (!isLaunched || enemyTargetTransform == null) return;

        // Update time based on distance-adjusted speed
        time += Time.deltaTime * speed / journeyLength;

        // Calculate the new position with arcing
        Vector3 currentPosition = Vector3.Lerp(startPosition, enemyTargetTransform.position, time);
        float heightOffset = arcHeight * Mathf.Sin(Mathf.Clamp01(time) * Mathf.PI); // Creates the arc

        currentPosition.y += heightOffset;

        transform.position = currentPosition;

        // Check if we have reached the target
        if (Vector3.Distance(transform.position, enemyTargetTransform.position) <= 0.1f)
        {
            transform.position = enemyTargetTransform.position;  // Snap to target position
            transform.parent = enemyTargetTransform;
            isLaunched = false;  // Stop movement after reaching the target
            enemyTarget.SetStun(true, stunDuration);
            enemyTarget.SetColor(COLOR_VIOLET);
        }
    }

    private Transform GetClosestEnemy(List<Enemy> enemies)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (Enemy t in enemies)
        {
            if (enemyTargetTransform != null)
            {
                if (t.name == enemyTargetTransform.name) continue;
            }
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                enemyTarget = t;
                tMin = t.transform;
                minDist = dist;
            }
        }

        return tMin;
    }

    private void SetTarget()
    {
        transform.parent = null;
        enemyTargetTransform = GetClosestEnemy(enemyList);

        if (enemyTargetTransform != null)
        {
            startPosition = transform.position;
            journeyLength = Vector3.Distance(startPosition, enemyTargetTransform.position);
            time = 0;  // Reset the time for the arc

            // Adjust speed based on the distance; farther enemies increase the speed
            speed = baseSpeed * journeyLength;

            isLaunched = true;
        }
    }
}