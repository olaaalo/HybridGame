using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    public CameraTarget[] cameraTargets;

    public bool wasPrepared;
    public bool wasActivated;

    private RaycastHit hit;
    public void PlaceOnBottom()
    {
        if (Physics.Raycast(transform.position, transform.up * -1, out hit, Mathf.Infinity, 1 << 13))
        {
            transform.position = hit.point;
        }
    }
}