using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Vehicle : MonoBehaviour
{
    public int ID;
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
    public float baseAcceleration;
    public float acceleration;
    public int _betOnThis;
    public int betOnThis { get { return _betOnThis; } set { _betOnThis = value; UpgradeSpeedStep(); } }
    public int speedStep;

    public bool isOverheated;
    public bool completeTrack;

    public float distanceRaycastForward;

    public FMODUnity.StudioEventEmitter startEngineEventEmitter;
    public FMODUnity.StudioEventEmitter engineEventEmitter;
    public FMODUnity.StudioEventEmitter engineUpStepEventEmitter;

    private void Start()
    {
        for (int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].material = GameManager.instance.gameValue.vehiclesInfos[ID - 1].material;
        }

        name = ID + " | " + machineName;

        baseSpeed = Random.Range(GameManager.instance.gameValue.minBaseSpeed, GameManager.instance.gameValue.maxBaseSpeed);

        baseAcceleration = GameManager.instance.gameValue.baseAcceleration;
        acceleration = baseAcceleration;

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

    public void DOStartRace()
    {
        DOVirtual.DelayedCall(Random.Range(0.1f, 0.4f), () =>
        {
            isStartRace = true;
            
            AccelerationCoco = Acceleration();
            StartCoroutine(AccelerationCoco);

            engineUpStepEventEmitter.Play();
        });
    }


    IEnumerator AccelerationCoco;
    float startTime;
    float t;
    IEnumerator Acceleration()
    {
        isOverheated = false;

        startTime = Time.time;
        t = 0;
        while (Time.time - startTime < 1)
        {
            t += Time.deltaTime;
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
    private bool isStartRace;
    private bool isArrived;
    private void FixedUpdate()
    {
        if (!isStartRace || isArrived) return;

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
                betZoneForward.wasPrepared = true;

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

    public void UpgradeSpeedStep()
    {
        speedStep++;

        if (Random.value > GameManager.instance.gameValue.overrideChance * speedStep || speedStep - GameManager.instance.gameValue.speedStepToOverride < 1)
        {
            engineUpStepEventEmitter.Play();

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
        PoolManager.instance.GetExplosionParticle(engine.transform.position, null);

        isOverheated = true;

        trail.emitting = false;
        engine.DetachFromVehicle();
        engine = null;

        speedBeforeOverheated = speed;
        acceleration = 0;

        startTime = Time.time;
        t = 0;

        while (Time.time - startTime < 1)
        {
            t += Time.deltaTime / 1;
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
                betZone.Activate();
                GameManager.instance.CheckpointBet();
            }
        }
    }
}