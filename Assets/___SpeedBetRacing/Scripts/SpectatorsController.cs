using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorsController : MonoBehaviour
{
    public GameObject[] spectatorPrefabs;
    public Transform[] spectatorTransforms;

    public List<Spectator> spectators;

    private void OnDrawGizmos()
    {
        for (int i = 0; i < spectatorTransforms.Length; ++i)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(spectatorTransforms[i].position, 0.5f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(spectatorTransforms[i].position, spectatorTransforms[i].position + spectatorTransforms[i].forward);
        }
    }

    private void Start()
    {
        spectators = new List<Spectator>();

        for (int i = 0; i < spectatorTransforms.Length; ++i)
        {
            if (Random.value > 0.1f)
            {
                spectators.Add(Instantiate(
                        spectatorPrefabs[Random.Range(0, spectatorPrefabs.Length)],
                        spectatorTransforms[i].position, spectatorTransforms[i].rotation,
                        spectatorTransforms[i])
                    .GetComponent<Spectator>());
            }
        }
    }
}