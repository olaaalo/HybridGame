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

    public Transform startTransform;
    public List<Vehicle> vehicles;

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

        // Spawn
        for (int i = 0; i < gameValue.vehiclesInfos.Count; ++i)
        {
            if (gameValue.vehiclesInfos[i].isActive)
            {
                vehicles.Add(Instantiate(gameValue.vehiclesInfos[i].prefab, startTransform.position, Quaternion.identity, vehiclesParent).GetComponent<Vehicle>());
                vehicles[i].ID = i;
            }
        }

        // Start Position
        for (int i = 0; i < vehicles.Count; ++i)
        {
            //vehicles[i].transform.position += Vector3
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            gameIsEnded = false;

        if (gameIsEnded) return;

        // Check Input Player 2
        for (int i = 0; i < gameValue.vehiclesInfos.Count; ++i)
        {
            if (Input.GetKeyDown(input_playerOne[i]))
            {
                vehiclesBetOnStep[i]++;
            }

            if (Input.GetKeyDown(input_playerTwo[i]))
            {
                vehiclesBetOnStep[i]++;
            }

            if (Input.GetKeyDown(input_playerThree[i]))
            {
                vehiclesBetOnStep[i]++;
            }
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