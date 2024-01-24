using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _progress;

    [SerializeField] private float _speed = 40f;

    private void Update()
    {
        _progress += Time.deltaTime * _speed;
        transform.position = Vector3.Lerp(_startPosition, _targetPosition, _progress);
    }

    public void SetTargetPosition(Vector3 startPosition, Vector3 targetPosition)
    {
        _progress = 0f;
        _startPosition = startPosition;
        _targetPosition = targetPosition;
        transform.position = _startPosition;
        transform.LookAt(targetPosition);
    }
}