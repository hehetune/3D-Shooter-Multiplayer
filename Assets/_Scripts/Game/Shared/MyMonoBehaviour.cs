using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMonoBehaviour : MonoBehaviour
{
    protected virtual void Reset()
    {
        this.LoadComponents();
        this.ResetValues();
    }
    protected virtual void Awake()
    {
        // this.LoadComponents();
        this.ResetValues();
    }
    protected virtual void Start()
    {

    }
    protected virtual void OnEnable()
    {

    }
    protected virtual void LoadComponents()
    {
        // For override
    }
    protected virtual void ResetValues()
    {
        // For override
    }
}
