using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * Author: Marshall (Xiangyu) Wang
 * Github: @MarshallW906/BoidSimulation2D
 * Time: Aug 2021
 */
[CustomEditor(typeof(ObstacleGenerator))]
public class ObstacleGeneratorEditor : Editor
{
    SerializedProperty radius;
    SerializedProperty obstacleNumber;


    private void OnEnable()
    {
        radius = serializedObject.FindProperty("radius");
        obstacleNumber = serializedObject.FindProperty("obstacleNumber");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //base.OnInspectorGUI();

        var obstacleGenerator = target as ObstacleGenerator;
        if (GUILayout.Button("Generate Obstacles"))
        {
            obstacleGenerator.GenerateObstacles(obstacleNumber.intValue);
        }
        if (GUILayout.Button("Generate Obstacle Circle"))
        {
            obstacleGenerator.GenerateCircularObstacles(radius.floatValue, obstacleNumber.intValue);
        }
        if (GUILayout.Button("Clear"))
        {
            obstacleGenerator.EditorCleanAllObstacles();
        }
    }
}
