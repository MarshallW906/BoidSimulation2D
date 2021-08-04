using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Author: Marshall (Xiangyu) Wang
 * Github: @MarshallW906/BoidSimulation2D
 * Time: Aug 2021
 * This Boid model was implemented accourding to Craig Reynolds' paper in 1986: http://www.red3d.com/cwr/boids/
 */
public class CommonBoidSettings : Singleton<CommonBoidSettings>
{
    [Header("Perception")]
    [Range(0f, 90f)] public float backIgnoreHalfAngle;
    [Range(0f, 1f)] public float perceptionRadiusRatio;
    [Range(0f, 100f)] public float innerRadius;

    [Header("Extreme Positions")]
    [Range(1f, 100f)] public float maxPosX;
    [Range(1f, 100f)] public float maxPosY;

    [Header("Boid simulation params")]
    [SerializeField] private bool useRandomInitMoveDir;
    public Vector2 initMoveDir;
    [Range(0f, 10f)] public float maxMoveSpeedSqr;
    [Range(0f, 1f)] public float kDamp;

    [Range(0f, 10f)] public float kCollisionAvoidance;
    [Range(0f, 10f)] public float kSeperation;
    [Range(0f, 10f)] public float kAlignment;
    [Range(0f, 10f)] public float kCohesion;

    [Range(0f, 10f)] public float accelerationQuotaSqr;

    private void Start()
    {
        if (useRandomInitMoveDir)
        {
            initMoveDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }
    }
}
