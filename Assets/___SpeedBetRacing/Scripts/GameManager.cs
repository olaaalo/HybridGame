using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoSingleton<GameManager>
{
    public GameValue gameValue;

    public KeyCode[] balanceKeyCodes;
    
    public Transform vehiclesParent;
    [HideInInspector] public List<Vehicle> vehicles;

    public Transform startTransform;
    public Transform endTransform;
    [HideInInspector] public float circuitLength;
    public List<Vehicle> vehiclesRank;
    public TextMeshProUGUI[] rankTexts;
    
    public RectTransform vehiclesProgressionsParent;
    public GameObject vehicleProgressionsPrefab;
    [HideInInspector] public List<Slider> vehiclesProgressions;

    public int[] vehiclesBetOnStep;
    public int[] vehiclesBetAll;

    public bool gameIsEnded;

    private BetZone[] betZones;

    private void Start()
    {
        SpawnVehiclesOnStart();
        UpdateVehicleRank();

        betZones = FindObjectsOfType<BetZone>();
    }

    private void SpawnVehiclesOnStart()
    {
        vehicles = new List<Vehicle>();
        vehiclesProgressions = new List<Slider>();

        // Spawn
        for (int i = 0; i < gameValue.vehiclesInfos.Count; ++i)
        {
            if (gameValue.vehiclesInfos[i].isActive)
            {
                vehicles.Add(Instantiate(gameValue.vehiclesInfos[i].prefab, startTransform.position, Quaternion.identity, vehiclesParent).GetComponent<Vehicle>());
                vehicles[vehicles.Count - 1].ID = i + 1;
                vehicles[vehicles.Count - 1].machineName = gameValue.vehiclesInfos[i].name;
                vehicles[vehicles.Count - 1].color = gameValue.vehiclesInfos[i].color;
                
                vehiclesProgressions.Add(Instantiate(vehicleProgressionsPrefab, vehiclesProgressionsParent).GetComponent<Slider>());
                vehiclesProgressions[vehiclesProgressions.Count - 1].targetGraphic.color = gameValue.vehiclesInfos[i].color;
            }
        }

        for (int i = 0; i < vehicles.Count; i++)
        {
            int randomIndex = Random.Range(i, vehicles.Count);

            Vehicle tempV = vehicles[i];
            vehicles[i] = vehicles[randomIndex];
            vehicles[randomIndex] = tempV;

            Slider tempS = vehiclesProgressions[i];
            vehiclesProgressions[i] = vehiclesProgressions[randomIndex];
            vehiclesProgressions[randomIndex] = tempS;
        }

        vehiclesRank = vehicles;

        // Get info & Start Position
        for (int i = 0; i < vehicles.Count; ++i)
        {
            vehicles[i].transform.position += Vector3.forward * i * 4f - Vector3.forward * (vehicles.Count - 1) * 4f / 2;
        }

        vehiclesBetOnStep = new int[vehicles.Count];
        vehiclesBetAll = new int[vehicles.Count];

        circuitLength = Vector3.Distance(startTransform.position, endTransform.position);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            gameIsEnded = false;

        if (gameIsEnded) return;

        // Update all vehicles
        for (int i = 0; i < vehicles.Count; ++i)
        {
            vehiclesProgressions[i].value = 1 - (endTransform.position.x - vehicles[i].transform.position.x) / circuitLength;

            if (Input.GetKeyDown(balanceKeyCodes[i]))
            {
                vehiclesBetOnStep[i]++;
                vehiclesBetAll[i]++;
            }
        }

        UpdateVehicleRank();
    }

    private void UpdateVehicleRank()
    {
        vehiclesRank = vehiclesRank.OrderBy(v => v.transform.position.x).ToList();

        for (int i = 0; i < rankTexts.Length; ++i)
        {
            rankTexts[i].text = string.Format("{0} <color=#{1}>|</color> {2}",
                i + 1,
                ColorUtility.ToHtmlStringRGB(vehiclesRank[vehiclesRank.Count - 1 - i].color),
                vehiclesRank[vehiclesRank.Count - 1 - i].machineName);
        }
    }


    public void CheckpointBet()
    {
        for (int i = 0; i < vehicles.Count; ++i)
        {
            for (int j = 0; j < vehiclesBetOnStep[i]; ++j)
            {
                vehicles[i].betOnThis++;
            }
                
            vehiclesBetOnStep[i] = 0;
        }
    }

    public void EndRace()
    {
        gameIsEnded = true;

        DOVirtual.DelayedCall(3f, () =>
        {
            for (int i = 0; i < betZones.Length; ++i)
            {
                betZones[i].gameObject.SetActive(true);
            }
            // for (int i = 0; i < vehicles.Length; ++i)
            // {
            //     vehicles[i].transform.position = vehicles[i].basePosition;
            //     vehicles[i].trail.Clear();
            // }
        });
    }
}