﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public int ID;
    public string machineName;
    public Color color;

    public CameraTarget[] cameraTargets;

    public MeshRenderer[] meshRenderers;
    public TrailRenderer trail;

    public Transform engineTransform;
    public Engine engine;

    public GameObject engineDronePrefab;

    public float baseSpeed;
    public float speed;
    public float baseAcceleration;
    public float acceleration;
    public int _betOnThis;
    public int betOnThis { get { return _betOnThis; } set { _betOnThis = value; UpgradeSpeedStep(); } }
    public int speedStep;

    public bool isOverheated;
    public bool completeTrack;

    public float distanceRaycastForward;

    private void Start()
    {
        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].material = GameManager.instance.gameValue.vehiclesInfos[ID - 1].material;
        }

        name = ID + " | " + machineName;

        baseSpeed = Random.Range(GameManager.instance.gameValue.minBaseSpeed, GameManager.instance.gameValue.maxBaseSpeed);
        speed = baseSpeed;

        baseAcceleration = GameManager.instance.gameValue.baseAcceleration;
        acceleration = baseAcceleration;

        //AccelerationCoco = Acceleration();
        //StartCoroutine(AccelerationCoco);
    }

    IEnumerator AccelerationCoco;
    float startTime;
    float t;
    IEnumerator Acceleration()
    {
        while (!GameManager.instance.gameHasStarted)
            yield return null;

        isOverheated = false;

        startTime = Time.time;
        t = 0;
        while (Time.time - startTime < 3)
        {
            t += Time.deltaTime / 3;
            speed = Mathf.Lerp(0, baseSpeed, t);
            yield return null;
        }
        speed = baseSpeed;

        while (!completeTrack)
        {
            speed += acceleration;

            yield return new WaitForSeconds(0.1f);
        }
    }

    private RaycastHit hitForward;
    private BetZone betZoneForward;
    private End endForward;
    private bool isArrived;
    private void FixedUpdate()
    {
        if (!GameManager.instance.gameHasStarted || isArrived) return;

        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (Physics.Raycast(transform.position, Vector3.right, out hitForward, distanceRaycastForward, 1 << 11))
        {
            betZoneForward = hitForward.transform.GetComponent<BetZone>();

            if (!betZoneForward.wasPrepared)
            {
                betZoneForward.wasPrepared = true;

                GameManager.instance.cameraConstraint.ChangeConstraint(betZoneForward.cameraTargets);
            }
        }
        else if (Physics.Raycast(transform.position, Vector3.right, out hitForward, distanceRaycastForward, 1 << 9))
        {
            endForward = hitForward.transform.GetComponent<End>();

            if (!endForward.wasPrepared)
            {
                betZoneForward.wasPrepared = true;

                GameManager.instance.cameraConstraint.ChangeConstraint(endForward.cameraTargets);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, Vector3.right * distanceRaycastForward);
    }
#endif

    public void UpgradeSpeedStep()
    {
        speedStep++;

        if (Random.value > GameManager.instance.gameValue.overrideChance * speedStep || speedStep - GameManager.instance.gameValue.speedStepToOverride < 1)
        {
            speed += Random.Range(GameManager.instance.gameValue.minBetSpeed, GameManager.instance.gameValue.maxBetSpeed);
            //acceleration += baseAcceleration * Random.Range(GameManager.instance.gameValue.minBetSpeed, GameManager.instance.gameValue.maxBetSpeed);
        }
        else if (!isOverheated)
        {
            speedStep = 0;
            //StopCoroutine(AccelerationCoco);
            StartCoroutine(Overheated());
        }
    }

    float speedBeforeOverheated;
    EngineDrone drone;
    IEnumerator Overheated()
    {
        isOverheated = true;

        trail.emitting = false;
        engine.DetachFromVehicle();
        engine = null;

        speedBeforeOverheated = speed;
        acceleration = 0;

        startTime = Time.time;
        t = 0;

        while (Time.time - startTime < 3)
        {
            t += Time.deltaTime / 3;
            speed = Mathf.Lerp(speedBeforeOverheated, 0, t);

            yield return null;
        }

        speed = 0;

        drone = Instantiate(engineDronePrefab).GetComponent<EngineDrone>();
        drone.vehicleTarget = this;
    }

    public void Repared()
    {
        engine.transform.SetParent(engineTransform);

        AccelerationCoco = Acceleration();
        StartCoroutine(AccelerationCoco);
    }

    private BetZone betZone;
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 9)
        {
            //GameManager.instance.EndRace();
            isArrived = true;
        }

        if (col.gameObject.layer == 11)
        {
            betZone = col.GetComponent<BetZone>();

            if (!betZone.wasActivated)
            {
                betZone.wasActivated = true;
                GameManager.instance.CheckpointBet();
            }
        }
    }
}