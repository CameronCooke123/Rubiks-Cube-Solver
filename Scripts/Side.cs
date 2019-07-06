using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Side
{
    public Cubie center;
    public List<Cubie> edges;
    public List<Cubie> corners;

    public Side ()
    {
        center = null;
        edges = new List<Cubie>();
        corners = new List<Cubie>();
    }
    public Side (Cubie centerPiece, Cubie[] edgePieces, Cubie[] cornerPieces)
    {
        center = centerPiece;
        for (int i = 0; i < 4; i++)
        {
            edges.Add(edgePieces[i]);
            corners.Add(cornerPieces[i]);
        }
    }
}
