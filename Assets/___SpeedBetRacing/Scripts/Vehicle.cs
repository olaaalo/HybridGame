using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LibLabSystem;
using UnityEngine;

namespace LibLabGames.SpeedBetRacing
{
    public class Vehicle : MonoBehaviour
    {
        public int ID;
        public int inGameID;
        public string machineName;
        public Color color;

        public Rigidbody rb;
        public GameObject bottomColliderObject;

        public CameraTarget[] cameraTargets;

        public ParticleSystem[] rowsParticles;

        public Transform engineTransform;
        public Engine engine;

        public GameObject engineDronePrefab;

        public float baseSpeed;
        public float speed;
        public int speedStep;

        public bool isOverheated;

        public float distanceRaycastForward;

        public bool isArrived;
        public int raceRank;

        public ParticleSystem dustParticle;
        public ParticleSystem engineSpeedUpParticle;
        public ParticleSystem engineBigFireParticle;
        public ParticleSystem[] engineSmallFireParticles;

        private float baseSpeedBigFireParticle;
        private float baseSpeedSmallFireParticle;

        public FMODUnity.StudioEventEmitter startEngineEventEmitter;
        public FMODUnity.StudioEventEmitter engineEventEmitter;
        public FMODUnity.StudioEventEmitter engineUpStepEventEmitter;

        private void Start()
        {

#if UNITY_EDITOR
            name = ID + " | " + machineName;
#endif

            baseSpeed = GameManager.instance.gameValue.baseSpeed;

            baseSpeedBigFireParticle = engineBigFireParticle.main.startSpeed.constant;
            baseSpeedSmallFireParticle = engineSmallFireParticles[0].main.startSpeed.constant;
        }

        public IEnumerator StartAnimation()
        {
            PlaceOnBottom();

            rb.isKinematic = true;
            bottomColliderObject.SetActive(false);

            yield return null;

            yield return new WaitForSeconds(Random.Range(0.3f, 0.8f));

            startEngineEventEmitter.Play();

            for (int i = 0; i < rowsParticles.Length; ++i)
            {
                rowsParticles[i].Play();
            }

            transform.DOLocalMoveY(0.6f, 3f).SetRelative().SetEase(Ease.OutBack)
                .OnComplete(() => bottomColliderObject.SetActive(true));

            transform.DOLocalRotate(Vector3.right * -5f, 0.3f);
            transform.DOLocalRotate(Vector3.right * 7f, 0.5f).SetDelay(0.3f);
            transform.DOLocalRotate(Vector3.right * -3f, 0.3f).SetDelay(0.8f);
            transform.DOLocalRotate(Vector3.zero, 0.3f).SetDelay(1.2f)
                .OnStart(() => engineEventEmitter.Play());
        }

        public void PlaceOnBottom()
        {
            if (Physics.Raycast(transform.position, transform.up * -1, out hit, Mathf.Infinity, 1 << 13))
            {
                transform.position = hit.point + Vector3.up * 0.5f;
            }
        }

        private float timeToWait;
        public void DOStartRace()
        {
            rb.isKinematic = false;

            if (raceRank == 0)
                timeToWait = Random.Range(0.1f, 0.3f);
            else
                timeToWait = 0.1f + (raceRank - 1) * 0.6f;

            DOVirtual.DelayedCall(timeToWait, () =>
            {
                isStartRace = true;

                AccelerationCoco = Acceleration(raceRank == 0 ? baseSpeed : speed);
                StartCoroutine(AccelerationCoco);

                engineUpStepEventEmitter.Play();
            });
        }

        IEnumerator AccelerationCoco;
        float startTime;
        float t;
        IEnumerator Acceleration(float speedToGo)
        {
            engineSpeedUpParticle.Play();
            engineBigFireParticle.Play();
            dustParticle.Play();

            isOverheated = false;

            startTime = Time.time;
            t = 0;
            while (Time.time - startTime < 1)
            {
                t += Time.deltaTime;
                speed = Mathf.Lerp(0, speedToGo, t);
                yield return null;
            }

            speed = speedToGo;
        }

        private RaycastHit hit;
        private BetZone betZoneForward;
        private End end;
        public bool isStartRace;
        private void FixedUpdate()
        {
            if (!GameManager.instance.gameHasStarted || !isStartRace || isArrived || isOverheated) return;

            transform.Translate(Vector3.right * speed * Time.deltaTime, transform);

            if (Physics.Raycast(transform.position, transform.right, out hit, distanceRaycastForward, 1 << 11))
            {
                betZoneForward = hit.transform.GetComponent<BetZone>();

                if (!betZoneForward.wasPrepared)
                {
                    betZoneForward.wasPrepared = true;

                    GameManager.instance.cameraConstraint.ChangeConstraint(betZoneForward.cameraTargets);
                }
            }
            else if (Physics.Raycast(transform.position, transform.right, out hit, distanceRaycastForward, 1 << 9))
            {
                end = hit.transform.GetComponent<End>();

                if (!end.wasPrepared)
                {
                    end.wasPrepared = true;

                    GameManager.instance.cameraConstraint.ChangeConstraint(end.cameraTargets);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, transform.right * distanceRaycastForward);
        }
#endif

        private float saveSpeed;
        private ParticleSystem.ForceOverLifetimeModule mainA;
        private ParticleSystem.ForceOverLifetimeModule mainB_0;
        private ParticleSystem.ForceOverLifetimeModule mainB_1;
        public void UpgradeSpeedStep(int step, int stepToOverride)
        {
            engineSpeedUpParticle.Play();

            if (!GameManager.instance.gameValue.withResetSpeed)
            {
                if (!isOverheated)
                {
                    if (Random.value < GameManager.instance.gameValue.overrideChance * (speedStep + step))
                        StartCoroutine(Overheated(speed));
                    else
                        engineUpStepEventEmitter.Play();
                }

                speedStep += step;

                speed = GameManager.instance.gameValue.baseSpeed + GameManager.instance.gameValue.addBetSpeed * speedStep;
                saveSpeed = GameManager.instance.gameValue.baseSpeed + GameManager.instance.gameValue.addBetSpeed * speedStep;
            }
            else
            {
                if (!isOverheated)
                {
                    if (speedStep + step >= stepToOverride)
                        StartCoroutine(Overheated(speed));
                    else
                    {
                        engineUpStepEventEmitter.Play();
                    }
                }

                speedStep += step;
                speedStep = speedStep % stepToOverride;

                mainA = engineBigFireParticle.forceOverLifetime;
                mainA.z = speedStep * 10;
                mainB_0 = engineSmallFireParticles[0].forceOverLifetime;
                mainB_0.z = speedStep * 10;
                mainB_1 = engineSmallFireParticles[1].forceOverLifetime;
                mainB_1.z = speedStep * 10;

                speed = GameManager.instance.gameValue.baseSpeed + GameManager.instance.gameValue.addBetSpeed * (speedStep % stepToOverride);
                saveSpeed = GameManager.instance.gameValue.baseSpeed + GameManager.instance.gameValue.addBetSpeed * (speedStep % stepToOverride);
            }
        }

        EngineDrone drone;
        IEnumerator Overheated(float sp)
        {
            engineBigFireParticle.Stop();
            dustParticle.Stop();

            LLPoolManager.instance.GetExplosionParticle(engine.transform.position, null);
            GameManager.instance.commentator.ExplosionVehicle(machineName);

            isOverheated = true;

            engine.DetachFromVehicle();
            engine = null;

            startTime = Time.time;
            t = 0;

            while (Time.time - startTime < 1)
            {
                t += Time.deltaTime / 1;
                speed = Mathf.Lerp(sp, 0, t);

                yield return null;
            }

            speed = 0;

            drone = Instantiate(engineDronePrefab).GetComponent<EngineDrone>();
            drone.vehicleTarget = this;
        }

        public void Repared()
        {
            engine.transform.SetParent(engineTransform);

            AccelerationCoco = Acceleration(saveSpeed);
            StartCoroutine(AccelerationCoco);
        }

        private BetZone betZone;
        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == 9 && !isArrived)
            {
                isArrived = true;

                GameManager.instance.CheckEndRace(inGameID, machineName);

                raceRank = GameManager.instance.countVehiclesArrived;

                transform.DOLocalMoveX(GameManager.instance.endTransform.localPosition.x + 5f + 0.6f / raceRank, 0.5f);

                engineBigFireParticle.Stop();
                dustParticle.Stop();
            }

            if (col.gameObject.layer == 11)
            {
                betZone = col.GetComponent<BetZone>();

                if (!betZone.wasActivated)
                {
                    betZone.wasActivated = true;
                    betZone.Activate();
                    GameManager.instance.UpdateBetValue();
                }
            }
        }
    }
}