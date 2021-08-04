using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Marshall (Xiangyu) Wang
 * Github: @MarshallW906/BoidSimulation2D
 * Time: Aug 2021
 */
public class BoidGenerator : MonoBehaviour
{
    public GameObject prefabBoid;

    [Range(0f, 60f)] public float randomRangeX;
    [Range(0f, 60f)] public float randomRangeY;

    public bool generateBoidsAtStart;
    public int boidNumber;

    private void Start()
    {
        if (generateBoidsAtStart) { GenerateNewBoid(boidNumber); }
    }

    public void GenerateNewBoid(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(prefabBoid, new Vector3(Random.Range(-randomRangeX, randomRangeX), Random.Range(-randomRangeY, randomRangeY), 0), Quaternion.identity, transform);
        }
    }

    public void EditorCleanAllBoids()
    {
        var childList = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            childList.Add(transform.GetChild(i).gameObject);
        }
        childList.ForEach(child => DestroyImmediate(child));
    }
}
