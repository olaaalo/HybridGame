using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EngineDrone : MonoBehaviour
{
    public Engine engine;
    [HideInInspector] public Vehicle vehicleTarget;

    public float timeToVehicle;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = new Vector3(vehicleTarget.transform.position.x - 15f, 10f, 45f);
        transform.position = startPosition;

        transform.DORotate(Vector3.up * -90f, 0.8f).SetDelay(timeToVehicle - 1f);
        transform.DOMoveX(vehicleTarget.engineTransform.position.x + 1f, timeToVehicle).SetEase(Ease.InQuad);
        transform.DOMoveY(vehicleTarget.engineTransform.position.y + 3f, timeToVehicle).SetEase(Ease.OutSine);
        transform.DOMoveZ(vehicleTarget.engineTransform.position.z, timeToVehicle).SetEase(Ease.InSine)
            .OnComplete(() => {
                engine.transform.DOMove(vehicleTarget.engineTransform.position, 1f)
                    .OnComplete(() => {
                        vehicleTarget.engine = engine;
                        vehicleTarget.Repared();
                        engine = null;
                        vehicleTarget = null;

                        transform.DORotateQuaternion(Quaternion.identity, 1f);
                        transform.DOMove(startPosition, timeToVehicle / 1.5f);
                    });
            });
    }
}
