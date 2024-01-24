using UnityEngine;

public abstract class BaseThrowObject : MonoBehaviour
{
    public abstract void Throw(Vector3 direction, float force);
}