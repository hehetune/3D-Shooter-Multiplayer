using System;
using UnityEngine;

public interface IPoolObject
{
    GameObject gameObject { get; }
    void Init(Action<IPoolObject> returnAction);

    void ReturnToPool();
}