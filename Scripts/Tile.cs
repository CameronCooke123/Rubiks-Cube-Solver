using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    public readonly Colors color;
    public SidesOfCube sideOfCube;

    public Tile (Colors col, SidesOfCube side)
    {
        color = col;
        sideOfCube = side;
    }
    public string GetColor ()
    {
        switch (color)
        {
            case Colors.red:
                return "r";
            case Colors.orange:
                return "o";
            case Colors.white:
                return "w";
            case Colors.yellow:
                return "y";
            case Colors.blue:
                return "b";
            default:
                return "g";
        }
    }
}
