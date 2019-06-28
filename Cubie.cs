using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cubie
{
    public readonly int numTiles;
    public Tile[] tiles;    //Count = 1 for center piece, 2 for edge piece, 3 for corner piece
    
    public Cubie (int tileCount, Tile[] tilesArr)
    {
        numTiles = tileCount;
        tiles = tilesArr;
    }

    public void RotateTiles (SidesOfCube axis, bool prime)
    {
        if (axis == SidesOfCube.up)
        {
            foreach (Tile tile in tiles)
                RotateCubieUD(tile, prime);
        }
        if (axis == SidesOfCube.down)
        {
            foreach (Tile tile in tiles)
                RotateCubieUD(tile, !prime);
        }
        else if (axis == SidesOfCube.right)
        {
            foreach (Tile tile in tiles)
                RotateCubieRL(tile, prime);
        }
        else if (axis == SidesOfCube.left)
        {
            foreach (Tile tile in tiles)
                RotateCubieRL(tile, !prime);
        }
        else if (axis == SidesOfCube.front)
        {
            foreach (Tile tile in tiles)
                RotateCubieFB(tile, prime);
        }
        else if (axis == SidesOfCube.back)
        {
            foreach (Tile tile in tiles)
                RotateCubieFB(tile, !prime);
        }
    }
    public void RotateTiles (MidLayersOfCube layer, bool prime)
    {
        if (layer == MidLayersOfCube.Middle)
        {
            foreach (Tile tile in tiles)
                RotateCubieRL(tile, prime);
        }
        else if (layer == MidLayersOfCube.Equatorial)
        {
            foreach (Tile tile in tiles)
                RotateCubieUD(tile, prime);
        }
        else
        {
            foreach (Tile tile in tiles)
                RotateCubieFB(tile, prime);
        }
    }

    void RotateCubieUD (Tile tile, bool prime)
    {
        if (tile.sideOfCube == SidesOfCube.right)
        {
            if (!prime)
                tile.sideOfCube = SidesOfCube.front;
            else
                tile.sideOfCube = SidesOfCube.back;
        }
        else if (tile.sideOfCube == SidesOfCube.front)
        {
            if (!prime)
                tile.sideOfCube = SidesOfCube.left;
            else
                tile.sideOfCube = SidesOfCube.right;
        }
        else if (tile.sideOfCube == SidesOfCube.left)
        {
            if (!prime)
                tile.sideOfCube = SidesOfCube.back;
            else
                tile.sideOfCube = SidesOfCube.front;
        }
        else if (tile.sideOfCube == SidesOfCube.back)
        {
            if (!prime)
                tile.sideOfCube = SidesOfCube.right;
            else
                tile.sideOfCube = SidesOfCube.left;
        }
    }
    void RotateCubieRL (Tile tile, bool prime)
    {
        if (tile.sideOfCube == SidesOfCube.up)
        {
            if (!prime)
                tile.sideOfCube = SidesOfCube.back;
            else
                tile.sideOfCube = SidesOfCube.front;
        }
        else if (tile.sideOfCube == SidesOfCube.front)
        {
            if (!prime)
                tile.sideOfCube = SidesOfCube.up;
            else
                tile.sideOfCube = SidesOfCube.down;
        }
        else if (tile.sideOfCube == SidesOfCube.down)
        {
            if (!prime)
                tile.sideOfCube = SidesOfCube.front;
            else
                tile.sideOfCube = SidesOfCube.back;
        }
        else if (tile.sideOfCube == SidesOfCube.back)
        {
            if (!prime)
                tile.sideOfCube = SidesOfCube.down;
            else
                tile.sideOfCube = SidesOfCube.up;
        }
    }
    void RotateCubieFB (Tile tile, bool prime)
    {
        if (tile.sideOfCube == SidesOfCube.up)
        {
            if (!prime)
                tile.sideOfCube = SidesOfCube.right;
            else
                tile.sideOfCube = SidesOfCube.left;
        }
        else if (tile.sideOfCube == SidesOfCube.right)
        {
            if (!prime)
                tile.sideOfCube = SidesOfCube.down;
            else
                tile.sideOfCube = SidesOfCube.up;
        }
        else if (tile.sideOfCube == SidesOfCube.down)
        {
            if (!prime)
                tile.sideOfCube = SidesOfCube.left;
            else
                tile.sideOfCube = SidesOfCube.right;
        }
        else if (tile.sideOfCube == SidesOfCube.left)
        {
            if (!prime)
                tile.sideOfCube = SidesOfCube.up;
            else
                tile.sideOfCube = SidesOfCube.down;
        }
    }
}
