using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;                                                                                                 
                                                                                                                   
public class Cube : MonoBehaviour                                                                                  
{
    //the edges and corners that are on each side in clockwise order
    readonly int[] edgeIndexesUp = { 0, 1, 2, 3 };
    readonly int[] edgeIndexesDown = { 11, 10, 9, 8 };
    readonly int[] edgeIndexesRight = { 3, 6, 11, 7 };
    readonly int[] edgeIndexesLeft = { 1, 4, 9, 5 };
    readonly int[] edgeIndexesFront = { 0, 7, 8, 4 };
    readonly int[] edgeIndexesBack = { 2, 5, 10, 6 };
    readonly int[] cornerIndexesUp = { 0, 1, 2, 3 };
    readonly int[] cornerIndexesDown = { 7, 6, 5, 4 };
    readonly int[] cornerIndexesRight = { 3, 2, 6, 7 };
    readonly int[] cornerIndexesLeft = { 1, 0, 4, 5 };
    readonly int[] cornerIndexesFront = { 0, 3, 7, 4 };
    readonly int[] cornerIndexesBack = { 2, 1, 5, 6 };

    public delegate void RotationMade (SidesOfCube side, bool prime);
    public event RotationMade rotationMade;
    public delegate void NewCubeGenerated ();
    public event NewCubeGenerated newCubeGenerated;


    public Cubie[] centerPieces;                                                                                   
    public Cubie[] edgePieces;                                                                                     
    public Cubie[] cornerPieces;

    public void Start ()                                                                                           
    {                                                                                                              //  Cubie Position Key
        //print("Generating New Cube...");                                                                           //  Top down view
        GenerateNewCube();                                                                                         //           T = center, E = edge, C = corner
        //print("New Cube Generated.");                                                                              //
    }                                                                                                              //                  C1 E2 C2
                                                                                                                   //      Top Layer   E1 T0 E3
    void GenerateNewCube ()                                                                                        //                  C0 E0 C3
    {                                                                                                              //
        //a cubie's position in the array will tell where on the cube it currently is - see key -->                //                  E5 T3 E6
        centerPieces = GenerateCenterPieces();                                                                     //      Mid Layer   T2    T4
        edgePieces = GenerateEdgePieces();                                                                         //                  E4 T1 E7
        cornerPieces = GenerateCornerPieces();                                                                     //
        if (newCubeGenerated != null)                                                                              //                  C5  E10 C6
            newCubeGenerated();                                                                                    //      Bot Layer   E9  T5  E11
        WriteSidesToFile();                                                                                        //                  C4  E8  C7
    }                                                                                                              
                                                                                                                   
    Cubie[] GenerateCenterPieces ()
    {
        Cubie[] centers = new Cubie[6];
        for (int i = 0; i < 6; i++)
        {
            Tile[] tiles = new Tile[1];

            Colors col;
            SidesOfCube side;
            if (i == 0)
                col = Colors.red;
            else if (i == 1)
                col = Colors.blue;
            else if (i == 2)
                col = Colors.yellow;
            else if (i == 3)
                col = Colors.green;
            else if (i == 4)
                col = Colors.white;
            else
                col = Colors.orange;
            side = GetDefaultSide(col);
            
            tiles[0] = new Tile(col, side);

            centers[i] = new Cubie(1, tiles);
        }

        return centers;
    }
    Cubie[] GenerateEdgePieces ()
    {
        Cubie[] edges = new Cubie[12];
        for (int i = 0; i < 12; i++)
        {
            Tile[] tiles = new Tile[2];

            if (i == 0)
            {
                tiles[0] = new Tile(Colors.red, GetDefaultSide(Colors.red));
                tiles[1] = new Tile(Colors.blue, GetDefaultSide(Colors.blue));
            }
            else if (i == 1)
            {
                tiles[0] = new Tile(Colors.red, GetDefaultSide(Colors.red));
                tiles[1] = new Tile(Colors.yellow, GetDefaultSide(Colors.yellow));
            }
            else if (i == 2)
            {
                tiles[0] = new Tile(Colors.red, GetDefaultSide(Colors.red));
                tiles[1] = new Tile(Colors.green, GetDefaultSide(Colors.green));
            }
            else if (i == 3)
            {
                tiles[0] = new Tile(Colors.red, GetDefaultSide(Colors.red));
                tiles[1] = new Tile(Colors.white, GetDefaultSide(Colors.white));
            }
            else if (i == 4)
            {
                tiles[0] = new Tile(Colors.blue, GetDefaultSide(Colors.blue));
                tiles[1] = new Tile(Colors.yellow, GetDefaultSide(Colors.yellow));
            }
            else if (i == 5)
            {
                tiles[0] = new Tile(Colors.yellow, GetDefaultSide(Colors.yellow));
                tiles[1] = new Tile(Colors.green, GetDefaultSide(Colors.green));
            }
            else if (i == 6)
            {
                tiles[0] = new Tile(Colors.green, GetDefaultSide(Colors.green));
                tiles[1] = new Tile(Colors.white, GetDefaultSide(Colors.white));
            }
            else if (i == 7)
            {
                tiles[0] = new Tile(Colors.white, GetDefaultSide(Colors.white));
                tiles[1] = new Tile(Colors.blue, GetDefaultSide(Colors.blue));
            }
            else if (i == 8)
            {
                tiles[0] = new Tile(Colors.orange, GetDefaultSide(Colors.orange));
                tiles[1] = new Tile(Colors.blue, GetDefaultSide(Colors.blue));
            }
            else if (i == 9)
            {
                tiles[0] = new Tile(Colors.orange, GetDefaultSide(Colors.orange));
                tiles[1] = new Tile(Colors.yellow, GetDefaultSide(Colors.yellow));
            }
            else if (i == 10)
            {
                tiles[0] = new Tile(Colors.orange, GetDefaultSide(Colors.orange));
                tiles[1] = new Tile(Colors.green, GetDefaultSide(Colors.green));
            }
            else
            {
                tiles[0] = new Tile(Colors.orange, GetDefaultSide(Colors.orange));
                tiles[1] = new Tile(Colors.white, GetDefaultSide(Colors.white));
            }

            edges[i] = new Cubie(2, tiles);
        }

        return edges;
    }
    Cubie[] GenerateCornerPieces ()
    {
        Cubie[] corners = new Cubie[8];
        for (int i = 0; i < 8; i++)
        {
            Tile[] tiles = new Tile[3];

            if (i == 0)
            {
                tiles[0] = new Tile(Colors.red, GetDefaultSide(Colors.red));
                tiles[1] = new Tile(Colors.blue, GetDefaultSide(Colors.blue));
                tiles[2] = new Tile(Colors.yellow, GetDefaultSide(Colors.yellow));
            }
            else if (i == 1)
            {
                tiles[0] = new Tile(Colors.red, GetDefaultSide(Colors.red));
                tiles[1] = new Tile(Colors.yellow, GetDefaultSide(Colors.yellow));
                tiles[2] = new Tile(Colors.green, GetDefaultSide(Colors.green));
            }
            else if (i == 2)
            {
                tiles[0] = new Tile(Colors.red, GetDefaultSide(Colors.red));
                tiles[1] = new Tile(Colors.green, GetDefaultSide(Colors.green));
                tiles[2] = new Tile(Colors.white, GetDefaultSide(Colors.white));
            }
            else if (i == 3)
            {
                tiles[0] = new Tile(Colors.red, GetDefaultSide(Colors.red));
                tiles[1] = new Tile(Colors.white, GetDefaultSide(Colors.white));
                tiles[2] = new Tile(Colors.blue, GetDefaultSide(Colors.blue));
            }
            else if (i == 4)
            {
                tiles[0] = new Tile(Colors.orange, GetDefaultSide(Colors.orange));
                tiles[1] = new Tile(Colors.yellow, GetDefaultSide(Colors.yellow));
                tiles[2] = new Tile(Colors.blue, GetDefaultSide(Colors.blue));
            }
            else if (i == 5)
            {
                tiles[0] = new Tile(Colors.orange, GetDefaultSide(Colors.orange));
                tiles[1] = new Tile(Colors.green, GetDefaultSide(Colors.green));
                tiles[2] = new Tile(Colors.yellow, GetDefaultSide(Colors.yellow));
            }
            else if (i == 6)
            {
                tiles[0] = new Tile(Colors.orange, GetDefaultSide(Colors.orange));
                tiles[1] = new Tile(Colors.white, GetDefaultSide(Colors.white));
                tiles[2] = new Tile(Colors.green, GetDefaultSide(Colors.green));
            }
            else
            {
                tiles[0] = new Tile(Colors.orange, GetDefaultSide(Colors.orange));
                tiles[1] = new Tile(Colors.blue, GetDefaultSide(Colors.blue));
                tiles[2] = new Tile(Colors.white, GetDefaultSide(Colors.white));
            }

            corners[i] = new Cubie(3, tiles);
        }

        return corners;
    }

    public static SidesOfCube GetDefaultSide (Colors color)
    {
        if (color == Colors.red)
            return SidesOfCube.up;
        else if (color == Colors.orange)
            return SidesOfCube.down;
        else if (color == Colors.white)
            return SidesOfCube.right;
        else if (color == Colors.yellow)
            return SidesOfCube.left;
        else if (color == Colors.blue)
            return SidesOfCube.front;
        else
            return SidesOfCube.back;
    }

    //FOR TESTING
    public void WriteSidesToFile ()
    {
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\Cameron\Dropbox\Unity Projects\Rubik's Cube Solver\Assets\Test.txt", false))
        {
            //write UP side
            file.Write(GetTile(cornerPieces[1], SidesOfCube.up).GetColor() + ' ');
            file.Write(GetTile(edgePieces[2], SidesOfCube.up).GetColor() + ' ');
            file.Write(GetTile(cornerPieces[2], SidesOfCube.up).GetColor() + '\n');

            file.Write(GetTile(edgePieces[1], SidesOfCube.up).GetColor() + ' ');
            file.Write(GetTile(centerPieces[0], SidesOfCube.up).GetColor() + ' ');
            file.Write(GetTile(edgePieces[3], SidesOfCube.up).GetColor() + '\n');

            file.Write(GetTile(cornerPieces[0], SidesOfCube.up).GetColor() + ' ');
            file.Write(GetTile(edgePieces[0], SidesOfCube.up).GetColor() + ' ');
            file.Write(GetTile(cornerPieces[3], SidesOfCube.up).GetColor() + '\n');

            file.Write('\n');
            //write DOWN side
            file.Write(GetTile(cornerPieces[5], SidesOfCube.down).GetColor() + ' ');
            file.Write(GetTile(edgePieces[10], SidesOfCube.down).GetColor() + ' ');
            file.Write(GetTile(cornerPieces[6], SidesOfCube.down).GetColor() + '\n');

            file.Write(GetTile(edgePieces[9], SidesOfCube.down).GetColor() + ' ');
            file.Write(GetTile(centerPieces[5], SidesOfCube.down).GetColor() + ' ');
            file.Write(GetTile(edgePieces[11], SidesOfCube.down).GetColor() + '\n');

            file.Write(GetTile(cornerPieces[4], SidesOfCube.down).GetColor() + ' ');
            file.Write(GetTile(edgePieces[8], SidesOfCube.down).GetColor() + ' ');
            file.Write(GetTile(cornerPieces[7], SidesOfCube.down).GetColor() + '\n');

            file.Write('\n');
            //write RIGHT side
            file.Write(GetTile(cornerPieces[3], SidesOfCube.right).GetColor() + ' ');
            file.Write(GetTile(edgePieces[3], SidesOfCube.right).GetColor() + ' ');
            file.Write(GetTile(cornerPieces[2], SidesOfCube.right).GetColor() + '\n');

            file.Write(GetTile(edgePieces[7], SidesOfCube.right).GetColor() + ' ');
            file.Write(GetTile(centerPieces[4], SidesOfCube.right).GetColor() + ' ');
            file.Write(GetTile(edgePieces[6], SidesOfCube.right).GetColor() + '\n');

            file.Write(GetTile(cornerPieces[7], SidesOfCube.right).GetColor() + ' ');
            file.Write(GetTile(edgePieces[11], SidesOfCube.right).GetColor() + ' ');
            file.Write(GetTile(cornerPieces[6], SidesOfCube.right).GetColor() + '\n');

            file.Write('\n');
            //write LEFT side
            file.Write(GetTile(cornerPieces[1], SidesOfCube.left).GetColor() + ' ');
            file.Write(GetTile(edgePieces[1], SidesOfCube.left).GetColor() + ' ');
            file.Write(GetTile(cornerPieces[0], SidesOfCube.left).GetColor() + '\n');

            file.Write(GetTile(edgePieces[5], SidesOfCube.left).GetColor() + ' ');
            file.Write(GetTile(centerPieces[2], SidesOfCube.left).GetColor() + ' ');
            file.Write(GetTile(edgePieces[4], SidesOfCube.left).GetColor() + '\n');

            file.Write(GetTile(cornerPieces[5], SidesOfCube.left).GetColor() + ' ');
            file.Write(GetTile(edgePieces[9], SidesOfCube.left).GetColor() + ' ');
            file.Write(GetTile(cornerPieces[4], SidesOfCube.left).GetColor() + '\n');

            file.Write('\n');
            //write FRONT side
            file.Write(GetTile(cornerPieces[0], SidesOfCube.front).GetColor() + ' ');
            file.Write(GetTile(edgePieces[0], SidesOfCube.front).GetColor() + ' ');
            file.Write(GetTile(cornerPieces[3], SidesOfCube.front).GetColor() + '\n');

            file.Write(GetTile(edgePieces[4], SidesOfCube.front).GetColor() + ' ');
            file.Write(GetTile(centerPieces[1], SidesOfCube.front).GetColor() + ' ');
            file.Write(GetTile(edgePieces[7], SidesOfCube.front).GetColor() + '\n');

            file.Write(GetTile(cornerPieces[4], SidesOfCube.front).GetColor() + ' ');
            file.Write(GetTile(edgePieces[8], SidesOfCube.front).GetColor() + ' ');
            file.Write(GetTile(cornerPieces[7], SidesOfCube.front).GetColor() + '\n');

            file.Write('\n');
            //write BACK side
            file.Write(GetTile(cornerPieces[2], SidesOfCube.back).GetColor() + ' ');
            file.Write(GetTile(edgePieces[2], SidesOfCube.back).GetColor() + ' ');
            file.Write(GetTile(cornerPieces[1], SidesOfCube.back).GetColor() + '\n');

            file.Write(GetTile(edgePieces[6], SidesOfCube.back).GetColor() + ' ');
            file.Write(GetTile(centerPieces[3], SidesOfCube.back).GetColor() + ' ');
            file.Write(GetTile(edgePieces[5], SidesOfCube.back).GetColor() + '\n');

            file.Write(GetTile(cornerPieces[6], SidesOfCube.back).GetColor() + ' ');
            file.Write(GetTile(edgePieces[10], SidesOfCube.back).GetColor() + ' ');
            file.Write(GetTile(cornerPieces[5], SidesOfCube.back).GetColor() + '\n');

            file.Write('\n');
        }
    }

    Tile GetTile (Cubie cubie, SidesOfCube side)
    {
        foreach (Tile tile in cubie.tiles)
        {
            if (tile.sideOfCube == side)
                return tile;
        }
        return cubie.tiles[0];
    }

    public void RotateSide (SidesOfCube side, bool prime)
    {
        //print("Rotating");
        int[] edgeIndexes, cornerIndexes;
        //set the index arrays to correspond to the side that's being rotated
        switch (side)
        {
            case SidesOfCube.up:
                edgeIndexes = edgeIndexesUp;
                cornerIndexes = cornerIndexesUp;
                break;
            case SidesOfCube.down:
                edgeIndexes = edgeIndexesDown;
                cornerIndexes = cornerIndexesDown;
                break;
            case SidesOfCube.right:
                edgeIndexes = edgeIndexesRight;
                cornerIndexes = cornerIndexesRight;
                break;
            case SidesOfCube.left:
                edgeIndexes = edgeIndexesLeft;
                cornerIndexes = cornerIndexesLeft;
                break;
            case SidesOfCube.front:
                edgeIndexes = edgeIndexesFront;
                cornerIndexes = cornerIndexesFront;
                break;
            default: // back side
                edgeIndexes = edgeIndexesBack;
                cornerIndexes = cornerIndexesBack;
                break;
        }
        //move the pieces
        if (!prime)
        {
            //edges
            Cubie temp = edgePieces[edgeIndexes[0]];
            edgePieces[edgeIndexes[0]] = edgePieces[edgeIndexes[3]];
            edgePieces[edgeIndexes[3]] = edgePieces[edgeIndexes[2]];
            edgePieces[edgeIndexes[2]] = edgePieces[edgeIndexes[1]];
            edgePieces[edgeIndexes[1]] = temp;
            //corners
            temp = cornerPieces[cornerIndexes[0]];
            cornerPieces[cornerIndexes[0]] = cornerPieces[cornerIndexes[3]];
            cornerPieces[cornerIndexes[3]] = cornerPieces[cornerIndexes[2]];
            cornerPieces[cornerIndexes[2]] = cornerPieces[cornerIndexes[1]];
            cornerPieces[cornerIndexes[1]] = temp;
        }
        else
        {
            //edges
            Cubie temp = edgePieces[edgeIndexes[0]];
            edgePieces[edgeIndexes[0]] = edgePieces[edgeIndexes[1]];
            edgePieces[edgeIndexes[1]] = edgePieces[edgeIndexes[2]];
            edgePieces[edgeIndexes[2]] = edgePieces[edgeIndexes[3]];
            edgePieces[edgeIndexes[3]] = temp;
            //corners
            temp = cornerPieces[cornerIndexes[0]];
            cornerPieces[cornerIndexes[0]] = cornerPieces[cornerIndexes[1]];
            cornerPieces[cornerIndexes[1]] = cornerPieces[cornerIndexes[2]];
            cornerPieces[cornerIndexes[2]] = cornerPieces[cornerIndexes[3]];
            cornerPieces[cornerIndexes[3]] = temp;
        }
        //rotate pieces
        edgePieces[edgeIndexes[0]].RotateTiles(side, prime);
        edgePieces[edgeIndexes[1]].RotateTiles(side, prime);
        edgePieces[edgeIndexes[2]].RotateTiles(side, prime);
        edgePieces[edgeIndexes[3]].RotateTiles(side, prime);
        cornerPieces[cornerIndexes[0]].RotateTiles(side, prime);
        cornerPieces[cornerIndexes[1]].RotateTiles(side, prime);
        cornerPieces[cornerIndexes[2]].RotateTiles(side, prime);
        cornerPieces[cornerIndexes[3]].RotateTiles(side, prime);

        if (rotationMade != null)
            rotationMade(side, prime);
    }

}

public enum SidesOfCube
{
    up, down,
    right, left,
    front, back
}

public enum Colors
{
    red,
    orange,
    white,
    yellow,
    blue,
    green
}