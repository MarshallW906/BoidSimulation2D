using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoidSettingData", menuName = "ScriptableObjects/BoidSettings", order = 1)]
public class BoidSettings : ScriptableObject
{
    [Header("Perception")]
    [Range(0f, 90f)] public float backIgnoreHalfAngle;
    [Range(0f, 1f)] public float perceptionRadiusRatio;
    [Range(0f, 100f)] public float innerRadius;

    [Header("Extreme Positions")]
    [Range(1f, 100f)] public float maxPosX;
    [Range(1f, 100f)] public float maxPosY;

    [Header("Boid simulation params")]
    public bool useRandomInitMoveDir;
    public Vector2 initMoveDir;
    [Range(0f, 10f)] public float maxMoveSpeedSqr;
    [Range(0f, 1f)] public float kDamp;

    [Range(0f, 10f)] public float kCollisionAvoidance;
    [Range(0f, 10f)] public float kSeperation;
    [Range(0f, 10f)] public float kAlignment;
    [Range(0f, 10f)] public float kCohesion;

    [Range(0f, 10f)] public float accelerationQuotaSqr;
}
