using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(BoidGenerator))]
public class BoidGeneratorEditor : Editor
{
    SerializedProperty boidNumber;

    private void OnEnable()
    {
        boidNumber = serializedObject.FindProperty("boidNumber");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //base.OnInspectorGUI();

        var boidGenerator = target as BoidGenerator;
        if (GUILayout.Button("Generate Boids"))
        {
            boidGenerator.GenerateNewBoid(boidNumber.intValue);
        }

        if (GUILayout.Button("Clear"))
        {
            boidGenerator.EditorCleanAllBoids();
        }
    }
}
