using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZoneLevel", menuName = "ScriptableObjects/NewZoneLevel", order = 1)]
public class ZoneLevelScriptable : ScriptableObject
{
    public float levelLengthInSeconds;
    public List<ZoneLevelObstacle> obstacles = new List<ZoneLevelObstacle>();
    public List<ZoneLevelMelonSpawn> melons = new List<ZoneLevelMelonSpawn>();
}


[Serializable]
public class ZoneLevelObstacle
{
    [Header("When the the obstacle is at player-position")]
    public float obstacleTime;
    public float obstacleTimeToReachPlayer;
    public GameObject obstaclePrefab;
    public Color wallColor;
    [Range(0.2f, 0.8f)] public float obstacleScreenPos_y;
    [Range(0.2f, 0.8f)] public float obstacleScreenPos_X;
}

[Serializable]
public class ZoneLevelMelonSpawn
{
    [Header("When the melon is at player position")]
    public float melonTime;
    public float melonTimeToReachPlayer;
    public GameObject melonPrefab;
    [Range(0.2f, 0.8f)] public float melonScreenPos_y;
    [Range(0.2f, 0.8f)] public float melonScreenPos_X;
}