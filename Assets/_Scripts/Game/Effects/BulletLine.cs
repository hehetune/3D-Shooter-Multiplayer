using System.Collections;
using UnityEngine;

public class BulletLine : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;

    public void SetTargetPosition(Vector3 startPosition, Vector3 targetPosition)
    {
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPositions(new Vector3[] { startPosition, targetPosition });
        // transform.position = startPosition;
        // transform.LookAt(targetPosition);
        StopAllCoroutines();
        StartCoroutine(DespawnAfterOneFrame());
    }

    private IEnumerator DespawnAfterOneFrame()
    {
        yield return null;
        GetComponent<PoolObject>().ReturnToPool();
    }
}