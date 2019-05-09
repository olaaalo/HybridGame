﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public int ID;
    public int inGameID;
    public string machineName;
    public Color color;

    public CameraTarget[] cameraTargets;

    public MeshRenderer[] meshRenderers;
    public TrailRenderer trail;

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

    public FMODUnity.StudioEventEmitter startEngineEventEmitter;
    public FMODUnity.StudioEventEmitter engineEventEmitter;
    public FMODUnity.StudioEventEmitter engineUpStepEventEmitter;

    private void Start()
    {

#if UNITY_EDITOR
        name = ID + " | " + machineName;
#endif

        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].material = GameManager.instance.gameValue.vehiclesInfos[ID - 1].material;
        }

        baseSpeed = GameManager.instance.gameValue.baseSpeed;

        StartCoroutine(StartAnimation());
    }

    private IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds(Random.Range(0.3f, 0.8f));

        startEngineEventEmitter.Play();

        for (int i = 0; i < rowsParticles.Length; ++i)
        {
            rowsParticles[i].Play();
        }

        transform.DOLocalMoveY(1f, 3f).SetEase(Ease.OutBack);

        transform.DOLocalRotate(Vector3.right * -5f, 0.3f);
        transform.DOLocalRotate(Vector3.right * 7f, 0.5f).SetDelay(0.3f);
        transform.DOLocalRotate(Vector3.right * -3f, 0.3f).SetDelay(0.8f);
        transform.DOLocalRotate(Vector3.zero, 0.3f).SetDelay(1.2f)
            .OnStart(() => engineEventEmitter.Play());
    }

    private float timeToWait;
    public void DOStartRace()
    {
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

    private RaycastHit hitForward;
    private BetZone betZoneForward;
    private End endForward;
    public bool isStartRace;
    private void FixedUpdate()
    {
        if (!GameManager.instance.gameHasStarted || !isStartRace || isArrived || isOverheated) return;

        transform.Translate(Vector3.right * speed * Time.deltaTime, transform);

        if (Physics.Raycast(transform.position, transform.right, out hitForward, distanceRaycastForward, 1 << 11))
        {
            betZoneForward = hitForward.transform.GetComponent<BetZone>();

            if (!betZoneForward.wasPrepared)
            {
                betZoneForward.wasPrepared = true;

                GameManager.instance.cameraConstraint.ChangeConstraint(betZoneForward.cameraTargets);
            }
        }
        else if (Physics.Raycast(transform.position, transform.right, out hitForward, distanceRaycastForward, 1 << 9))
        {
            endForward = hitForward.transform.GetComponent<End>();

            if (!endForward.wasPrepared)
            {
                endForward.wasPrepared = true;

                GameManager.instance.cameraConstraint.ChangeConstraint(endForward.cameraTargets);
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
    public void UpgradeSpeedStep(int step, int stepToOverride)
    {
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
                    engineUpStepEventEmitter.Play();
            }

            speedStep += step;
            speedStep = speedStep % stepToOverride;

            speed = GameManager.instance.gameValue.baseSpeed + GameManager.instance.gameValue.addBetSpeed * (speedStep % stepToOverride);
            saveSpeed = GameManager.instance.gameValue.baseSpeed + GameManager.instance.gameValue.addBetSpeed * (speedStep % stepToOverride);
        }
    }

    EngineDrone drone;
    IEnumerator Overheated(float sp)
    {
        PoolManager.instance.GetExplosionParticle(engine.transform.position, null);
        GameManager.instance.commentator.ExplosionVehicle(machineName);

        isOverheated = true;

        trail.emitting = false;
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
        if (col.gameObject.layer == 9)
        {
            isArrived = true;

            GameManager.instance.CheckEndRace(inGameID, machineName);

            raceRank = GameManager.instance.countVehiclesArrived;
            
            transform.DOLocalMoveX(GameManager.instance.endTransform.localPosition.x + 5f + 0.6f / raceRank, 0.5f);
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