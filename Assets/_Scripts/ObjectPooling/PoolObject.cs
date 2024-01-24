using System;
using System.Collections;
using UnityEngine;

public class PoolObject : MonoBehaviour, IPoolObject
{
    private Action<IPoolObject> _returnAction;

    public void Init(Action<IPoolObject> returnAction)
    {
        _returnAction = returnAction;
    }

    public void ReturnToPoolByLifeTime(float lifeTime)
    {
        StartCoroutine(ReturnToPoolByLifeTimeCoroutine(lifeTime));
    }
    private IEnumerator ReturnToPoolByLifeTimeCoroutine(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (_returnAction != null)
        {
            _returnAction.Invoke(this);
            _returnAction = null;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}