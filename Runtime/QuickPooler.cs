using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class QuickPooler : MonoBehaviour, IPool
{
    public GameObject prefab;

    [SerializeField] private List<Poolable> elements = new List<Poolable>();
    [SerializeField] private List<Poolable> available = new List<Poolable>();
    [SerializeField] private List<Poolable> unavailable = new List<Poolable>();

    public int prepopulate = 6;

    private void Awake()
    {
        elements = new List<Poolable>();
        available = new List<Poolable>();
        unavailable = new List<Poolable>();

        for (int i = 0; i < prepopulate; i++)
        {
            Poolable p = Dequeue();
            p.gameObject.SetActive(false);
            Enqueue(p);
        }
    }


    public Poolable Dequeue()
    {
        Poolable target;
        if (available.Count == 0)
        {
            GameObject go = Instantiate(prefab, transform);
            target = go.GetComponent<Poolable>();
            elements.Add(target);
        }
        else
        {
            target = available[available.Count-1];
            available.RemoveAt(available.Count - 1);
        }
        unavailable.Add(target);
        return target;
    }

    public void Enqueue(Poolable target)
    {
        unavailable.Remove(target);
        available.Add(target);
        target.transform.SetParent(transform);
        target.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < elements.Count; i++)
            Destroy(elements[i]);
    }

    public void EnqueueAll()
    {
        if (unavailable.Count == 0) return;
        for (int i = unavailable.Count-1; i >= 0; i--)
            Enqueue(unavailable[i]);
    }

    [Button]
    public void Clear()
    {
        EnqueueAll();

        List<Transform> childs = new List<Transform>();
        transform.GetComponentsInChildren(true, childs);
        if (childs.Contains(transform))
            childs.Remove(transform);

        for (int i = childs.Count - 1; i >= 0; i--)
        {
            if(Application.isPlaying)
                Destroy(childs[i].gameObject);
            else
                DestroyImmediate(childs[i].gameObject,true);
        }

        elements.Clear();
        available.Clear();
        unavailable.Clear();
    }
}

public interface IPool
{
    Poolable Dequeue();
    void Enqueue(Poolable target);
}