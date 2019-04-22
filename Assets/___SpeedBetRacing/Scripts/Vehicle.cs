using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public TrailRenderer trail;

    public int ID;
    public string machineName;
    public Color color;

    public float speed;
    public float baseSpeed;
    public int _betOnThis;
    public int betOnThis { get {return _betOnThis; } set {_betOnThis = value; UpgradeSpeedStep(); } }
    public int speedStep;

    private void Start()
    {
        meshRenderer.material = GameManager.instance.gameValue.vehiclesInfos[ID - 1].material;
        name = ID + " | " + machineName;

        baseSpeed = Random.Range(GameManager.instance.gameValue.minBaseSpeed, GameManager.instance.gameValue.maxBaseSpeed);
        speed = baseSpeed;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.gameIsEnded) return;

        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    public void UpgradeSpeedStep()
    {
        speedStep++;

        if (Random.value > GameManager.instance.gameValue.overrideChance * speedStep || speedStep - GameManager.instance.gameValue.speedStepToOverride < 1)
        {
            speed += baseSpeed * Random.Range(GameManager.instance.gameValue.minBetSpeed, GameManager.instance.gameValue.maxBetSpeed);
        }
        else
        {
            speedStep = 0;
            speed = baseSpeed;
        }
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