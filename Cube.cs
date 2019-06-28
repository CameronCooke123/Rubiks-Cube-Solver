using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;                                                                                                 
                                                                                                                   
public class Cube : MonoBehaviour                                                                                  
{
    //the cubies that are on each side in clockwise order
    readonly int[] centerIndexesMiddle = { 1, 0, 3, 5 };
    readonly int[] centerIndexesEquatorial = { 1, 2, 3, 4 };
    readonly int[] centerIndexesStanding = { 0, 4, 5, 2 };
    readonly int[] edgeIndexesUp = { 0, 1, 2, 3 };
    readonly int[] edgeIndexesDown = { 11, 10, 9, 8 };
    readonly int[] edgeIndexesRight = { 3, 6, 11, 7 };
    readonly int[] edgeIndexesLeft = { 1, 4, 9, 5 };
    readonly int[] edgeIndexesFront = { 0, 7, 8, 4 };
    readonly int[] edgeIndexesBack = { 2, 5, 10, 6 };
    readonly int[] edgeIndexesMiddle = { 0, 2, 10, 8 };
    readonly int[] edgeIndexesEquatorial = { 4, 5, 6, 7 };
    readonly int[] edgeIndexesStanding = { 1, 3, 11, 9 };
    readonly int[] cornerIndexesUp = { 0, 1, 2, 3 };
    readonly int[] cornerIndexesDown = { 7, 6, 5, 4 };
    readonly int[] cornerIndexesRight = { 3, 2, 6, 7 };
    readonly int[] cornerIndexesLeft = { 1, 0, 4, 5 };
    readonly int[] cornerIndexesFront = { 0, 3, 7, 4 };
    readonly int[] cornerIndexesBack = { 2, 1, 5, 6 };

    public delegate void RotationMade (SidesOfCube side, bool prime);
    public event RotationMade rotationMade;
    public delegate void RotationMade_Layer (MidLayersOfCube layer, bool prime);
    public event RotationMade_Layer rotationMade_Layer;
    public delegate void NewCubeGenerated ();
    public event NewCubeGenerated newCubeGenerated;

    //a cubie's position in the array will tell where on the cube it currently is - see key at bottom 
    public Cubie[] centerPieces;                                                                                   
    public Cubie[] edgePieces;                                                                                     
    public Cubie[] cornerPieces;

    public Side redSide, orangeSide, whiteSide, yellowSide, blueSide, greenSide;

    public void Start ()                                                                                           
    {                                                                                                             
        //print("Generating New Cube...");                                                                        
        GenerateNewCube();                                                                                        
        //print("New Cube Generated.");                                                                           
    }                                                                                                             
                                                                                                                  
    void GenerateNewCube ()                                                                                       
    {                                                                                                             
        redSide = new Side();
        orangeSide = new Side();
        whiteSide = new Side();
        yellowSide = new Side();
        blueSide = new Side();
        greenSide = new Side();

        centerPieces = GenerateCenterPieces();                                                                    
        edgePieces = GenerateEdgePieces();                                                                        
        cornerPieces = GenerateCornerPieces();                                                                    
        if (newCubeGenerated != null)                                                                             
            newCubeGenerated();                                                                                   
        WriteSidesToFile();       
    }                                                                                                              
                                                                                                                   
    Cubie[] GenerateCenterPieces ()
    {
        Cubie[] centers = new Cubie[6];
        for (int i = 0; i < 6; i++)
        {
            Tile[] tiles = new Tile[1];

            if (i == 0)
            {
                GenerateCenterPiece(ref tiles, ref centers[i], Colors.red);
                redSide.center = centers[i];
            }
            else if (i == 1)
            {
                GenerateCenterPiece(ref tiles, ref centers[i], Colors.blue);
                blueSide.center = centers[i];
            }
            else if (i == 2)
            {
                GenerateCenterPiece(ref tiles, ref centers[i], Colors.yellow);
                yellowSide.center = centers[i];
            }
            else if (i == 3)
            {
                GenerateCenterPiece(ref tiles, ref centers[i], Colors.green);
                greenSide.center = centers[i];
            }
            else if (i == 4)
            {
                GenerateCenterPiece(ref tiles, ref centers[i], Colors.white);
                whiteSide.center = centers[i];
            }
            else
            {
                GenerateCenterPiece(ref tiles, ref centers[i], Colors.orange);                
                orangeSide.center = centers[i];
            }
        }

        return centers;
    }
    void GenerateCenterPiece (ref Tile[] tiles, ref Cubie center, Colors col)
    {
        tiles[0] = new Tile(col, GetDefaultSide(col));
        center = new Cubie(1, tiles);
    }
    Cubie[] GenerateEdgePieces ()
    {
        Cubie[] edges = new Cubie[12];
        for (int i = 0; i < 12; i++)
        {
            Tile[] tiles = new Tile[2];

            if (i == 0)
            {
                GenerateEdge(ref tiles, ref edges[i], new Colors[] { Colors.red, Colors.blue });
                if (!redSide.edges.Contains(edges[i]))
                    redSide.edges.Add(edges[i]);
                if (!blueSide.edges.Contains(edges[i]))
                    blueSide.edges.Add(edges[i]);
            }
            else if (i == 1)
            {
                GenerateEdge(ref tiles, ref edges[i], new Colors[] { Colors.red, Colors.yellow });
                if (!redSide.edges.Contains(edges[i]))
                    redSide.edges.Add(edges[i]);
                if (!yellowSide.edges.Contains(edges[i]))
                    yellowSide.edges.Add(edges[i]);
            }
            else if (i == 2)
            {
                GenerateEdge(ref tiles, ref edges[i], new Colors[] { Colors.red, Colors.green });
                if (!redSide.edges.Contains(edges[i]))
                    redSide.edges.Add(edges[i]);
                if (!greenSide.edges.Contains(edges[i]))
                    greenSide.edges.Add(edges[i]);
            }
            else if (i == 3)
            {
                GenerateEdge(ref tiles, ref edges[i], new Colors[] { Colors.red, Colors.white });
                if (!redSide.edges.Contains(edges[i]))
                    redSide.edges.Add(edges[i]);
                if (!whiteSide.edges.Contains(edges[i]))
                    whiteSide.edges.Add(edges[i]);
            }
            else if (i == 4)
            {
                GenerateEdge(ref tiles, ref edges[i], new Colors[] { Colors.blue, Colors.yellow });
                if (!blueSide.edges.Contains(edges[i]))
                    blueSide.edges.Add(edges[i]);
                if (!yellowSide.edges.Contains(edges[i]))
                    yellowSide.edges.Add(edges[i]);
            }
            else if (i == 5)
            {
                GenerateEdge(ref tiles, ref edges[i], new Colors[] { Colors.yellow, Colors.green });
                if (!yellowSide.edges.Contains(edges[i]))
                    yellowSide.edges.Add(edges[i]);
                if (!greenSide.edges.Contains(edges[i]))
                    greenSide.edges.Add(edges[i]);
            }
            else if (i == 6)
            {
                GenerateEdge(ref tiles, ref edges[i], new Colors[] { Colors.green, Colors.white });
                if (!greenSide.edges.Contains(edges[i]))
                    greenSide.edges.Add(edges[i]);
                if (!whiteSide.edges.Contains(edges[i]))
                    whiteSide.edges.Add(edges[i]);
            }
            else if (i == 7)
            {
                GenerateEdge(ref tiles, ref edges[i], new Colors[] { Colors.white, Colors.blue });
                if (!whiteSide.edges.Contains(edges[i]))
                    whiteSide.edges.Add(edges[i]);
                if (!blueSide.edges.Contains(edges[i]))
                    blueSide.edges.Add(edges[i]);
            }
            else if (i == 8)
            {
                GenerateEdge(ref tiles, ref edges[i], new Colors[] { Colors.orange, Colors.blue });
                if (!orangeSide.edges.Contains(edges[i]))
                    orangeSide.edges.Add(edges[i]);
                if (!blueSide.edges.Contains(edges[i]))
                    blueSide.edges.Add(edges[i]);
            }
            else if (i == 9)
            {
                GenerateEdge(ref tiles, ref edges[i], new Colors[] { Colors.orange, Colors.yellow });
                if (!orangeSide.edges.Contains(edges[i]))
                    orangeSide.edges.Add(edges[i]);
                if (!yellowSide.edges.Contains(edges[i]))
                    yellowSide.edges.Add(edges[i]);
            }
            else if (i == 10)
            {
                GenerateEdge(ref tiles, ref edges[i], new Colors[] { Colors.orange, Colors.green });
                if (!orangeSide.edges.Contains(edges[i]))
                    orangeSide.edges.Add(edges[i]);
                if (!greenSide.edges.Contains(edges[i]))
                    greenSide.edges.Add(edges[i]);
            }
            else
            {
                GenerateEdge(ref tiles, ref edges[i], new Colors[] { Colors.orange, Colors.white });
                if (!orangeSide.edges.Contains(edges[i]))
                    orangeSide.edges.Add(edges[i]);
                if (!whiteSide.edges.Contains(edges[i]))
                    whiteSide.edges.Add(edges[i]);
            }
        }

        return edges;
    }
    void GenerateEdge (ref Tile[] tiles, ref Cubie edge, Colors[] cols)
    {
        for (int i = 0; i < 2; i++)
        {
            tiles[i] = new Tile(cols[i], GetDefaultSide(cols[i]));
        }
        edge = new Cubie(2, tiles);
    }
    Cubie[] GenerateCornerPieces ()
    {
        //the tiles are arranged such that as the index increases, the tiles' sides move clockwise
        //(example: for the blue, red, and white corner, the tiles array will have indexes: 0 = red, 1 = white, 2 = blue)
        Cubie[] corners = new Cubie[8];
        for (int i = 0; i < 8; i++)
        {
            Tile[] tiles = new Tile[3];

            if (i == 0)
            {
                GenerateCorner(ref tiles, ref corners[i], new Colors[] { Colors.red, Colors.blue, Colors.yellow });
                if (!redSide.corners.Contains(corners[i]))
                    redSide.corners.Add(corners[i]);
                if (!blueSide.corners.Contains(corners[i]))
                    blueSide.corners.Add(corners[i]);
                if (!yellowSide.corners.Contains(corners[i]))
                    yellowSide.corners.Add(corners[i]);
            }
            else if (i == 1)
            {
                GenerateCorner(ref tiles, ref corners[i], new Colors[] { Colors.red, Colors.yellow, Colors.green });
                if (!redSide.corners.Contains(corners[i]))
                    redSide.corners.Add(corners[i]);
                if (!yellowSide.corners.Contains(corners[i]))
                    yellowSide.corners.Add(corners[i]);
                if (!greenSide.corners.Contains(corners[i]))
                    greenSide.corners.Add(corners[i]);
            }
            else if (i == 2)
            {
                GenerateCorner(ref tiles, ref corners[i], new Colors[] { Colors.red, Colors.green, Colors.white });
                if (!redSide.corners.Contains(corners[i]))
                    redSide.corners.Add(corners[i]);
                if (!greenSide.corners.Contains(corners[i]))
                    greenSide.corners.Add(corners[i]);
                if (!whiteSide.corners.Contains(corners[i]))
                    whiteSide.corners.Add(corners[i]);
            }
            else if (i == 3)
            {
                GenerateCorner(ref tiles, ref corners[i], new Colors[] { Colors.red, Colors.white, Colors.blue });
                if (!redSide.corners.Contains(corners[i]))
                    redSide.corners.Add(corners[i]);
                if (!whiteSide.corners.Contains(corners[i]))
                    whiteSide.corners.Add(corners[i]);
                if (!greenSide.corners.Contains(corners[i]))
                    greenSide.corners.Add(corners[i]);
            }
            else if (i == 4)
            {
                GenerateCorner(ref tiles, ref corners[i], new Colors[] { Colors.orange, Colors.yellow, Colors.blue });
                if (!orangeSide.corners.Contains(corners[i]))
                    orangeSide.corners.Add(corners[i]);
                if (!yellowSide.corners.Contains(corners[i]))
                    yellowSide.corners.Add(corners[i]);
                if (!blueSide.corners.Contains(corners[i]))
                    blueSide.corners.Add(corners[i]);
            }
            else if (i == 5)
            {
                GenerateCorner(ref tiles, ref corners[i], new Colors[] { Colors.orange, Colors.green, Colors.yellow });
                if (!orangeSide.corners.Contains(corners[i]))
                    orangeSide.corners.Add(corners[i]);
                if (!greenSide.corners.Contains(corners[i]))
                    greenSide.corners.Add(corners[i]);
                if (!yellowSide.corners.Contains(corners[i]))
                    yellowSide.corners.Add(corners[i]);
            }
            else if (i == 6)
            {
                GenerateCorner(ref tiles, ref corners[i], new Colors[] { Colors.orange, Colors.white, Colors.green });
                if (!orangeSide.corners.Contains(corners[i]))
                    orangeSide.corners.Add(corners[i]);
                if (!whiteSide.corners.Contains(corners[i]))
                    whiteSide.corners.Add(corners[i]);
                if (!greenSide.corners.Contains(corners[i]))
                    greenSide.corners.Add(corners[i]);
            }
            else
            {
                GenerateCorner(ref tiles, ref corners[i], new Colors[] { Colors.orange, Colors.blue, Colors.white });
                if (!orangeSide.corners.Contains(corners[i]))
                    orangeSide.corners.Add(corners[i]);
                if (!blueSide.corners.Contains(corners[i]))
                    blueSide.corners.Add(corners[i]);
                if (!whiteSide.corners.Contains(corners[i]))
                    whiteSide.corners.Add(corners[i]);
            }
        }

        return corners;
    }
    void GenerateCorner (ref Tile[] tiles, ref Cubie corner, Colors[] cols)
    {
        for (int i = 0; i < 3; i++)
        {
            tiles[i] = new Tile(cols[i], GetDefaultSide(cols[i]));
        }
        corner = new Cubie(3, tiles);
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

    public Cubie GetCenterFromColor (Colors color)
    {
        for (int i = 1; i < 6; i++)
        {
            if (centerPieces[i].tiles[0].color == color)
                return centerPieces[i];
        }
        return centerPieces[0];
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
        CyclePieces(null, edgeIndexes, cornerIndexes, prime);
        //rotate pieces
        for (int i = 0; i < 4; i++)
        {
            edgePieces[edgeIndexes[i]].RotateTiles(side, prime);
            cornerPieces[cornerIndexes[i]].RotateTiles(side, prime);
        }

        if (rotationMade != null)
            rotationMade(side, prime);
    }
    public void RotateMidLayer (MidLayersOfCube layer, bool prime)
    {
        int[] centerIndexes, edgeIndexes;
        //set the index arrays to correspond to the layer that's being rotated
        switch (layer)
        {
            case MidLayersOfCube.Middle:
                centerIndexes = centerIndexesMiddle;
                edgeIndexes = edgeIndexesMiddle;
                break;
            case MidLayersOfCube.Equatorial:
                centerIndexes = centerIndexesEquatorial;
                edgeIndexes = edgeIndexesEquatorial;
                prime = !prime;
                break;
            default: // standing
                centerIndexes = centerIndexesStanding;
                edgeIndexes = edgeIndexesStanding;
                break;
        }
        //move the pieces
        CyclePieces(centerIndexes, edgeIndexes, null, prime);
        //rotate the pieces
        for (int i = 0; i < 4; i++)
        {
            centerPieces[centerIndexes[i]].RotateTiles(layer, prime);
            edgePieces[edgeIndexes[i]].RotateTiles(layer, prime);
        }

        if (rotationMade_Layer != null)
            rotationMade_Layer(layer, prime);

        WriteSidesToFile();
    }
    void CyclePieces (int[] centerIndexes, int[] edgeIndexes, int[] cornerIndexes, bool prime)
    {
        if (!prime)
        {
            Cubie temp;
            //centers
            if (centerIndexes != null && centerIndexes.Length > 0)
            {
                temp = centerPieces[centerIndexes[0]];
                centerPieces[centerIndexes[0]] = centerPieces[centerIndexes[3]];
                centerPieces[centerIndexes[3]] = centerPieces[centerIndexes[2]];
                centerPieces[centerIndexes[2]] = centerPieces[centerIndexes[1]];
                centerPieces[centerIndexes[1]] = temp;
            }
            //edges
            if (edgeIndexes != null && edgeIndexes.Length > 0)
            {
                temp = edgePieces[edgeIndexes[0]];
                edgePieces[edgeIndexes[0]] = edgePieces[edgeIndexes[3]];
                edgePieces[edgeIndexes[3]] = edgePieces[edgeIndexes[2]];
                edgePieces[edgeIndexes[2]] = edgePieces[edgeIndexes[1]];
                edgePieces[edgeIndexes[1]] = temp;
            }
            //corners
            if (cornerIndexes != null && cornerIndexes.Length > 0)
            {
                temp = cornerPieces[cornerIndexes[0]];
                cornerPieces[cornerIndexes[0]] = cornerPieces[cornerIndexes[3]];
                cornerPieces[cornerIndexes[3]] = cornerPieces[cornerIndexes[2]];
                cornerPieces[cornerIndexes[2]] = cornerPieces[cornerIndexes[1]];
                cornerPieces[cornerIndexes[1]] = temp;
            }
        }
        else
        {
            Cubie temp;
            //centers
            if (centerIndexes != null && centerIndexes.Length > 0)
            {
                temp = centerPieces[centerIndexes[0]];
                centerPieces[centerIndexes[0]] = centerPieces[centerIndexes[1]];
                centerPieces[centerIndexes[1]] = centerPieces[centerIndexes[2]];
                centerPieces[centerIndexes[2]] = centerPieces[centerIndexes[3]];
                centerPieces[centerIndexes[3]] = temp;
            }
            //edges
            if (edgeIndexes != null && edgeIndexes.Length > 0)
            {
                temp = edgePieces[edgeIndexes[0]];
                edgePieces[edgeIndexes[0]] = edgePieces[edgeIndexes[1]];
                edgePieces[edgeIndexes[1]] = edgePieces[edgeIndexes[2]];
                edgePieces[edgeIndexes[2]] = edgePieces[edgeIndexes[3]];
                edgePieces[edgeIndexes[3]] = temp;
            }
            //corners
            if (cornerIndexes != null && cornerIndexes.Length > 0)
            {
                temp = cornerPieces[cornerIndexes[0]];
                cornerPieces[cornerIndexes[0]] = cornerPieces[cornerIndexes[1]];
                cornerPieces[cornerIndexes[1]] = cornerPieces[cornerIndexes[2]];
                cornerPieces[cornerIndexes[2]] = cornerPieces[cornerIndexes[3]];
                cornerPieces[cornerIndexes[3]] = temp;
            }
        }
    }
}

public enum SidesOfCube
{
    up, down,
    right, left,
    front, back
}
public enum MidLayersOfCube
{
    Middle,
    Equatorial,
    Standing
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

//  Cubie Position Key
//  Top down view
//           T = center, E = edge, C = corner
//
//                  C1 E2 C2
//      Top Layer   E1 T0 E3
//                  C0 E0 C3
//
//                  E5 T3 E6
//      Mid Layer   T2    T4
//                  E4 T1 E7
//
//                  C5  E10 C6
//      Bot Layer   E9  T5  E11
//                  C4  E8  C7