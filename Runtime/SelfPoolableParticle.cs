using System;
using UnityEngine;

public class SelfPoolableParticle : SelfPoolable
{
    private void Reset()
    {
        var main = GetComponent<ParticleSystem>().main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnParticleSystemStopped()
    {
        Enqueue();
    }
}