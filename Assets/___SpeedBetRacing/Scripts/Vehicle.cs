using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public int ID;
    public string machineName;
    public Color color;

    public MeshRenderer meshRenderer;
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

    private void Start()
    {
        meshRenderer.material = GameManager.instance.gameValue.vehiclesInfos[ID - 1].material;
        name = ID + " | " + machineName;

        baseSpeed = Random.Range(GameManager.instance.gameValue.minBaseSpeed, GameManager.instance.gameValue.maxBaseSpeed);

        baseAcceleration = GameManager.instance.gameValue.baseAcceleration;
        acceleration = baseAcceleration;

        AccelerationCoco = Acceleration();
        StartCoroutine(AccelerationCoco);
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

    private void FixedUpdate()
    {
        if (!GameManager.instance.gameHasStarted) return;

        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    public void UpgradeSpeedStep()
    {
        speedStep++;

        if (Random.value > GameManager.instance.gameValue.overrideChance * speedStep || speedStep - GameManager.instance.gameValue.speedStepToOverride < 1)
        {
            acceleration += baseAcceleration * Random.Range(GameManager.instance.gameValue.minBetSpeed, GameManager.instance.gameValue.maxBetSpeed);
        }
        else if (!isOverheated)
        {
            speedStep = 0;
            StopCoroutine(AccelerationCoco);
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

    private BetZone bZ;
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 9)
        {
            GameManager.instance.EndRace();
        }

        if (col.gameObject.layer == 11)
        {
            GameManager.instance.CheckpointBet();
            col.gameObject.SetActive(false);
        }
    }
}