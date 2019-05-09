using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_GameValue", menuName = "SpeedBetRacing/GameValue")]
public class GameValue : ScriptableObject
{
    [Serializable]
    public struct VehicleInfo
    {
        public bool isActive;
        public string name;
        public KeyCode keyCode;
        public GameObject prefab;
        public Material material;
        public Color color;
    }

    public List<VehicleInfo> vehiclesInfos;

    [Header("VEHICLES")]

    public float baseSpeed;
    public float addBetSpeed;
    
    public bool withResetSpeed;
    public int speedStepToOverride;
    public float overrideChance;


    [Header("BETS")]

    public int stepRacing;

    [Range(0, 100)]
    public float[] checkpointPositions; 
    public float timeToApplyBet;

    
    [Header("CIRCUIT")]

    public float circuitLength;
}
