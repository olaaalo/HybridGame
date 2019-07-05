using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening;
using LibLabSystem;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LibLabGames.SpeedBetRacing
{
    public class GameManager : IGameManager
    {
        public static GameManager instance;

        public GameValue gameValue;
        public CommentatorObject commentatorObject;

        public Commentator commentator;

        public CameraConstraint cameraConstraint;
        public CameraTarget startCameraTarget;

        public Transform startTransform;
        public StartRace start;
        public Transform endTransform;
        public End end;
        public Transform circuitParent;
        public Transform roadTransform;

        public Transform vehiclesParent;
        [HideInInspector] public List<Vehicle> vehicles;
        [HideInInspector] public List<Vehicle> vehiclesRankList;
        [HideInInspector] public int[] vehiclesBetOnStep;
        [HideInInspector] public int[] vehiclesBetAll;
        [HideInInspector] public List<int[]> vehiclesRanks;

        public List<Transform> zoneSpeedDecreases;
        public List<Transform> zoneSpeedIncreases;

        public CanvasGroup fadeScreen;

        public RectTransform sectorRectTransform;
        public TextMeshProUGUI sectorsCountText;

        public RectTransform rankRectTransform;
        public TextMeshProUGUI[] rankTexts;
        public Image[] speedStepImages;
        public TextMeshProUGUI[] speedStepRankedTexts;

        public Image timeToBetImage;
        public RectTransform ratingRectTransform;

        [Serializable]
        public struct RatingVehicleInfo
        {
            public TextMeshProUGUI ratingVehicleName;
            public TextMeshProUGUI ratingVehicleBet;
            public TextMeshProUGUI[] ratingVehicleRank;
        }
        public List<RatingVehicleInfo> ratingVehicleInfos;

        public RectTransform vehiclesProgressionsParent;
        public GameObject vehicleProgressionsPrefab;
        public GameObject zoneIncreaseSliderPrefab;
        public GameObject zoneDecreaseSliderPrefab;
        [HideInInspector] public List<Slider> vehiclesProgressions;

        private void Awake()
        {
            if (!DOAwake()) return;

            instance = this;
        }

        private IEnumerator Start()
        {
            //base.DOStart();

            fadeScreen.alpha = 1;

            yield return new WaitForSeconds(0.5f);
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;

            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Decrease").Length; ++i)
                GameObject.FindGameObjectsWithTag("Decrease") [i].transform.SetParent(roadTransform);
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Increase").Length; ++i)
                GameObject.FindGameObjectsWithTag("Increase") [i].transform.SetParent(roadTransform);
            positionSliderZones = new List<Slider>();

            Inputs.instance.ActiveSerialPort();

            fadeScreen.DOFade(0, 3f);

            //roadTransform.localScale = new Vector3(gameValue.circuitLength, 1, 1);

            start.PlaceOnBottom();

            endTransform.localPosition = Vector3.right * gameValue.circuitLength;
            end.PlaceOnBottom();

            sectorsCountText.text = string.Format("<b>{0} / {1}</b>   SECTORS", countRace + 1, gameValue.stepRacing);

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
            vehiclesRanks = new List<int[]>();

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

            vehiclesRankList = vehicles;

            for (int i = 0; i < vehicles.Count; ++i)
            {
                // Vehicles position & info
                vehicles[i].transform.localPosition += Vector3.forward * i * 6f - Vector3.forward * (vehicles.Count - 1) * 6f / 2;
                vehicles[i].PlaceOnBottom();
                vehicles[i].inGameID = i;

                // Rank panel preparation
                vehiclesRanks.Add(new int[vehicles.Count]);

                ratingVehicleInfos[i].ratingVehicleName.text = string.Format("<color=#{0}>|</color> {1}",
                    ColorUtility.ToHtmlStringRGB(vehicles[i].color),
                    vehicles[i].machineName);

                ratingVehicleInfos[i].ratingVehicleBet.text = "0";

                for (int j = 0; j < gameValue.stepRacing; ++j)
                {
                    ratingVehicleInfos[i].ratingVehicleRank[j].text = string.Empty;
                }
            }

            vehiclesBetOnStep = new int[vehicles.Count];
            vehiclesBetAll = new int[vehicles.Count];

            StartCoroutine(StartGameCoco());
        }

        IEnumerator StartGameCoco()
        {
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;

            commentator.PlayQuote(commentatorObject.commentatorQuotes[0].startGame);

            yield return null;

            StartRace();
        }

        private Transform tempZone;
        private List<Slider> positionSliderZones;
        private void StartRace()
        {
            if (positionSliderZones.Count > 0)
            {
                for (int i = 0; i < positionSliderZones.Count; ++i)
                    Destroy(positionSliderZones[i].gameObject);
            }
            positionSliderZones = new List<Slider>();

            zoneSpeedDecreases = new List<Transform>();
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Decrease").Length; ++i)
            {
                tempZone = GameObject.FindGameObjectsWithTag("Decrease") [i].transform;
                if (tempZone.localPosition.x < gameValue.circuitLength * (countRace + 1) && tempZone.localPosition.x > gameValue.circuitLength * countRace)
                {
                    positionSliderZones.Add(Instantiate(zoneDecreaseSliderPrefab, vehiclesProgressionsParent).GetComponent<Slider>());
                    positionSliderZones[positionSliderZones.Count - 1].value = 1 - (gameValue.circuitLength - tempZone.localPosition.x + gameValue.circuitLength * countRace) / gameValue.circuitLength;

                    zoneSpeedDecreases.Add(tempZone);
                }
            }

            zoneSpeedIncreases = new List<Transform>();
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Increase").Length; ++i)
            {
                tempZone = GameObject.FindGameObjectsWithTag("Increase") [i].transform;
                if (tempZone.localPosition.x < gameValue.circuitLength * (countRace + 1) && tempZone.localPosition.x > gameValue.circuitLength * countRace)
                {
                    positionSliderZones.Add(Instantiate(zoneIncreaseSliderPrefab, vehiclesProgressionsParent).GetComponent<Slider>());
                    positionSliderZones[positionSliderZones.Count - 1].value = 1 - (gameValue.circuitLength - tempZone.localPosition.x + gameValue.circuitLength * countRace) / gameValue.circuitLength;

                    zoneSpeedIncreases.Add(tempZone);
                }
            }

            commentator.PlayQuote(commentatorObject.commentatorQuotes[0].startRace);

            for (int i = 0; i < vehicles.Count; ++i)
            {
                vehicles[i].StartCoroutine(vehicles[i].StartAnimation());
            }

            DOVirtual.DelayedCall(4f, () =>
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
            if (!gameHasStarted) return;

            // Update all vehicles
            for (int i = 0; i < vehicles.Count; ++i)
            {
                vehiclesProgressions[i].value = 1 - (gameValue.circuitLength - vehicles[i].transform.localPosition.x + gameValue.circuitLength * countRace) / gameValue.circuitLength;

                if (Input.GetKeyDown(gameValue.betKeyCodes[i]))
                {
                    BetOnVehicle(i, 1);
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

        public void BetOnVehicle(int indexVehicle, int betValue)
        {
            vehiclesBetOnStep[indexVehicle] += betValue;
            vehiclesBetAll[indexVehicle] += betValue;
        }

        private Vehicle currentVehicleFirstRank;
        private void UpdateVehicleRank()
        {
            vehiclesRankList = vehiclesRankList.OrderBy(v => v.transform.localPosition.x).ToList();

            // Camera sur véhicle première classe
            if (currentVehicleFirstRank != vehiclesRankList[vehiclesRankList.Count - 1])
            {
                currentVehicleFirstRank = vehiclesRankList[vehiclesRankList.Count - 1];

                if (countVehiclesArrived == 0 && gameHasStarted)
                {
                    cameraConstraint.ChangeConstraint(currentVehicleFirstRank.cameraTargets, null);
                    commentator.FirstPlaceVehicle(currentVehicleFirstRank.machineName);
                }
            }

            // Affichage UI classement
            for (int i = 0; i < rankTexts.Length; ++i)
            {
                rankTexts[i].text = string.Format("{0} <color=#{1}>|</color> {2}",
                    i + 1,
                    ColorUtility.ToHtmlStringRGB(vehiclesRankList[vehiclesRankList.Count - 1 - i].color),
                    vehiclesRankList[vehiclesRankList.Count - 1 - i].machineName);

                speedStepImages[i].color = vehiclesRankList[vehiclesRankList.Count - 1 - i].isOverheated ? Color.red : Color.white;

                if (gameValue.withResetSpeed)
                {
                    speedStepRankedTexts[i].text = string.Format("{0} / {1}",
                        vehiclesRankList[vehiclesRankList.Count - 1 - i].speedStep,
                        gameValue.speedStepToOverride);
                }
            }
        }

        public void UpdateBetValue()
        {
            for (int i = 0; i < vehicles.Count; ++i)
            {
                if (vehiclesBetOnStep[i] > 0)
                {
                    vehicles[i].UpgradeSpeedStep(vehiclesBetOnStep[i]);
                }

                vehiclesBetOnStep[i] = 0;
            }
        }

        public int countVehiclesArrived;
        public void CheckEndRace(int vehicleID, string machineName)
        {
            countVehiclesArrived++;

            vehiclesRanks[countRace][vehicleID] = countVehiclesArrived;

            if (countVehiclesArrived == vehicles.Count)
                EndRace();
        }

        public int countRace;
        private void EndRace()
        {
            gameHasStarted = false;

            timeToBetImage.fillAmount = 0;

            rankRectTransform.DOScaleY(0, 0.5f);
            ratingRectTransform.DOScale(1, 0.3f);

            // Rank Panel info
            for (int i = 0; i < ratingVehicleInfos.Count; ++i)
            {
                ratingVehicleInfos[i].ratingVehicleBet.text = vehiclesBetAll[i].ToString();

                ratingVehicleInfos[i].ratingVehicleRank[countRace].text = vehiclesRanks[countRace][i].ToString();
                ratingVehicleInfos[i].ratingVehicleRank[countRace].fontStyle = (vehiclesRanks[countRace][i] == 1) ? TMPro.FontStyles.Bold : TMPro.FontStyles.Normal;
                ratingVehicleInfos[i].ratingVehicleRank[countRace].fontSize = (vehiclesRanks[countRace][i] == 1) ? 20f : 16f;

                if (countRace - 1 >= 0)
                {
                    ratingVehicleInfos[i].ratingVehicleRank[countRace - 1].color = Color.grey;
                }
            }

            //commentator.RanksVehicle(vehiclesRanks[countRace]);
            commentator.FirstRankVehicle(vehiclesRanks[countRace]);

            countRace++;

            if (countRace == gameValue.stepRacing)
            {
                return;
            }

            DOVirtual.DelayedCall(3f, () =>
            {
                startTransform.localPosition = Vector3.right * gameValue.circuitLength * countRace + Vector3.up * 100;
                start.PlaceOnBottom();
                endTransform.localPosition = Vector3.right * gameValue.circuitLength * (countRace + 1) + Vector3.up * 100;
                end.PlaceOnBottom();

                countVehiclesArrived = 0;

                cameraConstraint.ChangeConstraint(startCameraTarget);

                sectorsCountText.text = string.Format("<b>{0} / {1}</b>   SECTORS", countRace + 1, gameValue.stepRacing);

                ratingRectTransform.DOScaleY(0, 0.3f);

                for (int i = 0; i < vehicles.Count; ++i)
                {
                    vehicles[i].transform.localPosition = new Vector3(gameValue.circuitLength * (countRace), 100, vehicles[i].transform.localPosition.z);
                    vehicles[i].PlaceOnBottom();
                    vehicles[i].isArrived = false;
                    vehicles[i].isStartRace = false;
                }

                end.wasPrepared = false;

                StartRace();
            });
        }
    }
}