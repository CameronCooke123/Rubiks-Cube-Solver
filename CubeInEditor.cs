using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cube))]
public class CubeInEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        DrawDefaultInspector();
        Cube cube = (Cube)target;
        CubeDisplay disp = cube.gameObject.GetComponent<CubeDisplay>();

        if (GUILayout.Button("Generate Cube"))
        {
            cube.Start();
        }
        
        //up side
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Up"))
        {
            cube.RotateSide(SidesOfCube.up, false);
            cube.WriteSidesToFile();
        }
        if (GUILayout.Button("Up Prime"))
        {
            cube.RotateSide(SidesOfCube.up, true);
            cube.WriteSidesToFile();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        //down side
        if (GUILayout.Button("Down"))
        {
            cube.RotateSide(SidesOfCube.down, false);
            cube.WriteSidesToFile();
        }
        if (GUILayout.Button("Down Prime"))
        {
            cube.RotateSide(SidesOfCube.down, true);
            cube.WriteSidesToFile();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        //right side
        if (GUILayout.Button("Right"))
        {
            cube.RotateSide(SidesOfCube.right, false);
            cube.WriteSidesToFile();
        }
        if (GUILayout.Button("Right Prime"))
        {
            cube.RotateSide(SidesOfCube.right, true);
            cube.WriteSidesToFile();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        //left side
        if (GUILayout.Button("Left"))
        {
            cube.RotateSide(SidesOfCube.left, false);
            cube.WriteSidesToFile();
        }
        if (GUILayout.Button("Left Prime"))
        {
            cube.RotateSide(SidesOfCube.left, true);
            cube.WriteSidesToFile();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        //front side
        if (GUILayout.Button("Front"))
        {
            cube.RotateSide(SidesOfCube.front, false);
            cube.WriteSidesToFile();
        }
        if (GUILayout.Button("Front Prime"))
        {
            cube.RotateSide(SidesOfCube.front, true);
            cube.WriteSidesToFile();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        //back side
        if (GUILayout.Button("Back"))
        {
            cube.RotateSide(SidesOfCube.back, false);
            cube.WriteSidesToFile();
        }
        if (GUILayout.Button("Back Prime"))
        {
            cube.RotateSide(SidesOfCube.back, true);
            cube.WriteSidesToFile();
        }
        GUILayout.EndHorizontal();
    }
}
