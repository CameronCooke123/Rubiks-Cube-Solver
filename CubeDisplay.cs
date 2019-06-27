using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeDisplay : MonoBehaviour
{
    public GameObject cubePrefab;

    Cube cube;

    public GameObject[] centers, edges, corners;
    Cubie[] centerPointers, edgePointers, cornerPointers;
    public float rotationSpeed = 10;
    bool rotating;

    void Awake ()
    {
        cube = GetComponent<Cube>();

        centers = new GameObject[6];
        edges = new GameObject[12];
        corners = new GameObject[8];

        centerPointers = new Cubie[6];
        edgePointers = new Cubie[12];
        cornerPointers = new Cubie[8];

        cube.newCubeGenerated += SetWorldCubes;
        cube.rotationMade += RotateSide;
    }

    void Update ()
    {
        if (!rotating)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    cube.RotateSide(SidesOfCube.up, true);
                else
                    cube.RotateSide(SidesOfCube.up, false);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    cube.RotateSide(SidesOfCube.down, true);
                else
                    cube.RotateSide(SidesOfCube.down, false);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    cube.RotateSide(SidesOfCube.front, true);
                else
                    cube.RotateSide(SidesOfCube.front, false);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    cube.RotateSide(SidesOfCube.back, true);
                else
                    cube.RotateSide(SidesOfCube.back, false);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    cube.RotateSide(SidesOfCube.right, true);
                else
                    cube.RotateSide(SidesOfCube.right, false);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    cube.RotateSide(SidesOfCube.left, true);
                else
                    cube.RotateSide(SidesOfCube.left, false);
            }
        }
    }

    void SetWorldCubes ()
    {
        for (int i = 0; i < 6; i++)
        {
            if (centers[i] == null)
                centers[i] = Instantiate(cubePrefab);
            centerPointers[i] = cube.centerPieces[i];

            Vector3 dir = GetDirFromSide(centerPointers[i].tiles[0].sideOfCube);
            centers[i].transform.position = dir;
            centers[i].transform.rotation = Quaternion.identity;
        }
        for (int i = 0; i < 12; i++)
        {
            if (edges[i] == null)
                edges[i] = Instantiate(cubePrefab);
            edgePointers[i] = cube.edgePieces[i];

            Vector3 dir = GetDirFromSide(edgePointers[i].tiles[0].sideOfCube) + GetDirFromSide(edgePointers[i].tiles[1].sideOfCube);
            edges[i].transform.position = dir;
            edges[i].transform.rotation = Quaternion.identity;
        }
        for (int i = 0; i < 8; i++)
        {
            if (corners[i] == null)
                corners[i] = Instantiate(cubePrefab);
            cornerPointers[i] = cube.cornerPieces[i];

            Vector3 dir = GetDirFromSide(cornerPointers[i].tiles[0].sideOfCube) +
                          GetDirFromSide(cornerPointers[i].tiles[1].sideOfCube) +
                          GetDirFromSide(cornerPointers[i].tiles[2].sideOfCube);
            corners[i].transform.position = dir;
            corners[i].transform.rotation = Quaternion.identity;
        }
    }

    void RotateSide (SidesOfCube side, bool prime)
    {
        StartCoroutine(RotateSideCo(side, prime));
    }
    IEnumerator RotateSideCo (SidesOfCube side, bool prime)
    {
        rotating = true;
        //find the cubies that are on the side being rotated
        List<GameObject> cubiesToRotate = new List<GameObject>();
        for (int i = 0; i < 6; i++)
        {
            if (centers[i].transform.position == GetDirFromSide(side))
            {
                cubiesToRotate.Add(centers[i]);
                break;
            }
        }
        for (int i = 0; i < 12; i++)
        {
            foreach (Tile tile in edgePointers[i].tiles)
            {
                if (tile.sideOfCube == side)
                {
                    cubiesToRotate.Add(edges[i]);
                    break;
                }
            }
        }
        for (int i = 0; i < 8; i++)
        {
            foreach (Tile tile in cornerPointers[i].tiles)
            {
                if (tile.sideOfCube == side)
                {
                    cubiesToRotate.Add(corners[i]);
                    break;
                }
            }
        }

        //rotate them all around the axis in the correct direction
        float rotationTotal = 0;
        while (rotationTotal < 90)
        {
            float rotationAmount = Mathf.Clamp(rotationSpeed * Time.deltaTime, 0, 90 - rotationTotal);
            rotationTotal += rotationAmount;
            if (prime)
                rotationAmount *= -1;
            for (int i = 0; i < cubiesToRotate.Count; i++)
            {
                cubiesToRotate[i].transform.RotateAround(Vector3.zero, GetDirFromSide(side), rotationAmount);
            }
            yield return null;
        }

        //once the rotation is finished, round their positions and rotations to the nearest integer,
        //just in case they're slightly off
        for (int i = 0; i < cubiesToRotate.Count; i++)
        {
            Vector3 pos = cubiesToRotate[i].transform.position;
            pos.x = Mathf.Round(pos.x);
            pos.y = Mathf.Round(pos.y);
            pos.z = Mathf.Round(pos.z);
            cubiesToRotate[i].transform.position = pos;

            Vector3 rot = cubiesToRotate[i].transform.eulerAngles;
            rot.x = Mathf.Round(rot.x);
            rot.y = Mathf.Round(rot.y);
            rot.z = Mathf.Round(rot.z);
            cubiesToRotate[i].transform.eulerAngles = rot;
        }
        rotating = false;
    }

    Vector3 GetDirFromSide (SidesOfCube side)
    {
        switch (side)
        {
            case SidesOfCube.up:
                return Vector3.up;
            case SidesOfCube.down:
                return Vector3.down;
            case SidesOfCube.right:
                return Vector3.left;
            case SidesOfCube.left:
                return Vector3.right;
            case SidesOfCube.front:
                return Vector3.forward;
            default:
                return Vector3.back;
        }
    }

    Vector3 RoundToNearestMultiple (Vector3 vec, float multiple)
    {
        vec.x = Mathf.Round(vec.x / multiple) * multiple;
        vec.y = Mathf.Round(vec.y / multiple) * multiple;
        vec.z = Mathf.Round(vec.z / multiple) * multiple;
        return vec;
    }
}
