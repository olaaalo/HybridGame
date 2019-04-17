using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonInstantiator : MonoBehaviour {

    public GameManager gameManagerPrefab;
    public SoundManager soundManagerPrefab;

    void Awake()
    {
        if (!GameManager.instanceExists && gameManagerPrefab != null)
            Instantiate(gameManagerPrefab);
        if (!SoundManager.instanceExists && soundManagerPrefab != null)
            Instantiate(soundManagerPrefab);
    }
}
