using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace LibLabGames.SpeedBetRacing
{
    public class EngineDrone : MonoBehaviour
    {
        public Engine engine;
        [HideInInspector] public Vehicle vehicleTarget;

        public float timeToVehicle;
        private Vector3 startPosition;

        private void Start()
        {
            transform.SetParent(GameManager.instance.circuitParent);

            startPosition = vehicleTarget.transform.localPosition + new Vector3(-15f, 10f, 45f);
            transform.localPosition = startPosition;
            transform.rotation = vehicleTarget.transform.rotation;

            transform.DOLocalRotate(Vector3.up * -90f, 0.8f).SetDelay(timeToVehicle - 1f).SetRelative();
            transform.DOMoveX(vehicleTarget.engineTransform.position.x + 1f, timeToVehicle).SetEase(Ease.InQuad);
            transform.DOMoveY(vehicleTarget.engineTransform.position.y + 3f, timeToVehicle).SetEase(Ease.OutSine);
            transform.DOMoveZ(vehicleTarget.engineTransform.position.z, timeToVehicle).SetEase(Ease.InSine)
                .OnComplete(() =>
                {
                    engine.transform.DOMove(vehicleTarget.engineTransform.position, 1f)
                        .OnComplete(() =>
                        {
                            vehicleTarget.engine = engine;
                            vehicleTarget.Repared();
                            engine = null;
                            vehicleTarget = null;

                            transform.DOLocalRotateQuaternion(Quaternion.identity, 1f);
                            transform.DOLocalMove(startPosition, timeToVehicle / 1.5f);
                        });
                });
        }
    }
}