using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

public class CameraConstraint : MonoBehaviour
{
    public Camera cam;

    public TransformConstraint transformConstraint;
    public LookAtConstraint lookAtConstraint;

    public float limitTimeToChange;

    public CameraTarget currentCameraTarget;
    public Vehicle currentVehicleTarget;

    [ContextMenu("Apply Camera Constraint")]
    private void Start()
    {
        if (currentCameraTarget != null)
        {
            ChangeConstraint(currentCameraTarget);
            transform.SetParent(transformConstraint.target);
        }
    }

    private float lastChangeTime;

    public void ChangeConstraint(CameraTarget[] camTargets, Vehicle vehicleToLookAt = null)
    {
        ChangeConstraint(camTargets[Random.Range(0, camTargets.Length)], vehicleToLookAt);
    }

    public void ChangeConstraint(CameraTarget camTarget, Vehicle vehicleToLookAt = null)
    {
        if (Time.time - lastChangeTime < limitTimeToChange)
            return;

        lastChangeTime = Time.time;

        currentCameraTarget = camTarget;
        currentVehicleTarget = vehicleToLookAt;

        camTarget.tweenAnimation?.DORestart();

        transformConstraint.target = currentCameraTarget.transformTarget;

        if (currentVehicleTarget == null)
        {
            lookAtConstraint.target = currentCameraTarget.lookAtTarget;
        }
        else
        {
            lookAtConstraint.target = vehicleToLookAt.transform;
        }

        transform.SetParent(transformConstraint.target);
    }
}