using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnParticle : MonoBehaviour {

	ParticleSystem particle;

	void OnSpawned ()
	{
		if (particle == null)
			particle = GetComponent<ParticleSystem>();

		particle.Clear();
		particle.Play();

		if (cocoDespawn != null)
			StopCoroutine(cocoDespawn);

		cocoDespawn = CheckDespawnParticle();
		StartCoroutine(cocoDespawn);
	}

	void OnDespawned ()
	{
		if (cocoDespawn != null)
		{
			StopCoroutine(cocoDespawn);
			cocoDespawn = null;
		}
	}
	
	IEnumerator cocoDespawn;
	IEnumerator CheckDespawnParticle ()
	{
		while(particle.isPlaying)
			yield return null;
		
		PoolManager.instance.Despawn(transform);
	}
}
