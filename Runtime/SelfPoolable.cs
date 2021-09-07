using Pooling.Poolers;
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Poolable))]
public class SelfPoolable : MonoBehaviour
{
    public Poolable poolable;
    IEnumerator currentRoutine;

    public bool startWaitTimeOnEnable = false;
    [ShowIf("startWaitTimeOnEnable")]public float waitTime = 1;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnValidate()
    {
        if (!poolable)
            poolable = GetComponent<Poolable>();
    }
    private void OnEnable()
    {
        if (startWaitTimeOnEnable)
            WaitAndPool(waitTime);
    }

    public void Enqueue()
    {
        GameObjectPoolController.Enqueue(poolable);
    }

    public void WaitAndPool(float targetWaitTime)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = WaitAndPoolRoutine(targetWaitTime);
        StartCoroutine(currentRoutine);
    }

    IEnumerator WaitAndPoolRoutine(float t )
    {
        yield return new WaitForSeconds(t);
        Enqueue();
    }

    private void OnDisable()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
    }

    private void OnDestroy()
    {
    }
}


public static class SelfPoolableGameObjectExtension
{
    public static void ClearSelfPoolables(this GameObject go)
    {
        SelfPoolable[] poolables = go.GetComponentsInChildren<SelfPoolable>();
        foreach (SelfPoolable item in poolables)
        {
            item.Enqueue();
        }
    }
}