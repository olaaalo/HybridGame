using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraTarget : MonoBehaviour
{
    public Transform transformTarget;
    public Transform lookAtTarget;
    public DOTweenAnimation tweenAnimation;

#if UNITY_EDITOR
    public bool drawGizmos = true;
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transformTarget.position, 0.5f);
            Gizmos.DrawLine(transformTarget.position, lookAtTarget.position);
        }
    }
#endif

}