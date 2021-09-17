using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Poolable), typeof(ParticleSystem))]
public class PoolableParticles : MonoBehaviour
{
    public Poolable poolable;
    public ParticleSystem particles;

    public bool playOnEnable = true;

    private void OnValidate()
    {
        if (!poolable) poolable = GetComponent<Poolable>();
        if (!particles) particles = GetComponent<ParticleSystem>();
    }

    private void Awake()
    {
        poolable.OnEnqueue += OnEnqueue;
    }

    private void OnDestroy()
    {
        poolable.OnEnqueue -= OnEnqueue;
    }

    private void OnEnqueue(Poolable poolable)
    {
        gameObject.SetActive(true);
        particles.Stop();
    }

    private void OnEnable()
    {
        if (playOnEnable)
            particles.Play();
    }
}
