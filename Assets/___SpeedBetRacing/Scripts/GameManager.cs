using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    public KeyCode[] balanceKeyCodes;

    public GameValue gameValue;

    [HideInInspector] public bool gameHasStarted;

    public CameraConstraint cameraConstraint;
    
    public Transform startTransform;
    public Transform endTransform;
    public Transform circuitParent;
    public Transform roadTransform;
    public GameObject betZonePrefab;
    private BetZone[] betZones;

    public Transform vehiclesParent;
    [HideInInspector] public List<Vehicle> vehicles;
    public int[] vehiclesBetOnStep;
    public int[] vehiclesBetAll;

    public int countCheckpoint;
    public RectTransform sectorRectTransform;
    public TextMeshProUGUI sectorsCountText;

    public List<Vehicle> vehiclesRank;
    public RectTransform rankRectTransform;
    public TextMeshProUGUI[] rankTexts;

    public RectTransform vehiclesProgressionsParent;
    public GameObject vehicleProgressionsPrefab;
    [HideInInspector] public List<Slider> vehiclesProgressions;


    private void Start()
    {
        roadTransform.localScale = new Vector3(gameValue.circuitLength, 1, 1);

        endTransform.localPosition = Vector3.right * gameValue.circuitLength;

        for (int i = 0; i < gameValue.checkpointPositions.Length; ++i)
        {
            var checkpoint = Instantiate(betZonePrefab, circuitParent);
            checkpoint.transform.localPosition = Vector3.right * (gameValue.circuitLength * gameValue.checkpointPositions[i] / 100);
        }

        betZones = FindObjectsOfType<BetZone>();

        SpawnVehiclesOnStart();
        UpdateVehicleRank();
        
        sectorsCountText.text = string.Format("<b>{0} / {1}</b>   SECTORS", countCheckpoint, gameValue.checkpointPositions.Length);

        rankRectTransform.localScale = new Vector3(1, 0, 1);
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
                vehicles.Add(Instantiate(gameValue.vehiclesInfos[i].prefab, vehiclesParent).GetComponent<Vehicle>());
                vehicles[vehicles.Count - 1].ID = i + 1;
                vehicles[vehicles.Count - 1].machineName = gameValue.vehiclesInfos[i].name;
                vehicles[vehicles.Count - 1].color = gameValue.vehiclesInfos[i].color;

                vehiclesProgressions.Add(Instantiate(vehicleProgressionsPrefab, vehiclesProgressionsParent).GetComponent<Slider>());
                vehiclesProgressions[vehiclesProgressions.Count - 1].targetGraphic.color = gameValue.vehiclesInfos[i].color;
            }
        }

        // for (int i = 0; i < vehicles.Count; i++)
        // {
        //     int randomIndex = Random.Range(i, vehicles.Count);

        //     Vehicle tempV = vehicles[i];
        //     vehicles[i] = vehicles[randomIndex];
        //     vehicles[randomIndex] = tempV;

        //     Slider tempS = vehiclesProgressions[i];
        //     vehiclesProgressions[i] = vehiclesProgressions[randomIndex];
        //     vehiclesProgressions[randomIndex] = tempS;
        // }

        vehiclesRank = vehicles;

        // Get info & Start Position
        for (int i = 0; i < vehicles.Count; ++i)
        {
            vehicles[i].transform.localPosition += Vector3.forward * i * 6f - Vector3.forward * (vehicles.Count - 1) * 6f / 2;
        }

        vehiclesBetOnStep = new int[vehicles.Count];
        vehiclesBetAll = new int[vehicles.Count];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !gameHasStarted)
        {
            gameHasStarted = true;

            for (int i = 0; i < vehicles.Count; ++i)
            {
                vehicles[i].DOStartRace();
            }
        }

        if (!gameHasStarted) return;

        // Update all vehicles
        for (int i = 0; i < vehicles.Count; ++i)
        {
            vehiclesProgressions[i].value = 1 - (gameValue.circuitLength - vehicles[i].transform.localPosition.x) / gameValue.circuitLength;

            if (Input.GetKeyDown(balanceKeyCodes[i]))
            {
                vehiclesBetOnStep[i]++;
                vehiclesBetAll[i]++;
            }
        }

        UpdateVehicleRank();
    }

    private Vehicle currentVehicleFirstRank;
    private void UpdateVehicleRank()
    {
        vehiclesRank = vehiclesRank.OrderBy(v => v.transform.localPosition.x).ToList();

        // Camera sur véhicle première classe
        if (currentVehicleFirstRank != vehiclesRank[vehiclesRank.Count - 1])
        {
            currentVehicleFirstRank = vehiclesRank[vehiclesRank.Count - 1];

            cameraConstraint.ChangeConstraint(currentVehicleFirstRank.cameraTargets);
        }

        
        // Affichage UI classement
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
        countCheckpoint++;

        sectorsCountText.text = string.Format("<b>{0} / {1}</b>   SECTORS", countCheckpoint, gameValue.checkpointPositions.Length);

        if (countCheckpoint == 1)
        {
            rankRectTransform.DOScale(1, 0.5f);
        }

        for (int i = 0; i < vehicles.Count; ++i)
        {
            if (!vehicles[i].isOverheated)
            {
                for (int j = 0; j < vehiclesBetOnStep[i]; ++j)
                {
                    vehicles[i].betOnThis++;
                }

                vehiclesBetOnStep[i] = 0;
            }
        }
    }

    // public void EndRace()
    // {
    //     gameHasStarted = false;

    //     DOVirtual.DelayedCall(3f, () =>
    //     {
    //         for (int i = 0; i < betZones.Length; ++i)
    //         {
    //             betZones[i].gameObject.SetActive(true);
    //         }
    //         // for (int i = 0; i < vehicles.Length; ++i)
    //         // {
    //         //     vehicles[i].transform.position = vehicles[i].basePosition;
    //         //     vehicles[i].trail.Clear();
    //         // }
    //     });
    // }
}