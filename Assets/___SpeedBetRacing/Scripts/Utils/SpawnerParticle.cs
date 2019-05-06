using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerParticle : MonoBehaviour
{
	ParticleSystem particle;

	public FMODUnity.StudioEventEmitter eventEmitter;

	void OnSpawned()
	{
		if (particle == null)
			particle = GetComponent<ParticleSystem>();

		particle.Clear();
		particle.Play();

		if (cocoDespawn != null)
			StopCoroutine(cocoDespawn);

		cocoDespawn = CheckDespawnParticle();
		StartCoroutine(cocoDespawn);

		eventEmitter?.Play();
	}

	void OnDespawned()
	{
		if (cocoDespawn != null)
		{
			StopCoroutine(cocoDespawn);
			cocoDespawn = null;
		}
	}

	IEnumerator cocoDespawn;
	IEnumerator CheckDespawnParticle()
	{
		while (particle.isPlaying)
			yield return null;

		PoolManager.instance.Despawn(transform);
	}
}