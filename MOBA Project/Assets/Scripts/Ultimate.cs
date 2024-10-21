using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ultimate : MonoBehaviour
{
    private Color32 COLOR_RED = new Color32(231, 76, 60, 255);
    private Color32 COLOR_BLUE = new Color32(52, 152, 219, 255);

    [SerializeField] private Transform wallHandler;
    [SerializeField] private Transform frozenEffectPrefab;
    [SerializeField] private float stunDuration;
    [SerializeField] private float wallDuration;
    [SerializeField] private float slowDuration;
    [SerializeField] private Vector3 boxSize = new Vector3(2, 2, 2); // Size of the BoxCast
    [SerializeField] private float boxCastDistance = 5f; // Distance the box should cast
    [SerializeField] private LayerMask enemyLayer; // Layer to detect enemies

    private Vector3 castDirection = Vector3.forward; // Direction of the cast
    private bool casted = false;
    private bool detected = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            wallHandler.transform.localPosition = Vector3.zero;
            transform.DOMove(Vector3.zero, 1f);
            detected = false;
            casted = true;
        }

        if (!casted) return;
        if (detected) return;

        if (Vector3.Distance(transform.position, Vector3.zero) < .1f)
        {
            detected = true;
            // Perform the BoxCast
            RaycastHit[] hits = Physics.BoxCastAll(transform.position, boxSize / 2, castDirection, Quaternion.identity, boxCastDistance, enemyLayer);

            foreach (var hit in hits)
            {
                if (hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    StartCoroutine(UltimateEffect(enemy));
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.SetSpeed(.5f);
            enemy.SetColor(COLOR_BLUE);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.SetSpeed();
            enemy.SetColor(COLOR_RED);
        }
    }

    private IEnumerator UltimateEffect(Enemy enemy)
    {
        GameObject frozenEffectClone = Instantiate(frozenEffectPrefab, enemy.transform.position, Quaternion.identity).gameObject;
        enemy.SetColor(COLOR_BLUE);
        enemy.SetStun(true, stunDuration);
        yield return new WaitForSeconds(stunDuration);
        Destroy(frozenEffectClone);
        yield return new WaitForSeconds(wallDuration);
        wallHandler.DOMove(new Vector3(0, -3, 0), 1f);
        yield return new WaitForSeconds(slowDuration);
        transform.DOMoveY(-3, .5f);
    }

    // Gizmo to visualize the BoxCast
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 castEnd = transform.position + castDirection * boxCastDistance;
        Gizmos.DrawWireCube(transform.position, boxSize); // Initial box position
        Gizmos.DrawWireCube(castEnd, boxSize); // Box at the cast end
        Gizmos.DrawLine(transform.position, castEnd); // Line indicating the cast
    }

    public void PerformBoxCast()
    {
        // Perform the BoxCast
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, boxSize / 2, castDirection, Quaternion.identity, boxCastDistance, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                StartCoroutine(UltimateEffect(enemy));
            }
        }
    }
}