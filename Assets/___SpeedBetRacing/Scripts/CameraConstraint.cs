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

        public List<CameraTarget> cameraTargetsGlobalRace;
        public float timeToRandomChange;
        private WaitForSeconds waitRandomChange;

        public bool canChange;
        public int vehicleIDLooked;

        private void Start()
        {
            waitRandomChange = new WaitForSeconds(timeToRandomChange);

            cameraTargetsGlobalRace.Shuffle();
        }

        public void StartCameraRace()
        {
            cam.fieldOfView = 60;
            cam.farClipPlane = 1500;

            canChange = true;
            ChangeConstraint(currentCameraTarget);
            transform.SetParent(transformConstraint.target);
            canChange = false;
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

        private List<CameraTarget> cameraTargetsGlobalRaceTemp;
        private Vehicle firstPlaceVehicle;
        public void ChangeConstraintGlobalRace()
        {
            firstPlaceVehicle = GameManager.instance.currentVehicleFirstRank;

            cameraTargetsGlobalRace[0].transform.localPosition =
                new Vector3(firstPlaceVehicle.transform.localPosition.x, firstPlaceVehicle.transform.localPosition.y, 0);

            ChangeConstraint(cameraTargetsGlobalRace[0]);

            cameraTargetsGlobalRaceTemp = cameraTargetsGlobalRace;

            for (int i = 0; i < cameraTargetsGlobalRaceTemp.Count; ++i)
            {
                cameraTargetsGlobalRace[i] = cameraTargetsGlobalRaceTemp[(i + 1 < cameraTargetsGlobalRaceTemp.Count) ? i + 1 : 0];
            }
        }
    }
}