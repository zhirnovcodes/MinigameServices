using System.Collections.Generic;
using UnityEngine;

public class ParticlesEffect : MonoBehaviour
{
    public float Duration;
    public GameObject ContentRoot;
    public List<ParticleSystem> Particles;


    public float GetDuration()
    {
        return Duration;
    }

    public void Enable()
    {
        ContentRoot.SetActive(true);
    }

    public void Disable()
    {
        ContentRoot.SetActive(false);
    }

    public void Play()
    {
		for (int i = 0; i < Particles.Count; i++)
		{
			var ps = Particles[i];
			ps.Clear(true);
			ps.Play(true);
		}
    }

    public void Stop()
    {
		for (int i = 0; i < Particles.Count; i++)
		{
			var ps = Particles[i];
			ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		}
    }
}
