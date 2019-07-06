//#pragma strict

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFaceColors : MonoBehaviour
{
    void Start ()
    {
        var mf = GetComponent<MeshFilter>();
        Mesh mesh;
        if (mf == null)
            return;

        mesh = mf.mesh;

        if (mesh == null || mesh.uv.Length != 24)
        {
            Debug.Log("Script needs to be attached to built-in cube");
            return;
        }

        var uvs = mesh.uv;

        // Front
        uvs[0] = new Vector2(0.0f, 0.0f);
        uvs[1] = new Vector2(0.333f, 0.0f);
        uvs[2] = new Vector2(0.0f, 0.333f);
        uvs[3] = new Vector2(0.333f, 0.333f);
        // Top
        uvs[4] = new Vector2(0.334f, 0.333f);
        uvs[5] = new Vector2(0.666f, 0.333f);
        uvs[8] = new Vector2(0.334f, 0.0f);
        uvs[9] = new Vector2(0.666f, 0.0f);
        // Back
        uvs[6] = new Vector2(1.0f, 0.0f);
        uvs[7] = new Vector2(0.667f, 0.0f);
        uvs[10] = new Vector2(1.0f, 0.333f);
        uvs[11] = new Vector2(0.667f, 0.333f);
        // Bottom
        uvs[12] = new Vector2(0.0f, 0.334f);
        uvs[13] = new Vector2(0.0f, 0.666f);
        uvs[14] = new Vector2(0.333f, 0.666f);
        uvs[15] = new Vector2(0.333f, 0.334f);
        // Left
        uvs[16] = new Vector2(0.334f, 0.334f);
        uvs[17] = new Vector2(0.334f, 0.666f);
        uvs[18] = new Vector2(0.666f, 0.666f);
        uvs[19] = new Vector2(0.666f, 0.334f);
        // Right        
        uvs[20] = new Vector2(0.667f, 0.334f);
        uvs[21] = new Vector2(0.667f, 0.666f);
        uvs[22] = new Vector2(1.0f, 0.666f);
        uvs[23] = new Vector2(1.0f, 0.334f);

        mesh.uv = uvs;
    }
}
