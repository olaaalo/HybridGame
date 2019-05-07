using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetZone : MonoBehaviour
{
    public CameraTarget[] cameraTargets;

    public GameObject shapeObject;

    public bool wasPrepared;
    public bool wasActivated;

    public void Activate()
    {
        shapeObject.SetActive(false);
    }
}