using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    IEnumerator Move (string move, bool prime)
    {
        if (prime)
            move += '\'';
        yield return StartCoroutine(Move(move));
    }
    IEnumerator Move (string move)
    {
        bool prime = (move.Length > 1 && move[1] == '\'');
        switch (move[0])
        {
            case 'U':
                yield return StartCoroutine(Move(SidesOfCube.up, prime));
                break;
            case 'D':
                yield return StartCoroutine(Move(SidesOfCube.down, prime));
                break;
            case 'R':
                yield return StartCoroutine(Move(SidesOfCube.right, prime));
                break;
            case 'L':
                yield return StartCoroutine(Move(SidesOfCube.left, prime));
                break;
            case 'F':
                yield return StartCoroutine(Move(SidesOfCube.front, prime));
                break;
            case 'B':
                yield return StartCoroutine(Move(SidesOfCube.back, prime));
                break;
            case 'M':
                yield return StartCoroutine(Move(MidLayersOfCube.Middle, prime));
                break;
            case 'E':
                yield return StartCoroutine(Move(MidLayersOfCube.Equatorial, prime));
                break;
            case 'S':
                yield return StartCoroutine(Move(MidLayersOfCube.Standing, prime));
                break;
            case 'u':
                StartCoroutine(Move(MidLayersOfCube.Equatorial, !prime));
                yield return StartCoroutine(Move(SidesOfCube.up, prime));
                break;
            case 'd':
                StartCoroutine(Move(MidLayersOfCube.Equatorial, prime));
                yield return StartCoroutine(Move(SidesOfCube.down, prime));
                break;
            case 'r':
                StartCoroutine(Move(MidLayersOfCube.Middle, prime));
                yield return StartCoroutine(Move(SidesOfCube.right, prime));
                break;
            case 'l':
                StartCoroutine(Move(MidLayersOfCube.Middle, !prime));
                yield return StartCoroutine(Move(SidesOfCube.left, prime));
                break;
            case 'f':
                StartCoroutine(Move(MidLayersOfCube.Standing, prime));
                yield return StartCoroutine(Move(SidesOfCube.front, prime));
                break;
            case 'b':
                StartCoroutine(Move(MidLayersOfCube.Standing, !prime));
                yield return StartCoroutine(Move(SidesOfCube.back, prime));
                break;
            case 'x':
                StartCoroutine(Move(SidesOfCube.right, prime));
                StartCoroutine(Move(MidLayersOfCube.Middle, prime));
                yield return StartCoroutine(Move(SidesOfCube.left, !prime));
                break;
            case 'y':
                StartCoroutine(Move(SidesOfCube.up, prime));
                StartCoroutine(Move(MidLayersOfCube.Equatorial, !prime));
                yield return StartCoroutine(Move(SidesOfCube.down, !prime));
                break;
            case 'z':
                StartCoroutine(Move(SidesOfCube.front, prime));
                StartCoroutine(Move(MidLayersOfCube.Standing, prime));
                yield return StartCoroutine(Move(SidesOfCube.back, !prime));
                break;
        }
    }
    IEnumerator Algorithm (string alg)
    {
        string[] moves = alg.Split(new char[] { ' ' });
        for (int i = 0; i < moves.Length; i++)
        {
            yield return StartCoroutine(Move(moves[i]));
        }
    }

    IEnumerator Align (SidesOfCube current, SidesOfCube target, string move, Func<SidesOfCube, SidesOfCube, int> func)
    {
        //if the value is negative, that means the moves should be prime
        int numMoves = func(current, target);
        if (Mathf.Abs(numMoves) == 1 && (move.ToUpper() == "L" || move.ToUpper() == "D" || move.ToUpper() == "B"))
            numMoves *= -1;//since L D and B rotate in the opposite direction as x y and z, the moves need to be reversed

        if (numMoves < 0)
            yield return StartCoroutine(Move(move, true));
        else
        {
            while (numMoves > 0)
            {
                yield return StartCoroutine(Move(move, false));
                numMoves--;
            }
        }
    }
    IEnumerator Align_XAxis (SidesOfCube current, SidesOfCube target, string move)
    {
        yield return StartCoroutine(Align(current, target, move, getFastestAlignMoves_XAxis));
    }
    int getFastestAlignMoves_XAxis (SidesOfCube current, SidesOfCube target)
    {
        if (current == target)
            return 0;

        int cur, tar;
        if (current == SidesOfCube.front)
            cur = 0;
        else if (current == SidesOfCube.up)
            cur = 1;
        else if (current == SidesOfCube.back)
            cur = 2;
        else
            cur = 3;
        if (target == SidesOfCube.front)
            tar = 0;
        else if (target == SidesOfCube.up)
            tar = 1;
        else if (target == SidesOfCube.back)
            tar = 2;
        else
            tar = 3;


        if (cur % 2 == tar % 2)
            return 2;

        if (tar == 0 && cur == 3)
            return 1;
        if (tar == 3 && cur == 0)
            return -1;
        if (tar < cur)
            return -1;
        return 1;
    }
    IEnumerator Align_YAxis (SidesOfCube current, SidesOfCube target, string move)
    {
        yield return StartCoroutine(Align(current, target, move, getFastestAlignMoves_YAxis));
    }
    int getFastestAlignMoves_YAxis (SidesOfCube current, SidesOfCube target)
    {
        if (current == target)
            return 0;

        int cur, tar;
        if (current == SidesOfCube.front)
            cur = 0;
        else if (current == SidesOfCube.left)
            cur = 1;
        else if (current == SidesOfCube.back)
            cur = 2;
        else
            cur = 3;
        if (target == SidesOfCube.front)
            tar = 0;
        else if (target == SidesOfCube.left)
            tar = 1;
        else if (target == SidesOfCube.back)
            tar = 2;
        else
            tar = 3;


        if (cur % 2 == tar % 2)
            return 2;

        if (tar == 0 && cur == 3)
            return 1;
        if (tar == 3 && cur == 0)
            return -1;
        if (tar < cur)
            return -1;
        return 1;
    }
    IEnumerator Align_ZAxis (SidesOfCube current, SidesOfCube target, string move)
    {
        yield return StartCoroutine(Align(current, target, move, getFastestAlignMoves_ZAxis));
    }
    int getFastestAlignMoves_ZAxis (SidesOfCube current, SidesOfCube target)
    {
        if (current == target)
            return 0;

        int cur, tar;
        if (current == SidesOfCube.up)
            cur = 0;
        else if (current == SidesOfCube.right)
            cur = 1;
        else if (current == SidesOfCube.down)
            cur = 2;
        else
            cur = 3;
        if (target == SidesOfCube.up)
            tar = 0;
        else if (target == SidesOfCube.right)
            tar = 1;
        else if (target == SidesOfCube.down)
            tar = 2;
        else
            tar = 3;

        if (cur % 2 == tar % 2)
            return 2;

        if (tar == 0 && cur == 3)
            return 1;
        if (tar == 3 && cur == 0)
            return -1;
        if (tar < cur)
            return -1;
        return 1;
    }
    IEnumerator Align_AnyAxis (SidesOfCube current, SidesOfCube target, string move)
    {
        char upperCaseMove = move.ToUpper()[0];
        if (upperCaseMove == 'R' || upperCaseMove == 'L')
            yield return StartCoroutine(Align_XAxis(current, target, move));
        else if (upperCaseMove == 'U' || upperCaseMove == 'D')
            yield return StartCoroutine(Align_YAxis(current, target, move));
        else if (upperCaseMove == 'F' || upperCaseMove == 'B')
            yield return StartCoroutine(Align_ZAxis(current, target, move));
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
            //check if the cubie is already in its proper place...
            SidesOfCube centerSide = cube.GetCenterFromColor(otherTiles[i].color).tiles[0].sideOfCube;
            bool cubieInPlace = (otherTiles[i].sideOfCube == centerSide);
            if (cubieInPlace)
            {
                yield return StartCoroutine(Align_AnyAxis(whiteTiles[i].sideOfCube, SidesOfCube.down, ConvertMove(otherTiles[i].sideOfCube)));
                continue;
            }

            //move the edge piece to the top layer and orient it properly
            if (otherTiles[i].sideOfCube == SidesOfCube.up)
            {
                //put the cubie and its corresponding non-white side in front
                yield return StartCoroutine(Align_AnyAxis(whiteTiles[i].sideOfCube, SidesOfCube.front, "U"));
                yield return StartCoroutine(Align_AnyAxis(centerSide, SidesOfCube.front, "d"));

                //flip the cubie
                yield return StartCoroutine(Algorithm("F D R' D'"));
            }
            else if (otherTiles[i].sideOfCube == SidesOfCube.down)
            {
                //put the corresponding center in front
                yield return StartCoroutine(Align_AnyAxis(centerSide, SidesOfCube.front, "u"));

                SidesOfCube originalFirstSide = cube.GetCenterFromColor(otherTiles[0].color).tiles[0].sideOfCube;
                //align down side for current edge
                yield return StartCoroutine(Align_AnyAxis(whiteTiles[i].sideOfCube, SidesOfCube.right, "D"));
                //todo: make this also align with the left side, if its faster
                //move current edge out of down side
                yield return StartCoroutine(Move(SidesOfCube.right, false));
                if (i > 0)//realign the down side as it was before if there were any other edges already solved
                    yield return StartCoroutine(Align_AnyAxis(otherTiles[0].sideOfCube, originalFirstSide, "D"));

                //move current edge into place
                yield return StartCoroutine(Move(SidesOfCube.front, false));

                //flip the cubie
                //yield return StartCoroutine(Algorithm("F' D R' D'"));
            }
            else
            {
                SidesOfCube originalWhiteTileSide = whiteTiles[i].sideOfCube;
                string move = ConvertMove(otherTiles[i].sideOfCube);

                //rotate otherTiles[i].side until white is up
                yield return StartCoroutine(Align_AnyAxis(originalWhiteTileSide, SidesOfCube.up, move));

                //move top layer out of the way (align with its proper center)
                yield return StartCoroutine(Align_AnyAxis(otherTiles[i].sideOfCube, centerSide, "U"));

                //undo front side movement, in case another white edge piece was already aligned there
                yield return StartCoroutine(Align_AnyAxis(SidesOfCube.up, originalWhiteTileSide, move));

                //move edge into place
                yield return StartCoroutine(Align_AnyAxis(whiteTiles[i].sideOfCube, SidesOfCube.down, ConvertMove(centerSide)));
            }
            /*else
            {
                //int moveCount = 0;
                //SidesOfCube sideToMove = whiteTiles[i].sideOfCube;
                ////move the cubie up to the top layer
                //while (otherTiles[i].sideOfCube != SidesOfCube.up)
                //{
                //    moveCount++;
                //    yield return StartCoroutine(Move(sideToMove, false));
                //}
                ////move the edge out of the way, then undo the moves on sideToMove
                //if (moveCount > 0)
                //    yield return StartCoroutine(Move(SidesOfCube.up, false));
                //while (moveCount > 0)
                //{
                //    yield return StartCoroutine(Move(sideToMove, true));
                //    moveCount--;
                //}
                //set up cubie
                yield return StartCoroutine(Align_YAxis(whiteTiles[i].sideOfCube, SidesOfCube.front, "U"));
                //while (whiteTiles[i].sideOfCube != SidesOfCube.front)
                //    yield return StartCoroutine(Move(SidesOfCube.up, false));
                //flip the cubie
                yield return StartCoroutine(Algorithm("F R U U R' F' U'"));
            }*/

            ////find the center cubie that is the same color as otherTile
            //SidesOfCube otherColorSide = cube.GetCenterFromColor(otherTiles[i].color).tiles[0].sideOfCube;

            ////move the properly oriented cubie to its side on the cube
            //while (otherTiles[i].sideOfCube != otherColorSide)
            //{
            //    yield return StartCoroutine(Move(SidesOfCube.up, false));
            //}

            ////move the edge piece down to its proper spot
            //yield return StartCoroutine(Move(otherColorSide, false));
            //yield return StartCoroutine(Move(otherColorSide, false));
        }
        yield return null;
        currentStep = StartCoroutine(FirstTwoLayers());
    }
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
            if (whiteTile.sideOfCube == SidesOfCube.down &&
                secondTile.sideOfCube == cube.GetCenterFromColor(secondTile.color).tiles[0].sideOfCube &&
                thirdTile.sideOfCube == cube.GetCenterFromColor(thirdTile.color).tiles[0].sideOfCube)
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
                //rotate cube so that the corner is in the proper place
                yield return StartCoroutine(Align_YAxis(sidewaysTiles[0].sideOfCube, SidesOfCube.front, "y"));
                //todo: implement AlignYAxis() where appropriate. Ex above
                //do algorithm
                yield return StartCoroutine(Algorithm("R U R'"));
            }

            //rotate cube into position
            Tile secondSide = cube.GetCenterFromColor(secondTile.color).tiles[0];
            Tile thirdSide = cube.GetCenterFromColor(thirdTile.color).tiles[0];
            bool bottom2LayersInPlace = (secondSide.sideOfCube == SidesOfCube.front);
            bool cornerInPlace = (cube.whiteSide.corners[i] == cube.cornerPieces[3]);
            
            while (!bottom2LayersInPlace || !cornerInPlace)
            {
                if (!bottom2LayersInPlace && !cornerInPlace)
                {

                    if (secondSide.sideOfCube == SidesOfCube.left)
                        yield return StartCoroutine(Move("y'"));
                    else if (secondSide.sideOfCube == SidesOfCube.back)
                        yield return StartCoroutine(Move("y"));
                    if (secondSide.sideOfCube == SidesOfCube.right)
                        yield return StartCoroutine(Move("y"));
                }
                else if (!bottom2LayersInPlace)
                {
                    if (secondSide.sideOfCube == SidesOfCube.left)
                        yield return StartCoroutine(Move("d"));
                    else if (secondSide.sideOfCube == SidesOfCube.back)
                        yield return StartCoroutine(Move("d'"));
                    if (secondSide.sideOfCube == SidesOfCube.right)
                        yield return StartCoroutine(Move("d'"));
                }
                else if (!cornerInPlace)
                {
                    if (cube.whiteSide.corners[i] == cube.cornerPieces[0])
                        yield return StartCoroutine(Move("U'"));
                    else if (cube.whiteSide.corners[i] == cube.cornerPieces[1])
                        yield return StartCoroutine(Move("U"));
                    if (cube.whiteSide.corners[i] == cube.cornerPieces[2])
                        yield return StartCoroutine(Move("U"));
                }

                bottom2LayersInPlace = (secondSide.sideOfCube == SidesOfCube.front);
                cornerInPlace = (cube.whiteSide.corners[i] == cube.cornerPieces[3]);
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
                    yield return StartCoroutine(Move("y'"));
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
                yield return StartCoroutine(Move("y'"));
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
            yield return StartCoroutine(Algorithm("F R U R' U' S R U R' U' f'"));
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
                        yelTilesUpIndexes[0] = (yelTilesUpIndexes[0] + 3) % 4;//this is kind of an obnoxious looking formula. It just means
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
                yield return StartCoroutine(Algorithm("f R U R' U' f'"));
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
                yield return StartCoroutine(Algorithm("F' r U R' U' r' F R"));
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
                yield return StartCoroutine(Algorithm("r U R' U' r' F R F'"));
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

    string ConvertMove (SidesOfCube side)
    {
        switch (side)
        {
            case SidesOfCube.right:
                return "R";
            case SidesOfCube.left:
                return "L";
            case SidesOfCube.up:
                return "U";
            case SidesOfCube.down:
                return "D";
            case SidesOfCube.front:
                return "F";
            default:
                return "B";
        }
    }
}

//OLD CODE
//if (centerSide == whiteTiles[i].sideOfCube)
//{
//    yield return StartCoroutine(Align_AnyAxis(centerSide, SidesOfCube.front, "y"));
//}
//else if (centerSide == SidesOfCube.front)
//{
//    yield return StartCoroutine(Align_AnyAxis(whiteTiles[i].sideOfCube, SidesOfCube.front, "U"));
//}
//else if (whiteTiles[i].sideOfCube == SidesOfCube.front)
//{
//    yield return StartCoroutine(Align_AnyAxis(centerSide, SidesOfCube.front, "d"));
//}
//else
//{
//    if (centerSide == SidesOfCube.back)
//    {
//        yield return StartCoroutine(Align_AnyAxis(whiteTiles[i].sideOfCube, SidesOfCube.front, "y"));
//        yield return StartCoroutine(Align_AnyAxis(centerSide, SidesOfCube.front, "d"));
//    }
//    else if (whiteTiles[i].sideOfCube == SidesOfCube.back)
//    {
//        yield return StartCoroutine(Align_AnyAxis(centerSide, SidesOfCube.front, "y"));
//        yield return StartCoroutine(Align_AnyAxis(whiteTiles[i].sideOfCube, SidesOfCube.front, "U"));
//    }
//    else
//    {
//        yield return StartCoroutine(Align_AnyAxis(centerSide, SidesOfCube.front, "d"));
//        yield return StartCoroutine(Align_AnyAxis(whiteTiles[i].sideOfCube, SidesOfCube.front, "U"));
//    }
//}