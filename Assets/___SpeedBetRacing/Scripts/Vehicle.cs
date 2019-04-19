using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public TrailRenderer trail;

    public int ID;

    public float speed;
    public float baseSpeed;
    public int speedStep;

    private void Start()
    {
        meshRenderer.material = GameManager.instance.gameValue.vehiclesInfos[ID].material;

        baseSpeed = Random.Range(GameManager.instance.gameValue.minBaseSpeed, GameManager.instance.gameValue.maxBaseSpeed);
        speed = baseSpeed;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.gameIsEnded) return;

        transform.Translate(Vector3.right * speed * Time.deltaTime);
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
            col.gameObject.SetActive(false);
            GameManager.instance.UpdateVehiclesSpeed();
        }
    }
}