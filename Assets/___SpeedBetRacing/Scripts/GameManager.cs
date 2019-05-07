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
    public CameraTarget startCameraTarget;

    public Transform startTransform;
    public Transform endTransform;
    public End end;
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
    public Image[] speedStepImages;
    public TextMeshProUGUI[] speedStepRankedTexts;

    public Image timeToBetImage;
    public RectTransform ratingRectTransform;
    public TextMeshProUGUI[] ratingVehicleTexts;
    public TextMeshProUGUI[] ratingVehicleBetTexts;

    public RectTransform vehiclesProgressionsParent;
    public GameObject vehicleProgressionsPrefab;
    [HideInInspector] public List<Slider> vehiclesProgressions;

    private void Start()
    {
        roadTransform.localScale = new Vector3(gameValue.circuitLength, 1, 1);

        endTransform.localPosition = Vector3.right * gameValue.circuitLength;

        if (gameValue.checkpointPositions.Length > 0)
        {
            for (int i = 0; i < gameValue.checkpointPositions.Length; ++i)
            {
                var checkpoint = Instantiate(betZonePrefab, circuitParent);
                checkpoint.transform.localPosition = Vector3.right * (gameValue.circuitLength * gameValue.checkpointPositions[i] / 100);
            }
        }

        sectorsCountText.text = string.Format("<b>{0} / {1}</b>   SECTORS", countRace + 1, gameValue.stepRacing);

        betZones = FindObjectsOfType<BetZone>();

        SpawnVehiclesOnStart();
        UpdateVehicleRank();

        rankRectTransform.localScale = new Vector3(1, 0, 1);
        ratingRectTransform.localScale = new Vector3(1, 0, 1);

        timeToBetImage.fillAmount = 0;

        if (!gameValue.withResetSpeed)
        {
            for (int i = 0; i < speedStepRankedTexts.Length; ++i)
                speedStepRankedTexts[i].gameObject.SetActive(false);
        }
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

        vehiclesRank = vehicles;

        // Get info & Start Position
        for (int i = 0; i < vehicles.Count; ++i)
        {
            vehicles[i].transform.localPosition += Vector3.forward * i * 6f - Vector3.forward * (vehicles.Count - 1) * 6f / 2;
        }

        vehiclesBetOnStep = new int[vehicles.Count];
        vehiclesBetAll = new int[vehicles.Count];

        StartRace();
    }

    private void StartRace()
    {
        DOVirtual.DelayedCall(3f, () =>
        {
            gameHasStarted = true;

            rankRectTransform.DOScale(1, 0.5f);

            for (int i = 0; i < vehicles.Count; ++i)
            {
                vehicles[i].DOStartRace();
            }
        });
    }

    private float timeCount;
    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Return) && !gameHasStarted)
        // {
        //     gameHasStarted = true;

        //     StartRace();
        // }

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

        // Update Bet values
        timeCount += Time.deltaTime;
        timeToBetImage.fillAmount = timeCount / gameValue.timeToApplyBet;
        if (timeCount >= gameValue.timeToApplyBet)
        {
            timeCount = 0;

            UpdateBetValue();
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

            if (countVehiclesArrived == 0)
            {
                cameraConstraint.ChangeConstraint(currentVehicleFirstRank.cameraTargets);
            }
        }

        // Affichage UI classement
        for (int i = 0; i < rankTexts.Length; ++i)
        {
            rankTexts[i].text = string.Format("{0} <color=#{1}>|</color> {2}",
                i + 1,
                ColorUtility.ToHtmlStringRGB(vehiclesRank[vehiclesRank.Count - 1 - i].color),
                vehiclesRank[vehiclesRank.Count - 1 - i].machineName);

            speedStepImages[i].color = vehiclesRank[vehiclesRank.Count - 1 - i].isOverheated ? Color.red : Color.white;

            if (gameValue.withResetSpeed)
            {
                speedStepRankedTexts[i].text = string.Format("{0} / {1}",
                    vehiclesRank[vehiclesRank.Count - 1 - i].speedStep,
                    gameValue.speedStepToOverride);
            }
        }
    }

    public void UpdateBetValue()
    {
        countCheckpoint++;

        for (int i = 0; i < vehicles.Count; ++i)
        {
            if (vehiclesBetOnStep[i] > 0)
            {
                vehicles[i].UpgradeSpeedStep(vehiclesBetOnStep[i], gameValue.speedStepToOverride + 1);
            }

            vehiclesBetOnStep[i] = 0;
        }
    }

    public int countVehiclesArrived;
    public void CheckEndRace()
    {
        countVehiclesArrived++;

        if (countVehiclesArrived == vehicles.Count)
            EndRace();
    }

    public int countRace;
    private void EndRace()
    {
        countRace++;

        gameHasStarted = false;

        timeToBetImage.fillAmount = 0;

        rankRectTransform.DOScaleY(0, 0.5f);
        ratingRectTransform.DOScale(1, 0.3f);

        for (int i = 0; i < ratingVehicleTexts.Length; ++i)
        {
            ratingVehicleTexts[i].text = string.Format("<color=#{0}>|</color> {1}",
                ColorUtility.ToHtmlStringRGB(vehicles[i].color),
                vehicles[i].machineName);

            ratingVehicleBetTexts[i].text = vehiclesBetAll[i].ToString();
        }

        if (countRace == gameValue.stepRacing)
        {
            sectorsCountText.text = string.Format("<b>{0} / {1}</b>   SECTORS", countRace + 1, gameValue.stepRacing);
            return;
        }

        DOVirtual.DelayedCall(5f, () =>
        {
            countVehiclesArrived = 0;

            cameraConstraint.ChangeConstraint(startCameraTarget);

            sectorsCountText.text = string.Format("<b>{0} / {1}</b>   SECTORS", countRace + 1, gameValue.stepRacing);

            ratingRectTransform.DOScaleY(0, 0.3f);

            for (int i = 0; i < vehicles.Count; ++i)
            {
                vehicles[i].transform.localPosition = new Vector3(0, vehicles[i].transform.localPosition.y, vehicles[i].transform.localPosition.z);
                vehicles[i].isArrived = false;
                vehicles[i].isStartRace = false;
            }

            end.wasPrepared = false;

            StartRace();
        });
    }
}