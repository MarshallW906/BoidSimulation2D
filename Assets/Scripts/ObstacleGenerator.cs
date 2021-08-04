using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    public GameObject prefabObstacle;

    [Range(0f, 60f)] public float maxPosX;
    [Range(0f, 60f)] public float maxPosY;

    public bool generateCircleAtStart;
    public bool generateRandomPosAtStart;
    public float radius;
    public int obstacleNumber;

    private void Start()
    {
        if (generateCircleAtStart) { GenerateCircularObstacles(radius, obstacleNumber); }
        if (generateRandomPosAtStart) { GenerateObstacles(obstacleNumber); }
    }

    public void GenerateObstacles(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(prefabObstacle, new Vector3(Random.Range(-maxPosX, maxPosX), Random.Range(-maxPosY, maxPosY), 0), Quaternion.identity, transform);
        }
    }

    public void GenerateCircularObstacles(float r, int n)
    {
        var singleDegree = 360 / n;
        var singleRadians = Mathf.Deg2Rad * singleDegree;
        for (int i = 0; i < n; i++)
        {
            Instantiate(prefabObstacle, new Vector3(r * Mathf.Cos(i * singleRadians), r * Mathf.Sin(i * singleRadians), 0), Quaternion.identity, transform);
        }
    }

    public void EditorCleanAllObstacles()
    {
        var childList = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            childList.Add(transform.GetChild(i).gameObject);
        }
        childList.ForEach(child => DestroyImmediate(child));
    }
}
