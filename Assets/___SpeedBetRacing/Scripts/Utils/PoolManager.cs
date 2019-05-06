using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
	[SerializeField] private GameObject explosionParticlePrefab;
	[SerializeField] private int nbExplosionParticle;
	private List<Transform> explosionParticleList;


	private void Start()
	{
		explosionParticleList = new List<Transform>();
		for (int i = 0; i < nbExplosionParticle; ++i)
		{
			var t = Instantiate(explosionParticlePrefab, transform);
			t.gameObject.SetActive(false);
			explosionParticleList.Add(t.transform);
		}
	}

	private Transform tTemp;
	private Transform Pool(List<Transform> list, GameObject objectPrefab)
	{
		tTemp = null;

		for (int i = 0; i < explosionParticleList.Count; ++i)
		{
			if (!explosionParticleList[i].gameObject.activeInHierarchy)
			{
				tTemp = explosionParticleList[i];
				break;
			}
		}
		if (tTemp == null)
		{
			Debug.LogWarning("[PoolManager] Instantiate new pool object for list : \"" + list + "\"");

			var t = Instantiate(objectPrefab, transform);
			t.gameObject.SetActive(false);
			explosionParticleList.Add(t.transform);

			return Pool(list, objectPrefab);
		}

		tTemp.gameObject.SetActive(true);
		tTemp.BroadcastMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);

		return tTemp;
	}


	public Transform GetExplosionParticle(Vector3 position, Transform parent)
	{
		var t = Pool(explosionParticleList, explosionParticlePrefab);
		t.SetParent(parent);
		t.position = position;
		return t;
	}

	public void Despawn(Transform toDespawn)
	{
		toDespawn.BroadcastMessage("OnDespawned", SendMessageOptions.DontRequireReceiver);
		toDespawn.gameObject.SetActive(false);
		toDespawn.SetParent(instance.transform);
	}
}