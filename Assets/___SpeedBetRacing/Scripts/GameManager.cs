using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    public GameValue gameValue;
    
    public Transform vehiclesParent;
    public List<Vehicle> vehicles;

    public Transform startTransform;
    public Transform endTransform;
    public float circuitLength;
    public List<int> vehiclesRank;
    
    public RectTransform vehiclesProgressionsParent;
    public GameObject vehicleProgressionsPrefab;
    public List<Slider> vehiclesProgressions;

    public int[] vehiclesBetOnStep;
    public int[] vehiclesBetAll;

    public KeyCode[] input_playerOne;
    public KeyCode[] input_playerTwo;
    public KeyCode[] input_playerThree;

    public bool gameIsEnded;

    private BetZone[] betZones;

    private void Start()
    {
        SpawnVehiclesOnStart();

        betZones = FindObjectsOfType<BetZone>();
    }

    private void SpawnVehiclesOnStart()
    {
        vehicles = new List<Vehicle>();
        vehiclesProgressions = new List<Slider>();
        vehiclesRank = new List<int>(gameValue.vehiclesInfos.Count);

        // Spawn
        for (int i = 0; i < gameValue.vehiclesInfos.Count; ++i)
        {
            if (gameValue.vehiclesInfos[i].isActive)
            {
                vehicles.Add(Instantiate(gameValue.vehiclesInfos[i].prefab, startTransform.position, Quaternion.identity, vehiclesParent).GetComponent<Vehicle>());
                vehicles[i].ID = i;
                
                vehiclesProgressions.Add(Instantiate(vehicleProgressionsPrefab, vehiclesProgressionsParent).GetComponent<Slider>());
                vehiclesProgressions[i].targetGraphic.color = gameValue.vehiclesInfos[i].color;
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

        // Start Position
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

        // Check Input Player 2
        for (int i = 0; i < gameValue.vehiclesInfos.Count; ++i)
        {
            vehiclesProgressions[i].value = 1 - (endTransform.position.x - vehicles[i].transform.position.x) / circuitLength;

            // if (Input.GetKeyDown(input_playerOne[i]))
            // {
            //     vehiclesBetOnStep[i]++;
            // }

            // if (Input.GetKeyDown(input_playerTwo[i]))
            // {
            //     vehiclesBetOnStep[i]++;
            // }

            // if (Input.GetKeyDown(input_playerThree[i]))
            // {
            //     vehiclesBetOnStep[i]++;
            // }
        }
    }

    public void UpdateVehiclesSpeed()
    {
        for (int i = 0; i < 5; ++i)
        {
            vehicles[i].speedStep++;
            // vehicules[i].speed = vehiclesInfos[i].baseSpeed + 10 * ((bet_playerOne[i] + bet_playerTwo[i] + bet_playerThree[i]) % 3);
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