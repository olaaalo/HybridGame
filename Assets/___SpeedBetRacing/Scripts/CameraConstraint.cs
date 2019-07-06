using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

namespace LibLabGames.SpeedBetRacing
{
    public class CameraConstraint : MonoBehaviour
    {
        public Camera cam;

        public TransformConstraint transformConstraint;
        public LookAtConstraint lookAtConstraint;

        public float limitTimeToChange;

        public CameraTarget currentCameraTarget;
        public Vehicle currentVehicleTarget;

        public float timeToRandomChange;
        private WaitForSeconds waitRandomChange;

        public bool canChange;
        public int vehicleIDLooked;

        [ContextMenu("Apply Camera Constraint")]
        private void Start()
        {
            if (currentCameraTarget != null)
            {
                ChangeConstraint(currentCameraTarget);
                transform.SetParent(transformConstraint.target);
            }

            waitRandomChange = new WaitForSeconds(timeToRandomChange);
        }

        private float lastChangeTime;

        public void ChangeConstraint(CameraTarget[] camTargets, int ID, Vehicle vehicleToLookAt = null)
        {
            if (Time.time - lastChangeTime < limitTimeToChange)
                return;
            
            vehicleIDLooked = ID;

            ChangeConstraint(camTargets[Random.Range(0, camTargets.Length)], vehicleToLookAt);
        }

        public void ChangeConstraint(CameraTarget[] camTargets, Vehicle vehicleToLookAt = null)
        {
            ChangeConstraint(camTargets[Random.Range(0, camTargets.Length)], vehicleToLookAt);
        }

        public void ChangeConstraint(CameraTarget camTarget, Vehicle vehicleToLookAt = null)
        {
            if (!canChange) return;

            if (Time.time - lastChangeTime < limitTimeToChange)
                return;

            lastChangeTime = Time.time;

            currentCameraTarget = camTarget;
            currentVehicleTarget = vehicleToLookAt;

            //camTarget.tweenAnimation?.DOKill();
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

            if (countdown != null)
                StopCoroutine(countdown);

            countdown = cocoCountdown();

            StartCoroutine(countdown);
        }

        public void ForceChangeConstraint(CameraTarget[] camTargets, Vehicle vehicleToLookAt = null)
        {
            ForceChangeConstraint(camTargets[Random.Range(0, camTargets.Length)], vehicleToLookAt);
        }

        public void ForceChangeConstraint(CameraTarget camTarget, Vehicle vehicleToLookAt = null)
        {
            if (!canChange) return;

            lastChangeTime = Time.time;

            currentCameraTarget = camTarget;
            currentVehicleTarget = vehicleToLookAt;

            //camTarget.tweenAnimation?.DOKill();
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

            if (countdown != null)
                StopCoroutine(countdown);

            countdown = cocoCountdown();

            StartCoroutine(countdown);
        }

        private CameraTarget ct;
        public void EndRaceConstraint(CameraTarget[] camTargets)
        {
            ct = camTargets[Random.Range(0, camTargets.Length)];

            lastChangeTime = Time.time;

            currentCameraTarget = ct;

            //ct.tweenAnimation?.DOKill();
            ct.tweenAnimation?.DORestart();

            transformConstraint.target = currentCameraTarget.transformTarget;

            if (currentVehicleTarget == null)
            {
                lookAtConstraint.target = currentCameraTarget.lookAtTarget;
            }

            transform.SetParent(transformConstraint.target);

            if (countdown != null)
                StopCoroutine(countdown);
        }

        private IEnumerator countdown;
        private IEnumerator cocoCountdown()
        {
            yield return waitRandomChange;

            ChangeConstraintGlobalRace();
        }

        public void ChangeConstraintGlobalRace()
        {
            //TODO => Position camera qui voit globalement la course

        }
    }
}