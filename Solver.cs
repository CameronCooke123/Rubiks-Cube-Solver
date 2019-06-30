using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solver : MonoBehaviour
{
    public bool pauseAfterMove;
    public bool solve;
    public bool scramble;
    public int numTurns = 15;

    Cube cube;
    CubeDisplay cubeDisp;
    bool scrambling;
    bool solving;
    Coroutine currentStep;

    void Start ()
    {
        cube = GetComponent<Cube>();
        cubeDisp = GetComponent<CubeDisplay>();
    }

    void Update ()
    {
        if (scramble && !scrambling && !solving)
        {
            scramble = false;
            Scramble();
        }
        else if (solve && !scrambling && !solving)
        {
            solve = false;
            Solve();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            StopCoroutine(currentStep);
            scrambling = false;
            solving = false;
        }
    }

    void Solve ()
    {
        currentStep = StartCoroutine(Cross());
    }

    IEnumerator Move (SidesOfCube side, bool prime)
    {
        cube.RotateSide(side, prime);
        while (cubeDisp.rotating)
            yield return null;
        if (pauseAfterMove)
            Debug.Break();
    }
    IEnumerator Move (MidLayersOfCube layer, bool prime)
    {
        cube.RotateMidLayer(layer, prime);
        while (cubeDisp.rotating)
            yield return null;
        if (pauseAfterMove)
            Debug.Break();
    }
    IEnumerator Algorithm (string alg)
    {
        string[] moves = alg.Split(new char[] { ' ' });
        for (int i = 0; i < moves.Length; i++)
        {
            switch (moves[i])
            {
                case "U":
                    yield return StartCoroutine(Move(SidesOfCube.up, false));
                    break;
                case "U'":
                    yield return StartCoroutine(Move(SidesOfCube.up, true));
                    break;
                case "D":
                    yield return StartCoroutine(Move(SidesOfCube.down, false));
                    break;
                case "D'":
                    yield return StartCoroutine(Move(SidesOfCube.down, true));
                    break;
                case "R":
                    yield return StartCoroutine(Move(SidesOfCube.right, false));
                    break;
                case "R'":
                    yield return StartCoroutine(Move(SidesOfCube.right, true));
                    break;
                case "L":
                    yield return StartCoroutine(Move(SidesOfCube.left, false));
                    break;
                case "L'":
                    yield return StartCoroutine(Move(SidesOfCube.left, true));
                    break;
                case "F":
                    yield return StartCoroutine(Move(SidesOfCube.front, false));
                    break;
                case "F'":
                    yield return StartCoroutine(Move(SidesOfCube.front, true));
                    break;
                case "B":
                    yield return StartCoroutine(Move(SidesOfCube.back, false));
                    break;
                case "B'":
                    yield return StartCoroutine(Move(SidesOfCube.back, true));
                    break;
                case "M":
                    yield return StartCoroutine(Move(MidLayersOfCube.Middle, false));
                    break;
                case "M'":
                    yield return StartCoroutine(Move(MidLayersOfCube.Middle, true));
                    break;
                case "E":
                    yield return StartCoroutine(Move(MidLayersOfCube.Equatorial, false));
                    break;
                case "E'":
                    yield return StartCoroutine(Move(MidLayersOfCube.Equatorial, true));
                    break;
                case "S":
                    yield return StartCoroutine(Move(MidLayersOfCube.Standing, false));
                    break;
                case "S'":
                    yield return StartCoroutine(Move(MidLayersOfCube.Standing, true));
                    break;
            }
        }
    }

    //todo: implement Cubie.GetTile() and Cubie.GetTileIndex() everywhere it belongs.
    IEnumerator Cross ()
    {
        solving = true;
        //start with white side

        //move white center to the down side
        Tile tile = cube.whiteSide.center.tiles[0];
        if (tile.sideOfCube == SidesOfCube.front)
        {
            while (tile.sideOfCube != SidesOfCube.down)
                yield return StartCoroutine(Move(MidLayersOfCube.Middle, true));
        }
        else if (tile.sideOfCube == SidesOfCube.back)
        {
            while (tile.sideOfCube != SidesOfCube.down)
                yield return StartCoroutine(Move(MidLayersOfCube.Middle, false));
        }
        else if (tile.sideOfCube == SidesOfCube.right)
        {
            while (tile.sideOfCube != SidesOfCube.down)
                yield return StartCoroutine(Move(MidLayersOfCube.Standing, false));
        }
        else
        {
            while (tile.sideOfCube != SidesOfCube.down)
                yield return StartCoroutine(Move(MidLayersOfCube.Standing, true));
        }

        //find white edges
        Tile[] whiteTiles = new Tile[4];
        Tile[] otherTiles = new Tile[4];
        for (int i = 0; i < 4; i++)
        {
            if (cube.whiteSide.edges[i].tiles[0].color == Colors.white)
            {
                whiteTiles[i] = cube.whiteSide.edges[i].tiles[0];
                otherTiles[i] = cube.whiteSide.edges[i].tiles[1];
            }
            else
            {
                whiteTiles[i] = cube.whiteSide.edges[i].tiles[1];
                otherTiles[i] = cube.whiteSide.edges[i].tiles[0];
            }
        }

        //line up the other side of each edge piece with its color's center piece
        for (int i = 0; i < 4; i++)
        {
            //Debug.Log(otherTiles[i].color);
            //check if the cubie is already in its proper place...
            bool cubieInPlace = whiteTiles[i].sideOfCube == SidesOfCube.down && otherTiles[i].sideOfCube == cube.GetCenterFromColor(otherTiles[i].color).tiles[0].sideOfCube;
            if (cubieInPlace)
                continue;

            //move the edge piece to the top layer and orient it properly
            if (otherTiles[i].sideOfCube != SidesOfCube.up && otherTiles[i].sideOfCube != SidesOfCube.down)
            {
                int moveCount = 0;
                SidesOfCube sideToMove = otherTiles[i].sideOfCube;
                //move the cubie up to the top layer
                while (whiteTiles[i].sideOfCube != SidesOfCube.up)
                {
                    moveCount++;
                    yield return StartCoroutine(Move(sideToMove, false));
                }
                //move the edge out of the way, then undo the moves on sideToMove
                yield return StartCoroutine(Move(SidesOfCube.up, false));
                while (moveCount > 0)
                {
                    yield return StartCoroutine(Move(sideToMove, true));
                    moveCount--;
                }
            }
            else
            {
                int moveCount = 0;
                SidesOfCube sideToMove = whiteTiles[i].sideOfCube;
                //move the cubie up to the top layer
                while (otherTiles[i].sideOfCube != SidesOfCube.up)
                {
                    moveCount++;
                    yield return StartCoroutine(Move(sideToMove, false));
                }
                //move the edge out of the way, then undo the moves on sideToMove
                if (moveCount > 0)
                    yield return StartCoroutine(Move(SidesOfCube.up, false));
                while (moveCount > 0)
                {
                    yield return StartCoroutine(Move(sideToMove, true));
                    moveCount--;
                }
                //set up cubie
                while (whiteTiles[i].sideOfCube != SidesOfCube.front)
                    yield return StartCoroutine(Move(SidesOfCube.up, false));
                //flip the cubie
                yield return StartCoroutine(Algorithm("F R U' R' F'"));
            }

            //find the center cubie that is the same color as otherTile
            SidesOfCube otherColorSide = cube.GetCenterFromColor(otherTiles[i].color).tiles[0].sideOfCube;

            //move the properly oriented cubie to its side on the cube
            while (otherTiles[i].sideOfCube != otherColorSide)
            {
                //print("otherTile color: " + otherTiles[i].color + ", whiteTile side: " + whiteTiles[i].sideOfCube + ", otherTile side: " + otherTiles[i].sideOfCube + ", otherColorSide: " + otherColorSide);
                yield return StartCoroutine(Move(SidesOfCube.up, false));
            }

            //move the edge piece down to its proper spot
            yield return StartCoroutine(Move(otherColorSide, false));
            yield return StartCoroutine(Move(otherColorSide, false));
        }
        yield return null;
        currentStep = StartCoroutine(FirstTwoLayers());
    }
    /*  OLD NOTES
         1. whichever tile is NOT the white one, rotate that side (!prime) until the white side is facing up. (this is very inefficient obviously, but it will work for now)
            a. rotate the up side once, and then undo the rotations on the first side.
                1. store the number of moves. then when undoing them, if the number is greater than 2, do the same rotation until the total is 4. 
                                                                      else, do the opposite rotation until the total is 0.
         2. rotate up side until the non-white side is the same color as the center piece on that side
         3. rotate that side twice

         1. special case for when the piece is on the up layer but upside down, do a flipping algorithm
         */
    IEnumerator FirstTwoLayers ()
    {
        //CORNERS
        //find all white corners (just to easily access them)
        Tile whiteTile = null;
        Tile secondTile = null;//the tile next to the white one in the clockwise direction
        Tile thirdTile = null;//the tile next to the white one in the counterclockwise direction

        //solve corners
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (cube.whiteSide.corners[i].tiles[j].color == Colors.white)
                {
                    whiteTile = cube.whiteSide.corners[i].tiles[j];
                    secondTile = cube.whiteSide.corners[i].tiles[(j + 1) % 3];
                    thirdTile = cube.whiteSide.corners[i].tiles[(j + 2) % 3];
                    break;
                }
            }

            //check if the corner piece is already in place
            bool inPlace = whiteTile.sideOfCube == SidesOfCube.down &&
                           secondTile.sideOfCube == cube.GetCenterFromColor(secondTile.color).tiles[0].sideOfCube &&
                           thirdTile.sideOfCube == cube.GetCenterFromColor(thirdTile.color).tiles[0].sideOfCube;
            if (inPlace)
                continue;

            //Check if the piece is in the bottom layer.
            Tile[] sidewaysTiles = new Tile[2];//this will be used to know which tiles on the corner are not vertical to check that the piece is aligned properly
            if (whiteTile.sideOfCube == SidesOfCube.down)
            {
                sidewaysTiles[0] = secondTile;
                sidewaysTiles[1] = thirdTile;
            }
            else if (secondTile.sideOfCube == SidesOfCube.down)
            {
                sidewaysTiles[0] = thirdTile;
                sidewaysTiles[1] = whiteTile;
            }
            else if (thirdTile.sideOfCube == SidesOfCube.down)
            {
                sidewaysTiles[0] = whiteTile;
                sidewaysTiles[1] = secondTile;
            }

            if (sidewaysTiles[0] != null && sidewaysTiles[1] != null)//will be true only if the cubie is in the bottom layer
            {
                //rotate the down side so that the corner is in the proper place
                int moveCount = 0;
                while (sidewaysTiles[0].sideOfCube != SidesOfCube.front || sidewaysTiles[1].sideOfCube != SidesOfCube.right)
                {
                    yield return StartCoroutine(Move(SidesOfCube.down, false));
                    moveCount++;
                }

                //do algorithm
                yield return StartCoroutine(Algorithm("R U R'"));

                while (moveCount > 0)
                {
                    yield return StartCoroutine(Move(SidesOfCube.down, true));
                    moveCount--;
                }
            }

            //rotate up side until the corner piece is in the proper spot
            Tile secondSide = cube.GetCenterFromColor(secondTile.color).tiles[0];
            Tile thirdSide = cube.GetCenterFromColor(thirdTile.color).tiles[0];

            while (true)
            {
                if ((whiteTile.sideOfCube == secondSide.sideOfCube || secondTile.sideOfCube == secondSide.sideOfCube || thirdTile.sideOfCube == secondSide.sideOfCube) &&
                    (whiteTile.sideOfCube == thirdSide.sideOfCube || secondTile.sideOfCube == thirdSide.sideOfCube || thirdTile.sideOfCube == thirdSide.sideOfCube))
                {
                    break;
                }

                yield return StartCoroutine(Move(SidesOfCube.up, false));
            }
            //rotate whole cube until it's in the correct orientation
            while (secondSide.sideOfCube != SidesOfCube.front || thirdSide.sideOfCube != SidesOfCube.right)
            {
                yield return StartCoroutine(Move(SidesOfCube.up, true));
                yield return StartCoroutine(Move(MidLayersOfCube.Equatorial, false));
                yield return StartCoroutine(Move(SidesOfCube.down, false));
            }

            //perform algorithm until cube is in final spot
            while (whiteTile.sideOfCube != SidesOfCube.down)
            {
                yield return StartCoroutine(Algorithm("R U R' U'"));
            }
        }

        //EDGES
        //find all non-white, non-yellow edges
        Cubie[] nonWYEdges = new Cubie[4];
        int k = 0;
        //search blue edges
        foreach (Cubie edge in cube.blueSide.edges)
        {
            if (edge.tiles[0].color == Colors.yellow || edge.tiles[0].color == Colors.white ||
                edge.tiles[1].color == Colors.yellow || edge.tiles[1].color == Colors.white)
                continue;

            nonWYEdges[k] = edge;
            k++;
        }
        //search green edges
        foreach (Cubie edge in cube.greenSide.edges)
        {
            if (edge.tiles[0].color == Colors.yellow || edge.tiles[0].color == Colors.white ||
                edge.tiles[1].color == Colors.yellow || edge.tiles[1].color == Colors.white)
                continue;

            nonWYEdges[k] = edge;
            k++;
        }

        //solve edges
        for (int i = 0; i < 4; i++)
        {
            Colors color0 = nonWYEdges[i].tiles[0].color;
            Colors color1 = nonWYEdges[i].tiles[1].color;
            //check if edge is already in correct spot
            if (nonWYEdges[i].tiles[0].sideOfCube == cube.GetCenterFromColor(color0).tiles[0].sideOfCube &&
                nonWYEdges[i].tiles[1].sideOfCube == cube.GetCenterFromColor(color1).tiles[0].sideOfCube)
                continue;

            //if edge is not in top layer
            if (nonWYEdges[i].tiles[0].sideOfCube != SidesOfCube.up && nonWYEdges[i].tiles[1].sideOfCube != SidesOfCube.up)
            {
                //rotate whole cube into position
                while (nonWYEdges[i] != cube.edgePieces[7])
                {
                    yield return StartCoroutine(Move(SidesOfCube.up, true));
                    yield return StartCoroutine(Move(MidLayersOfCube.Equatorial, false));
                    yield return StartCoroutine(Move(SidesOfCube.down, false));
                }

                //find random yellow piece from top layer and move it into position
                while (!cube.yellowSide.edges.Contains(cube.edgePieces[0]))
                {
                    yield return StartCoroutine(Move(SidesOfCube.up, true));
                }

                //perform algorithm to put yellow edge in target place
                yield return StartCoroutine(Algorithm("U R U' R' U' F' U F"));
            }

            //move top layer until edge and center colors match
            Tile topTile = (nonWYEdges[i].tiles[0].sideOfCube == SidesOfCube.up) ? nonWYEdges[i].tiles[0] : nonWYEdges[i].tiles[1];
            Tile sideTile = (nonWYEdges[i].tiles[0].sideOfCube == SidesOfCube.up) ? nonWYEdges[i].tiles[1] : nonWYEdges[i].tiles[0];
            while (sideTile.sideOfCube != cube.GetCenterFromColor(sideTile.color).tiles[0].sideOfCube)
            {
                yield return StartCoroutine(Move(SidesOfCube.up, true));
            }

            //rotate whole cube into position
            while (nonWYEdges[i] != cube.edgePieces[0])
            {
                yield return StartCoroutine(Move(SidesOfCube.up, true));
                yield return StartCoroutine(Move(MidLayersOfCube.Equatorial, false));
                yield return StartCoroutine(Move(SidesOfCube.down, false));
            }

            //perform algorithm
            if (cube.GetCenterFromColor(topTile.color).tiles[0].sideOfCube == SidesOfCube.right)
                yield return StartCoroutine(Algorithm("U R U' R' U' F' U F"));
            else
                yield return StartCoroutine(Algorithm("U' L' U L U F U' F'"));
        }

        yield return null;
        currentStep = StartCoroutine(OrientLastLayer());
    }
    IEnumerator OrientLastLayer ()
    {
        //EDGES
        //find out which yellow edges are facing up
        List<int> yelTilesUpIndexes = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            int yellowIndex = (cube.edgePieces[i].tiles[0].color == Colors.yellow) ? 0 : 1;
            if (cube.edgePieces[i].tiles[yellowIndex].sideOfCube == SidesOfCube.up)
            {
                yelTilesUpIndexes.Add(i);//indexes are added to the list in clockwise order, starting from the front side (see key)
            }
        }

        //get top cross
        if (yelTilesUpIndexes.Count == 0)
        {
            yield return StartCoroutine(Algorithm("F R U R' U' S R U R' U' F' S'"));
        }
        else if (yelTilesUpIndexes.Count == 2)
        {
            //LINE
            if (yelTilesUpIndexes[0] % 2 == yelTilesUpIndexes[1] % 2)
            {
                //align pieces
                if (yelTilesUpIndexes[0] != 1 && yelTilesUpIndexes[0] != 3)
                    yield return StartCoroutine(Move(SidesOfCube.up, true));

                //get top cross
                yield return StartCoroutine(Algorithm("F R U R' U' F'"));
            }
            //L
            else
            {
                //align pieces
                while ((yelTilesUpIndexes[0] == 2 || yelTilesUpIndexes[0] == 1) ||
                       (yelTilesUpIndexes[1] == 2 || yelTilesUpIndexes[1] == 1))
                {
                    if (yelTilesUpIndexes[0] < 2)
                    {
                        yield return StartCoroutine(Move(SidesOfCube.up, true));
                        yelTilesUpIndexes[0] = (yelTilesUpIndexes[0] + 3) % 4;//this is kind of an obnoxious looking formula. It just means:
                        yelTilesUpIndexes[1] = (yelTilesUpIndexes[1] + 3) % 4;//subtract 1. C# modulus is weird with negative numbers for some reason.
                    }
                    else
                    {
                        yield return StartCoroutine(Move(SidesOfCube.up, false));
                        yelTilesUpIndexes[0] = (yelTilesUpIndexes[0] + 1) % 4;
                        yelTilesUpIndexes[1] = (yelTilesUpIndexes[1] + 1) % 4;
                    }
                }

                //get top cross
                yield return StartCoroutine(Algorithm("F S R U R' U' F' S'"));
            }
        }

        //CORNERS
        //get the yellow tiles for each corner piece
        Tile[] yellowTiles = new Tile[4];
        List<int> facingUpIndexes = new List<int>();
        List<Tile> notFacingUpYelTiles = new List<Tile>();
        for (int i = 0; i < 4; i++)
        {
            yellowTiles[i] = cube.cornerPieces[i].GetTile(Colors.yellow);
            if (yellowTiles[i].sideOfCube == SidesOfCube.up)
            {
                facingUpIndexes.Add(i);
            }
            else
            {
                notFacingUpYelTiles.Add(yellowTiles[i]);
            }
        }

        //find case
        if (facingUpIndexes.Count == 0)
        {
            //H
            if ((yellowTiles[0].sideOfCube == yellowTiles[1].sideOfCube && yellowTiles[2].sideOfCube == yellowTiles[3].sideOfCube) ||
                (yellowTiles[0].sideOfCube == yellowTiles[3].sideOfCube && yellowTiles[1].sideOfCube == yellowTiles[2].sideOfCube))
            {
                //align up side if necessary
                if (yellowTiles[0].sideOfCube == SidesOfCube.left)
                    yield return StartCoroutine(Move(SidesOfCube.up, false));

                //perform algorithm
                yield return StartCoroutine(Algorithm("F R U R' U' R U R' U' R U R' U' F'"));
            }
            //pi
            else
            {
                //align up side
                //find the 2 yellow tiles that are facing the same direction
                for (int i = 0; i < 4; i++)
                {
                    if (yellowTiles[i].sideOfCube == yellowTiles[(i+1)%4].sideOfCube)
                    {
                        //once found, rotate the up side until they're on the left
                        if (yellowTiles[i].sideOfCube == SidesOfCube.left)
                            break;

                        if (yellowTiles[i].sideOfCube == SidesOfCube.back)
                            yield return StartCoroutine(Move(SidesOfCube.up, true));
                        else
                        {
                            if (yellowTiles[i].sideOfCube == SidesOfCube.right)
                                yield return StartCoroutine(Move(SidesOfCube.up, false));
                            yield return StartCoroutine(Move(SidesOfCube.up, false));
                        }
                        break;
                    }
                }

                //perform algorithm
                yield return StartCoroutine(Algorithm("R U' U' R R U' R R U' R R U' U' R"));
            }
        }
        else if (facingUpIndexes.Count == 1)
        {
            //find the tile that is facing up
            int index = facingUpIndexes[0];
            int nextTileIndex = (cube.cornerPieces[index].GetTileIndex(Colors.yellow) + 1) % 3;//the tile on corner i Going clockwise from the yellow
            int prevCubieIndex = (index + 3) % 4;//the next corner piece going counterclockwise, subtract 1 without going negative
            if (yellowTiles[prevCubieIndex].sideOfCube == cube.cornerPieces[index].tiles[nextTileIndex].sideOfCube)
            {
                //sune
                //rotate top layer into position
                if (index == 1)
                    yield return StartCoroutine(Move(SidesOfCube.up, true));
                else
                {
                    if (index == 2)
                        yield return StartCoroutine(Move(SidesOfCube.up, false));
                    if (index != 0)
                        yield return StartCoroutine(Move(SidesOfCube.up, false));
                }

                //perform algorithm
                yield return StartCoroutine(Algorithm("R U R' U R U U R'"));
            }
            else
            {
                //anti sune
                //rotate top layer into position
                if (index == 1)
                    yield return StartCoroutine(Move(SidesOfCube.up, false));
                else
                {
                    if (index == 0)
                        yield return StartCoroutine(Move(SidesOfCube.up, true));
                    if (index != 2)
                        yield return StartCoroutine(Move(SidesOfCube.up, true));
                }

                //perform algorithm
                yield return StartCoroutine(Algorithm("R U' U' R' U' R U' R'"));
            }
        }
        else if (facingUpIndexes.Count == 2)
        {
            if (facingUpIndexes[1] - facingUpIndexes[0] == 2)
            {
                //bowtie
                //rotate up side into position
                if (facingUpIndexes[0] == 0)
                {
                    if (notFacingUpYelTiles[1].sideOfCube != SidesOfCube.front)
                    {
                        yield return StartCoroutine(Move(SidesOfCube.up, false));
                        yield return StartCoroutine(Move(SidesOfCube.up, false));
                    }
                }
                else
                {
                    if (notFacingUpYelTiles[0].sideOfCube == SidesOfCube.left)
                        yield return StartCoroutine(Move(SidesOfCube.up, true));
                    else
                        yield return StartCoroutine(Move(SidesOfCube.up, false));
                }

                //perform algorithm
                yield return StartCoroutine(Algorithm("F' R M U R' U' R' M' F R"));
            }
            else if (notFacingUpYelTiles[0].sideOfCube == notFacingUpYelTiles[1].sideOfCube)
            {
                //headlights
                //rotate up side into position
                if (notFacingUpYelTiles[0].sideOfCube == SidesOfCube.left)
                    yield return StartCoroutine(Move(SidesOfCube.up, true));
                else if (notFacingUpYelTiles[0].sideOfCube == SidesOfCube.back)
                {
                    yield return StartCoroutine(Move(SidesOfCube.up, false));
                    yield return StartCoroutine(Move(SidesOfCube.up, false));
                }
                else if (notFacingUpYelTiles[0].sideOfCube == SidesOfCube.right)
                    yield return StartCoroutine(Move(SidesOfCube.up, false));

                //perform algorithm
                yield return StartCoroutine(Algorithm("R R D R' U U R D' R' U U R'"));
            }
            else
            {
                //T
                //rotate up side into position
                if (facingUpIndexes[0] == 0)
                {
                    if (facingUpIndexes[1] == 1)
                        yield return StartCoroutine(Move(SidesOfCube.up, true));
                    yield return StartCoroutine(Move(SidesOfCube.up, true));
                }
                else if (facingUpIndexes[0] == 1)
                {
                    yield return StartCoroutine(Move(SidesOfCube.up, false));
                }

                //perform algorithm
                yield return StartCoroutine(Algorithm("R M U R' U' R' M' F R F'"));
            }
        }

        currentStep = StartCoroutine(PermutateLastLayer());
    }
    IEnumerator PermutateLastLayer ()
    {
        //CORNERS
        //check for matching adjacent corner tiles
        Tile lastMatch = null;
        int matches = 0;
        int cubieIndex = 0, nextCubieIndex = 0, cubieTileCheckIndex = 0, nextCubieTileCheckIndex = 0;
        for (int i = 0; i < 4; i++)
        {
            cubieIndex = i;
            nextCubieIndex = (i + 1) % 4;
            cubieTileCheckIndex = (cube.cornerPieces[cubieIndex].GetTileIndex(Colors.yellow) + 2) % 3;
            nextCubieTileCheckIndex = (cube.cornerPieces[nextCubieIndex].GetTileIndex(Colors.yellow) + 1) % 3;

            if (cube.cornerPieces[cubieIndex].tiles[cubieTileCheckIndex].color == cube.cornerPieces[nextCubieIndex].tiles[nextCubieTileCheckIndex].color)
            {
                matches++;
                lastMatch = cube.cornerPieces[cubieIndex].tiles[cubieTileCheckIndex];
            }
        }

        if (matches == 0)
        {
            //perform algorithm
            yield return StartCoroutine(Algorithm("F R U' R' U' R U R' F' R U R' U' R' F R F'"));
        }
        else if (matches == 1)
        {
            //rotate up side into position
            if (lastMatch.sideOfCube == SidesOfCube.back)
                yield return StartCoroutine(Move(SidesOfCube.up, true));
            else if (lastMatch.sideOfCube == SidesOfCube.right)
            {
                yield return StartCoroutine(Move(SidesOfCube.up, false));
                yield return StartCoroutine(Move(SidesOfCube.up, false));
            }
            else if (lastMatch.sideOfCube == SidesOfCube.front)
                yield return StartCoroutine(Move(SidesOfCube.up, false));

            //perform algorithm
            yield return StartCoroutine(Algorithm("R U R' U' R' F R R U' R' U' R U R' F'"));
        }

        //EDGES
        //check if an edge is already permutated
        int solvedTiles = 0;
        Tile solvedTile = null;
        if (cube.edgePieces[0].GetTile(SidesOfCube.front).color == cube.cornerPieces[0].GetTile(SidesOfCube.front).color)
        {
            solvedTiles++;
            solvedTile = cube.edgePieces[0].GetTile(SidesOfCube.front);
        }
        if (cube.edgePieces[1].GetTile(SidesOfCube.left).color == cube.cornerPieces[1].GetTile(SidesOfCube.left).color)
        {
            solvedTiles++;
            solvedTile = cube.edgePieces[1].GetTile(SidesOfCube.left);
        }
        if (cube.edgePieces[2].GetTile(SidesOfCube.back).color == cube.cornerPieces[2].GetTile(SidesOfCube.back).color)
        {
            solvedTiles++;
            solvedTile = cube.edgePieces[2].GetTile(SidesOfCube.back);
        }
        if (cube.edgePieces[3].GetTile(SidesOfCube.right).color == cube.cornerPieces[3].GetTile(SidesOfCube.right).color)
        {
            solvedTiles++;
            solvedTile = cube.edgePieces[3].GetTile(SidesOfCube.right);
        }

        if (solvedTiles < 4)
        {
            if (solvedTiles == 1)
            {
                //move top layer into position
                if (solvedTile.sideOfCube == SidesOfCube.left)
                    yield return StartCoroutine(Move(SidesOfCube.up, false));
                else if (solvedTile.sideOfCube == SidesOfCube.front)
                {
                    yield return StartCoroutine(Move(SidesOfCube.up, false));
                    yield return StartCoroutine(Move(SidesOfCube.up, false));
                }
                else if (solvedTile.sideOfCube == SidesOfCube.right)
                    yield return StartCoroutine(Move(SidesOfCube.up, true));

                //determine if clockwise or counterclockwise
                if (cube.edgePieces[1].GetTile(SidesOfCube.left).color == cube.cornerPieces[3].GetTile(SidesOfCube.right).color)
                {
                    //clockwise
                    yield return StartCoroutine(Algorithm("R R U R U R' U' R' U' R' U R'"));
                }
                else if (cube.edgePieces[3].GetTile(SidesOfCube.right).color == cube.cornerPieces[0].GetTile(SidesOfCube.left).color)
                {
                    //counterclockwise
                    yield return StartCoroutine(Algorithm("R U' R U R U R U' R' U' R R"));
                }
            }
            else
            {
                if (cube.edgePieces[0].GetTile(SidesOfCube.front).color == cube.cornerPieces[1].GetTile(SidesOfCube.back).color)
                {
                    //H perm
                    yield return StartCoroutine(Algorithm("M M U' M M U' U' M M U' M M"));
                }
                else
                {
                    //Z perm
                    //rotate top layer into position
                    if (cube.edgePieces[0].GetTile(SidesOfCube.front).color == cube.cornerPieces[3].GetTile(SidesOfCube.right).color)
                        yield return StartCoroutine(Move(SidesOfCube.up, false));

                    //perform algorithm
                    yield return StartCoroutine(Algorithm("M M U' M M U' M' U' U' M M U' U' M'"));
                }
            }
        }

        Colors frontColor = cube.centerPieces[1].tiles[0].color;
        if (cube.edgePieces[1].GetTile(SidesOfCube.left).color == frontColor)
            yield return StartCoroutine(Move(SidesOfCube.up, true));
        else if (cube.edgePieces[2].GetTile(SidesOfCube.back).color == frontColor)
        {
            yield return StartCoroutine(Move(SidesOfCube.up, false));
            yield return StartCoroutine(Move(SidesOfCube.up, false));
        }
        else if (cube.edgePieces[3].GetTile(SidesOfCube.right).color == frontColor)
            yield return StartCoroutine(Move(SidesOfCube.up, false));

        currentStep = null;
        solving = false;
    }

    void Scramble ()
    {
        currentStep = StartCoroutine(ScrambleCo());
    }
    IEnumerator ScrambleCo ()
    {
        scrambling = true;
        int turnsLeft = numTurns;
        while (turnsLeft > 0)
        {
            int rand = Random.Range(1, 9);
            bool prime = (Random.Range(0, 1) == 1);
            switch (rand)
            {
                case 1:
                    yield return StartCoroutine(Move(SidesOfCube.up, prime));
                    break;
                case 2:
                    yield return StartCoroutine(Move(SidesOfCube.down, prime));
                    break;
                case 3:
                    yield return StartCoroutine(Move(SidesOfCube.front, prime));
                    break;
                case 4:
                    yield return StartCoroutine(Move(SidesOfCube.back, prime));
                    break;
                case 5:
                    yield return StartCoroutine(Move(SidesOfCube.right, prime));
                    break;
                case 6:
                    yield return StartCoroutine(Move(SidesOfCube.left, prime));
                    break;
                case 7:
                    yield return StartCoroutine(Move(MidLayersOfCube.Middle, prime));
                    break;
                case 8:
                    yield return StartCoroutine(Move(MidLayersOfCube.Equatorial, prime));
                    break;
                case 9:
                    yield return StartCoroutine(Move(MidLayersOfCube.Standing, prime));
                    break;
            }
            turnsLeft--;
        }
        scrambling = false;
    }
}
