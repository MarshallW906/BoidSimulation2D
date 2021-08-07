using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Marshall (Xiangyu) Wang
 * Github: @MarshallW906/BoidSimulation2D
 * Time: Aug 2021
 * This Boid model was implemented accourding to Craig Reynolds' paper in 1986: http://www.red3d.com/cwr/boids/
 */
public class SingleBoid : MonoBehaviour
{
    // all the comments were replaced by CommonBuildSettings.Instance.xxxxx
    //[Range(0f, 100f)] public float innerRadius;
    //[Range(0f, 90f)] public float backIgnoreHalfAngle;

    public BoidSettings settings;

    private CircleCollider2D CircleCollider2D;

    private List<CircleCollider2D> obstaclesNearBy;
    private List<SingleBoid> boidsNearBy;

    private static readonly string tagObstacle = "obstacle";
    private static readonly string tagBoid = "boid";

    //[Header("Extreme Positions")]
    //[Range(1f, 100f)] public float maxPosX;
    //[Range(1f, 100f)] public float maxPosY;

    [Header("Info")]
    public Vector3 moveDir;
    public Vector3 totalAcceleration;

    [Header("Info / Accelerations")]
    public Vector3 accCollisionAvoidance;
    public int accCollisionAvoidanceCount;
    public float accCollisionAvoidanceSqrMag;

    public Vector3 accSeperation;
    public int accSeperationCount;
    public float accSeperationSqrMag;

    public Vector3 accAlignment;
    public int accAlignmentCount;
    public float accAlignmentSqrMag;

    public Vector3 accCohesion;
    public int accCohesionCount;
    public float accCohesionSqrMag;

    [Header("Info / Quota")]
    //[Range(0f, 10f)] public float accelerationQuotaSqr;
    public Vector3 realAccSeperationApplied;
    public Vector3 realAccAlignmentApplied;
    public Vector3 realAccCohesionApplied;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.name, gameObject);
        if (collision as CircleCollider2D)
        {
            if (collision.CompareTag(tagObstacle))
            {
                if (IsInPerceptionSector(collision.transform.position))
                {
                    obstaclesNearBy.Add(collision as CircleCollider2D);
                }
            }
            // see physics2D settings: Collisions between BoidPerception themselves are disabled
            // only collisions between BoidPerception and BoidInner
            else if (collision.CompareTag(tagBoid))
            {
                //Debug.Log($"{collision.transform}, {collision.transform.parent}", gameObject);
                boidsNearBy.Add(collision.transform.parent.GetComponent<SingleBoid>());
            }
        }
    }

    private bool IsInPerceptionSector(Vector3 pos)
    {
        // ignore objects in the back sector with halfTheta=backIgnoreHalfAngle
        var toPos = pos - transform.position;
        // apparently its faster to compare by squares than by lengths themselves
        if (toPos.sqrMagnitude < 
            (CircleCollider2D.radius * settings.perceptionRadiusRatio 
            * CircleCollider2D.radius * settings.perceptionRadiusRatio))
        {
            return false;
        }
        var threshold = -1 * Mathf.Cos(settings.backIgnoreHalfAngle * Mathf.Deg2Rad);
        return Vector3.Dot(toPos.normalized, moveDir.normalized) > threshold;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision as CircleCollider2D)
        {
            if (collision.CompareTag(tagObstacle))
            {
                obstaclesNearBy.Remove(collision as CircleCollider2D);
            }
            // see physics2D settings: Collisions between BoidPerception themselves are disabled
            // only collisions between BoidPerception and BoidInner
            else if (collision.CompareTag(tagBoid))
            {
                boidsNearBy.Remove(collision.transform.parent.GetComponent<SingleBoid>());
            }
        }
    }

    private void Awake()
    {
        CircleCollider2D = GetComponent<CircleCollider2D>();

        obstaclesNearBy = new List<CircleCollider2D>();
        boidsNearBy = new List<SingleBoid>();
    }

    private void Start()
    {
        moveDir = settings.useRandomInitMoveDir ? new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) : settings.initMoveDir;
    }

    private void Update()
    {
        CalcAllAccelerations();
        ApplyAccelerationByQuota();

        UpdatePosAndRot();
    }

    private void CalcAllAccelerations()
    {
        accCollisionAvoidance = CalcAccCollisionAvoidance();
        accCollisionAvoidanceSqrMag = accCollisionAvoidance.sqrMagnitude;
        accSeperation = CalcAccSeperation();
        accSeperationSqrMag = accSeperation.sqrMagnitude;
        accAlignment = CalcAccAlignment();
        accAlignmentSqrMag = accAlignment.sqrMagnitude;
        accCohesion = CalcAccCohesion();
        accCohesionSqrMag = accCohesion.sqrMagnitude;
    }

    private void ApplyAccelerationByQuota()
    {
        var accAdded = Vector3.zero;

        // always apply collision avoidance
        accAdded += accCollisionAvoidance;

        // change the order of the 3 lines below if you want different priorities of the three kinds of accelerations
        realAccSeperationApplied = CalcRealAccAppliedByQuota(accSeperation, accAdded, settings.accelerationQuotaSqr);
        accAdded += realAccSeperationApplied;
        realAccAlignmentApplied = CalcRealAccAppliedByQuota(accAlignment, accAdded, settings.accelerationQuotaSqr);
        accAdded += realAccAlignmentApplied;
        realAccCohesionApplied = CalcRealAccAppliedByQuota(accCohesion, accAdded, settings.accelerationQuotaSqr);
        accAdded += realAccCohesionApplied;

        totalAcceleration = accAdded;
    }

    private Vector3 CalcRealAccAppliedByQuota(Vector3 nextAccToAdd, Vector3 curAccAdded, float quotaSqr)
    {
        var quotaSqrLeft = quotaSqr - curAccAdded.sqrMagnitude;
        if (quotaSqrLeft > 0)
        {
            var accSqrMag = nextAccToAdd.sqrMagnitude;
            float scalar;
            if (quotaSqrLeft > accSqrMag)
            {
                // will NOT exceed the quota ---- can still apply full of next acceleration
                scalar = 1;
            }
            else
            {
                // the next acceleration vector will exceed the quota - capped at the quota
                scalar = Mathf.Sqrt(settings.accelerationQuotaSqr) - curAccAdded.magnitude;
            }
            return scalar * nextAccToAdd;
        }
        return Vector3.zero;
    }

    private void UpdatePosAndRot()
    {
        moveDir += totalAcceleration;

        if (moveDir.sqrMagnitude > settings.maxMoveSpeedSqr)
        {
            moveDir *= 1 - ((1 - settings.kDamp) * Time.deltaTime);
        }
        Vector3 newPos = transform.position + moveDir * Time.deltaTime;

        if (Mathf.Abs(newPos.x) >= settings.maxPosX)
        {
            newPos.x *= -1;
        }
        if (Mathf.Abs(newPos.y) >= settings.maxPosY)
        {
            newPos.y *= -1;
        }


        transform.position = newPos;
        transform.up = moveDir;
    }

    #region Accelerations: CollisionAvoidance, Seperation, Alignment, Cohesion

    private Vector3 CalcAccCollisionAvoidance()
    {
        Vector3 result = Vector3.zero;
        var myInnerColliderCenter = transform.position;
        var myInnerColliderRadius = settings.innerRadius;
        accCollisionAvoidanceCount = 0;

        foreach (var circleCollider in obstaclesNearBy)
        {
            var colliderCenter = circleCollider.bounds.center;
            var colliderRadius = circleCollider.radius;
            var toObstacle = colliderCenter - myInnerColliderCenter;
            var projectedVector = Vector3.Project(toObstacle, moveDir);
            var offset = projectedVector - toObstacle;
            if (offset.sqrMagnitude < 0.001f)
            {
                offset = Vector3.Cross(toObstacle, Vector3.forward);
            }
            var distance = offset.magnitude;
            if (distance < colliderRadius + myInnerColliderRadius)
            {
                // should apply forces
                accCollisionAvoidanceCount++;
                var forceDir = offset.normalized;
                var distanceDiff = colliderRadius + myInnerColliderRadius - distance;
                result += settings.kCollisionAvoidance * distanceDiff * forceDir;
            }
        }

        return result;
    }

    private Vector3 CalcAccSeperation()
    {
        Vector3 result = Vector3.zero;
        //var myInnerColliderCenter = transform.position;
        //var myInnerColliderRadius = innerRadius;
        accSeperationCount = 0;

        foreach (var boid in boidsNearBy)
        {
            var boidPos = boid.transform.position;
            var fromBoid = transform.position - boidPos;
            var distanceSqrReciprocal = 1.0f / fromBoid.sqrMagnitude;
            //if (distanceSqrReciprocal <= float.Epsilon) { continue; }

            result += settings.kSeperation * distanceSqrReciprocal * fromBoid.normalized;
            accSeperationCount++;
        }

        return result;
    }

    private Vector3 CalcAccAlignment()
    {
        Vector3 assembledWeightedAllySpeed = Vector3.zero;
        float sumDistanceSqrReciprocal = 0;
        accAlignmentCount = 0;

        foreach (var boid in boidsNearBy)
        {
            var boidPos = boid.transform.position;
            var fromBoid = transform.position - boidPos;
            var distanceSqrReciprocal = 1.0f / fromBoid.sqrMagnitude;

            assembledWeightedAllySpeed += distanceSqrReciprocal * boid.moveDir;
            sumDistanceSqrReciprocal += distanceSqrReciprocal;
            accAlignmentCount++;
        }

        //Debug.Log($"{assembledWeightedAllySpeed}, {sumDistanceSqrReciprocal}", gameObject);
        if (sumDistanceSqrReciprocal <= float.Epsilon) { return Vector3.zero; }
        Vector3 result = settings.kAlignment * (assembledWeightedAllySpeed / sumDistanceSqrReciprocal - moveDir);
        return result;
    }

    private Vector3 CalcAccCohesion()
    {
        Vector3 assembledWeightedAllyPos = Vector3.zero;
        float sumDistanceSqrReciprocal = 0;
        accCohesionCount = 0;

        foreach (var boid in boidsNearBy)
        {
            var boidPos = boid.transform.position;
            var fromBoid = transform.position - boidPos;
            var distanceSqrReciprocal = 1.0f / fromBoid.sqrMagnitude;

            assembledWeightedAllyPos += distanceSqrReciprocal * boidPos;
            sumDistanceSqrReciprocal += distanceSqrReciprocal;

            accCohesionCount++;
        }

        if (sumDistanceSqrReciprocal <= float.Epsilon) { return Vector3.zero; }
        Vector3 result = settings.kCohesion * (assembledWeightedAllyPos / sumDistanceSqrReciprocal - transform.position);
        return result;
    }

    #endregion

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, )
    //}
}
